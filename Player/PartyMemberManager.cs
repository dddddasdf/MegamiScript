using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Text;
using System.IO;

#region Observer

/// <summary>
/// 파티 매니저용 옵저버 패턴-발행자 인터페이스
/// </summary>
public interface IPartySubject
{
    void AddObserver(IPartyObserver NewObserver);
    void RemoveObserver(IPartyObserver Observer);
    void NotifyObserver();
}


/// <summary>
/// 파티 매니저용 옵저버 패턴-구독자 인터페이스
/// </summary>
public interface IPartyObserver
{
    void UpdateEntry();
}

#endregion

/// <summary>
/// 속성에 대한 약점~강점
/// </summary>
public enum SkillAffinities
{
    Weak,   //약점 弱
    Resist, //내성 耐
    Void,   //무효 無
    Reflect,    //반사 反
    Drain   //흡수 吸
}

[Flags]
public enum AilmentType
{
    None = 0   //상태이상 없음

    , Poison = 1 << 0    //독 - 전투에서 행동시 체력 감소, 필드에서 이동시 체력 감소/자동 회복X
    , Sick = 1 << 1      //감기 - 전투에서 공격력 25% 하락, 회피율 0%로 하락, 턴 종료마다 5%의 확률로 타 아군 캐릭터에게 감염/자동 회복X
    , Panic = 1 << 2     //혼란 - 조작 불가능, 마음대로 행동함 / 전투 중 및 전투 종료시 자동 회복
    , Bind = 1 << 3      //결박 - 행동 불가능, 결박 상태의 악마에게 각종 대화시 플러스 효과 / 전투 중 및 전투 종료시 자동 회복
    , Sleep = 1 << 4      //수면 - 행동 불가능, 피격시 25% 추가 대미지, 피격시 회복되지 않음 / 전투 중 및 전투 종료시 자동 회복

    , All = int.MaxValue    //치료기 전용
}

#region ClassField
[System.Serializable][JsonObject(MemberSerialization.OptIn)]
public class PartyMemberData
{
    public PartyMemberData()
    {

    }
    
    #region Field
    [JsonProperty] private string Name = "tst"; // { get; init; }     //이름
    [JsonProperty] private int MaxHP = 0;       //최대 체력
    [JsonProperty] private int NowHP = 0;       //현재 체력
    [JsonProperty] private int MaxMP = 0;       //최대 마나
    [JsonProperty] private int NowMP = 0;       //현재 마나
    [JsonProperty] private bool IsDead = false;     //사망 판정

    [JsonProperty] private int St = 0;          //힘-칼 기본 공격 위력 상승, 물리 스킬 위력 소폭 상승
    [JsonProperty] private int Dx = 0;          //기-총 기본 공격 위력 상승, 물리 스킬 위력 대폭 상승
    [JsonProperty] private int Ma = 0;          //마-마법 스킬 위력 상승
    [JsonProperty] private int Ag = 0;          //속-선제턴 확률, 행동순서, 명중률, 회피율 상승
    [JsonProperty] private int Lu = 0;          //운-크리티컬, 회피율, 상태이상 성공률, 상태이상 회복속도, 아이템 드랍률 상승

    [JsonProperty] private AilmentType NowAilment = AilmentType.None;  //현재 걸려있는 상태이상

    [JsonProperty] private int BeginningLevel = 0;     //초기 레벨<-악마 쪽에만 옮겨둘 것
    [JsonProperty] private int Level = 0;       //현재 레벨
    [JsonProperty] private int EXP = 0;         //현재 경험치

    [JsonIgnore] private List<SkillDataRec> LearnedSkillList = new List<SkillDataRec>();     //습득한 스킬 리스트

    [JsonProperty] private SkillAffinities? AffinityPhysical { get; init; }
    [JsonProperty] private SkillAffinities? AffinityGun { get; init; }
    [JsonProperty] private SkillAffinities? AffinityFire { get; init; }
    [JsonProperty] private SkillAffinities? AffinityIce { get; init; }
    [JsonProperty] private SkillAffinities? AffinityElectric { get; init; }
    [JsonProperty] private SkillAffinities? AffinityForce { get; init; }
    [JsonProperty] private SkillAffinities? AffinityLight { get; init; }
    [JsonProperty] private SkillAffinities? AffinityDark { get; init; }

    [JsonProperty] private bool IsNormalGun = false; // { get; init; }        //일반 공격이 총속성일 경우 true, 그 외에는 false
    [JsonProperty] private int NormalMinHit = 0; //{ get; init; }        //일반 공격의 최소 타수: 일부 악마를 제외하면 통상적으로는 1회
    [JsonProperty] private int NormalMaxHit = 0; //{ get; init; }        //일반 공격의 최대 타수: 일부 악마를 제외하면 통상적으로는 1회


    #endregion

    /// <summary>
    /// 체력 감소 함수
    /// </summary>
    /// <param name="Value">대미지 수치</param>
    public virtual void LossHP(int Value, out bool IsTargetDead)
    {
        NowHP -= Value;

        //현재 체력이 0 이하가 되면 사망 처리
        if (NowHP <= 0)
        {
            NowHP = 0;
            IsDead = true;
        }
        IsTargetDead = IsDead;
    }

    /// <summary>
    /// 부활
    /// </summary>
    /// <param name="Value">부활시 가지고 부활할 체력</param>
    public void Revival(int Value)
    {
        IsDead = false;
        NowHP = Value;
    }

    /// <summary>
    /// 현재 사망했는지 반환
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsDead()
    {
        return IsDead;
    }

    /// <summary>
    /// 힐 스킬, 물약 등으로 체력 회복
    /// </summary>
    /// <param name="Value">회복 수치</param>
    public void RecoverHP(int? Value, out int RecovredHPValue)
    {
        if (Value == null)
        {
            //풀체력 회복템일 경우 약간의 계산만 거치고 바로 종료
            RecovredHPValue = MaxHP - NowHP;
            NowHP = MaxHP;
        }
        
        int Tmp = (int)Value;   //형변환용 변수

        if ((int)Value < MaxHP - NowHP)
        {
            RecovredHPValue = (int)Tmp;
            NowHP += Tmp;
        }
        else
        {
            RecovredHPValue = MaxHP - NowHP;
            NowHP = MaxHP;
        }
        
    }

    /// <summary>
    /// 힐 스킬, 물약 등으로 마나 회복
    /// </summary>
    /// <param name="Value">회복 수치</param>
    public void RecoverMP(int? Value, out int RecovredMPValue)
    {
        if (Value == null)
        {
            //풀마나 회복템일 경우 약간의 계산만 거치고 바로 종료
            RecovredMPValue = MaxMP - NowMP;
            NowMP = MaxMP;
        }

        int Tmp = (int)Value;   //형변환용 변수

        if ((int)Value < MaxMP - NowMP)
        {
            RecovredMPValue = (int)Tmp;
            NowMP += Tmp;
        }
        else
        {
            RecovredMPValue = MaxMP - NowMP;
            NowMP = MaxMP;
        }

    }

    /// <summary>
    /// 상태이상 적용
    /// </summary>
    /// <param name="newAil">추가할 상태이상</param>
    public void SetAil(AilmentType newAil)
    {
        NowAilment |= newAil;
    }

    /// <summary>
    /// 걸려 있는 상태이상 제거
    /// </summary>
    /// <param name="CureAilmentType">제거 대상 상태이상</param>
    public void CureAil(AilmentType CureAilmentType)
    {
        //현재 가지고 있는 상태이상이 없을 경우 더 이상의 처리 없이 종료
        if (NowAilment == AilmentType.None)
            return;             
        
        //들어온 치유기가 모든 상태이상 치유기일 경우 가지고 있는 상태이상을 비워버리고 종료
        if (CureAilmentType.HasFlag(AilmentType.All))
        {
            NowAilment = AilmentType.None;
            return;
        }

        /*
        HasFlag로 제거 대상 상태이상 리스트에 해당 상태이상이 존재하는지 확인하고, 적용 대상 캐릭터가 해당 상태이상을 가지고 있는지 확인 후 맞다면 해당 상태이상을 제거 
        */
        //독 치료
        if (CureAilmentType.HasFlag(AilmentType.Poison) && NowAilment.HasFlag(AilmentType.Poison))
        {
            NowAilment &= ~AilmentType.Poison;
        }

        //감기 치료
        if (CureAilmentType.HasFlag(AilmentType.Sick) && NowAilment.HasFlag(AilmentType.Sick))
        {
            NowAilment &= ~AilmentType.Sick;
        }

        //속박 치료
        if (CureAilmentType.HasFlag(AilmentType.Bind) && NowAilment.HasFlag(AilmentType.Bind))
        {
            NowAilment &= ~AilmentType.Bind;
        }

        //혼란 치료
        if (CureAilmentType.HasFlag(AilmentType.Panic) && NowAilment.HasFlag(AilmentType.Panic))
        {
            NowAilment &= ~AilmentType.Panic;
        }

        //수면 치료
        if (CureAilmentType.HasFlag(AilmentType.Sleep) && NowAilment.HasFlag(AilmentType.Sleep))
        {
            NowAilment &= ~AilmentType.Sleep;
        }
    }

    /// <summary>
    /// 휴식 기능 또는 레벨업을 이용해서 모든 상태 회복 
    /// </summary>
    public void RecoverAllByRest()
    {
        NowHP = MaxHP;
        NowMP = MaxMP;
        CureAil(AilmentType.All);
    }


    #region ReturnStatus

    /// <summary>
    /// 이름 반환
    /// </summary>
    /// <returns></returns>
    public string ReturnName()
    {
        return Name;
    }

    /// <summary>
    /// 힘 스탯 수치 반환
    /// </summary>
    /// <returns>힘 스탯</returns>
    public int ReturnSt()
    {
        return St;
    }

    /// <summary>
    /// 기 스탯 수치 반환
    /// </summary>
    /// <returns>기 스탯</returns>
    public int ReturnDx()
    {
        return Dx;
    }

    /// <summary>
    /// 마 스탯 수치 반환
    /// </summary>
    /// <returns>마 스탯</returns>
    public int ReturnMa()
    {
        return Ma;
    }

    /// <summary>
    /// 속 스탯 수치 반환
    /// </summary>
    /// <returns>속 스탯</returns>
    public int ReturnAg()
    {
        return Ag;
    }

    /// <summary>
    /// 운 스탯 수치 반환
    /// </summary>
    /// <returns>운 스탯</returns>
    public int ReturnLu()
    {
        return Lu;
    }

    /// <summary>
    /// 객체의 약점/내성 반환
    /// </summary>
    /// <param name="AttackedSkillType"></param>
    /// <returns></returns>
    public SkillAffinities? ReturnMatchingAffinity(SkillTypeSort AttackedSkillType)
    {
        switch (AttackedSkillType)
        {
            case SkillTypeSort.Physical:
                return AffinityPhysical;
            case SkillTypeSort.Gun:
                return AffinityGun;
            case SkillTypeSort.Fire:
                return AffinityFire;
            case SkillTypeSort.Ice:
                return AffinityIce;
            case SkillTypeSort.Electric:
                return AffinityElectric;
            case SkillTypeSort.Force:
                return AffinityForce;
            case SkillTypeSort.Light:
                return AffinityLight;
            case SkillTypeSort.Dark:
                return AffinityDark;
            default:
                return null;
        }
    }

    /// <summary>
    /// 일반 공격이 총타입인지 리턴
    /// </summary>
    /// <returns>총타입일 경우 true</returns>
    public bool ReturnIsNormalGun()
    {
        return IsNormalGun;
    }

    /// <summary>
    /// 일반 공격 최소 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnNormalMinHit()
    {
        return NormalMinHit;
    }

    /// <summary>
    /// 일반 공격 최대 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnNormalMaxHit()
    {
        return NormalMaxHit;
    }

    /// <summary>
    /// 감기에 걸렸는지 파악용: 걸렸으면 공깎 디버프를 걸어야 하므로
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsSick()
    {
        return (NowAilment.HasFlag(AilmentType.Sick));
    }

    /// <summary>
    /// 초기 레벨 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnBeginningLv()
    {
        return BeginningLevel;
    }

    /// <summary>
    /// 습득한 스킬 개수 반환
    /// </summary>
    /// <returns>습득한 스킬 개수</returns>
    public int ReturnNumberOfSkill()
    {
        return LearnedSkillList.Count;
    }

    public SkillDataRec ReturnSkillByIndex(int Index)
    {
        return LearnedSkillList[Index];
    }

    /// <summary>
    /// 최대 체력 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnMaxHP()
    {
        return MaxHP;
    }


    /// <summary>
    /// 최대 마나 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnMaxMP()
    {
        return MaxMP;
    }

    /// <summary>
    /// 현재 체력 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnNowHP()
    {
        return NowHP;
    }

    /// <summary>
    /// 현재 마나 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnNowMP()
    {
        return NowMP;
    }


    #endregion

    public void AddNewSkill(SkillDataRec NewSkill)
    {
        //스킬은 최대 8개까지 습득 가능
        if (LearnedSkillList.Count < 8)
        {
            LearnedSkillList.Add(NewSkill);
        }
    }
}

[System.Serializable][JsonObject(MemberSerialization.OptIn)]
public class PartyDemonData : PartyMemberData
{
    [JsonProperty] private int ID = 0;          //악마 식별 번호
    [JsonProperty] private bool IsEntry = false;    //출전 중인지 확인용

    
    /// <summary>
    /// 악마 ID 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnID()
    {
        return ID;
    }

    /// <summary>
    /// 현재 엔트리에 있는지 반환
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsEntry()
    {
        return IsEntry;
    }

    /// <summary>
    /// 엔트리 교체
    /// </summary>
    /// <param name="Value"></param>
    public void SwapEntry(bool Value)
    {
        IsEntry = Value;
    }

    /// <summary>
    /// 동료 악마는 체력이 다하면 엔트리에서 자동으로 빠지므로 오버라이딩해서 추가적인 부분을 붙인다
    /// </summary>
    /// <param name="Value"></param>
    public override void LossHP(int Value, out bool IsTargetDead)
    {
        base.LossHP(Value, out IsTargetDead);

        if (base.ReturnIsDead())
        {
            //파티 멤버 관리자에게 자신을 엔트리에서 빼달라고 전달
            GameManager.Instance.ReturnPartyManager().RemoveInEntry(this);
        }
    }

    /// <summary>
    /// 엔트리에서 빼달라고 요청
    /// </summary>
    public void CallRemoveThis()
    {
        GameManager.Instance.ReturnPartyManager().RemoveInEntry(this);
    }
}

[System.Serializable]
public class PlayerCharacterData : PartyMemberData
{

}

#endregion

public class PartyMemberManager : IPartySubject
{
    #region ObserverFiled

    private List<IPartyObserver> thisObserverList = new List<IPartyObserver>();   //구독자 리스트

    /// <summary>
    /// 구독자 추가
    /// </summary>
    /// <param name="NewObserver"></param>
    public void AddObserver(IPartyObserver NewObserver)
    {
        thisObserverList.Add(NewObserver);
    }

    /// <summary>
    /// 구독자 리스트에서 제거
    /// </summary>
    /// <param name="Observer"></param>
    public void RemoveObserver(IPartyObserver Observer)
    {
        if (thisObserverList.Contains(Observer))
            thisObserverList.Remove(Observer);
    }

    public void NotifyObserver()
    {
        foreach(IObserver Observer in thisObserverList)
        {
            //Observer.Update(this);
        }
    }

    #endregion

    private readonly JsonSerializerSettings serializerSetting = new()
    {
        Formatting = Formatting.Indented,
        ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
    };

    public void SaveTMP()
    {
        PartyDemonData TMP = new PartyDemonData();
        TMP.AddNewSkill(TestScript.Instance.ReturnClass().ReturnSkillData(1));

        

        StringBuilder SaveSB = new StringBuilder();
        SaveSB.Append("test.json");
        string JsonFile;
        //= JsonUtility.ToJson(TMP);
        JsonFile = JsonConvert.SerializeObject(TMP, Formatting.Indented);
        Debug.Log(JsonFile);
        string SaveDataDirectory = Application.persistentDataPath + "/SaveData/";
        File.WriteAllText(SaveDataDirectory + SaveSB.ToString(), JsonFile);
    }

    /// <summary>
    /// 테스트용 임시 함수
    /// </summary>
    public void LoadTMP()
    {
        AsyncOperationHandle<TextAsset> TextAssetHandle;
        TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("TestParty");

        TextAssetHandle.Completed += Handle =>
        {
            if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
            {
                int a = 100;
            }

            PartyDemonList = JsonConvert.DeserializeObject<List<PartyDemonData>>(TextAssetHandle.Result.text);
            EntryDemonList = PartyDemonList;
        };

        //AsyncOperationHandle<TextAsset> TextAssetHandle2;
        //TextAssetHandle2 = Addressables.LoadAssetAsync<TextAsset>("TestPlayer");
        //TextAssetHandle2.Completed += Handle =>
        //{
        //    if (TextAssetHandle2.Status == AsyncOperationStatus.Failed)
        //    {
        //        int b = 100;
        //    }
        //
        //    PlayerCharacter = JsonConvert.DeserializeObject<PlayerCharacterData>(TextAssetHandle2.Result.text);
        //    Addressables.Release(TextAssetHandle2);
        //};

        int i = 0;
    }

    private PlayerCharacterData PlayerCharacter;
    private List<PartyDemonData> PartyDemonList = new List<PartyDemonData>();        //전체 동료 악마 리스트
#nullable enable
    private List<PartyDemonData?> EntryDemonList = new List<PartyDemonData?>();     //출전 동료 악마 리스트(총 셋), 들어간 악마가 없이 비워진 칸은 null 처리
#nullable disable


    private int MaxNumberOfDemon = 32;          //동시에 스톡에 넣을 수 있는 악마의 최대 숫자 임의 설정

    /// <summary>
    /// 게임 구동 후 현재 스톡에 있는 동료 악마 리스트 초기화
    /// </summary>
    private void InitPartyDemonList()
    {
        //동료 악마 관련 List의 최대 길이는 정해져 있으므로 미리 메모리를 할당해서 넣고 뺄 때 재할당이 이루어지지 않게 한다
        PartyDemonList.Capacity = MaxNumberOfDemon;
        EntryDemonList.Capacity = 3;
    }

    /// <summary>
    /// 게임 구동 후 현재 출전 중인 동료 악마 리스트 초기화
    /// </summary>
    private void InitEntryDemonList()
    {

    }

    /// <summary>
    /// 출전 동료 악마 교체
    /// </summary>
    /// <param name="PartyIndex"></param>
    /// <param name="EntryIndex"></param>
    public void SwapEntryDemon(int PartyIndex, int EntryIndex)
    {
        //들어가려는 공간이 비어있는 공간(null)인지 체크, null이라면 바로 넣어버리면 된다
        if (EntryDemonList[EntryIndex] != null)
            EntryDemonList[EntryIndex].SwapEntry(false);        //먼저 있던 녀석을 엔트리에서 제외로 설정


        EntryDemonList[EntryIndex] = PartyDemonList[PartyIndex];        //있던 악마와 교체할 악마 교환
        EntryDemonList[EntryIndex].SwapEntry(true);         //새로 엔트리에 들어간 악마를 엔트리 포함으로 설정
    }

    /// <summary>
    /// 악마를 엔트리에서 제거
    /// </summary>
    /// <param name="CallTarget">호출한 대상</param>
    public void RemoveInEntry(PartyDemonData CallTarget)
    {
        int CallTargetIndex;        //호출한 객체가 리스트에서 어떤 인덱스를 갖고 있는지 담을 변수

        //호출한 객체가 제대로 리스트에 존재하는 게 맞다면 이행한다
        if (EntryDemonList.Contains(item: CallTarget))
        {
            CallTargetIndex = EntryDemonList.IndexOf(item: CallTarget);
            CallTarget.SwapEntry(false);
            EntryDemonList[CallTargetIndex] = null;

            foreach (IObserver Observer in thisObserverList)
            {
                //Observer.Update(this);
            }
        }
    }

    /// <summary>
    /// 주인공 캐릭터 데이터 반환
    /// </summary>
    /// <returns></returns>
    public PlayerCharacterData ReturnPlayerCharacterData()
    {
        return PlayerCharacter;
    }

#nullable enable
    /// <summary>
    /// 엔트리에 들어가 있는 동료 악마 데이터 반환
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public PartyDemonData? ReturnEntryDemonData(int Index)
    {
        return EntryDemonList[Index];
    }
#nullable disable



    /// <summary>
    /// 객체에게 회복 함수 호출
    /// </summary>
    /// <param name="Target">적용 대상</param>
    /// <param name="Value">회복 수치</param>
    /// <param name="RecovredHPValue">실제 회복된 수치</param>
    public void CallRecoverHP(PartyDemonData Target, int? Value, out int RecovredHPValue)
    {
        Target.RecoverHP(Value, out RecovredHPValue);
    }

    /// <summary>
    /// 이름을 반환하라고 객체에게 전달
    /// </summary>
    /// <param name="Target">대상</param>
    /// <returns>대상의 이름</returns>
    public string CallReturnName(PartyDemonData Target)
    {
        return Target.ReturnName();
    }

    /// <summary>
    /// 공격시 약점/내성 체크하라고 객체에게 전달
    /// </summary>
    /// <param name="Target">공격을 받는 파티원</param>
    /// <param name="AttackedSkillType">공격 스킬의 타입</param>
    /// <returns></returns>
    public SkillAffinities? CallReturnAffinity(PartyDemonData Target, SkillTypeSort AttackedSkillType)
    {
        return Target.ReturnMatchingAffinity(AttackedSkillType);
    }

    
}