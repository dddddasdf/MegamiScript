using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

/// <summary>
/// init 기능 사용을 가능하게 해주는 단락. 지우지 말 것
/// </summary>
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

//게임 구동시 스킬 효과들이 수록된 json을 파싱해 내용을 저장하는 클래스

#region Enums

public enum SkillTypeSort
{
    Physical,       //물리
    Gun,            //총
    Fire,           //아기-화염
    Ice,            //부흐-얼음
    Electric,       //지오-전기
    Force,          //잔-충격
    Light,          //하마-빛
    Dark,           //무드-주살
    Almighty,       //메기도-만능
    //여기까지 공격 계열

    Recover,    //회복
    Support,    //기타 버프&디버프기
    Passive,     //패시브기
    Ailment      //상태이상기
}

/// <summary>
/// 스킬이 적중하는 개체의 수
/// </summary>
public enum NumberOfTarget
{
    Single,     //단일
    Multi,      //다수
    All         //전체
}

/// <summary>
/// 스킬 대상
/// </summary>
public enum WhichTarget
{
    Foe,    //적
    Party,  //아군
    Self    //자신만
}

/// <summary>
/// 부가 효과 및 부여 상태이상 종류
/// </summary>
public enum EffectType
{
    Critical,       //크리티컬 확률 증가
    DrainHP,        //HP 흡수
    DrainMP,        //MP 흡수
    DrainHPMP,      //HP, MP 흡수
    Drain,
    InstantKill,    //즉사
    //여기까지 공격 계열

    Buff,           //버프기
    Debuff,         //디버프기
    //여기까지 버프 계열

    RecoverAllHP,
    RecoverAil1,
    RecoverAil2,
    RecoverAllAil, 
    RecoverAllHPAil,
    RecoverHP,
    Recover,
    //여기까지 회복 계열

    InflictSleep,
    InflictPoison,
    InflictSick,
    InflictPanic,
    InflictBind,
    Inflict,
    //여기까지 상태이상 계열 - 공격 스킬 중 상태이상을 부여하는 스킬이 있다면 병행 사용 가능
}

/// <summary>
/// 버프/디버프기에서 증가 및 감소가 적용되는 스탯
/// </summary>
[Flags]
public enum TargetStats
{
    None = 0,           //버프 대상 스탯 없음
    Atk = 1 << 0,    //공격력
    Def = 1 << 1,   //방어력
    Acc = 1 << 2,  //명중률
    Avd = 1 << 3      //회피율
}

#endregion


[System.Serializable][JsonObject(MemberSerialization.OptIn)]
public record SkillDataRec
{
    /// <summary>
    /// Init으로 인해 생성자를 명시해줘야 역직렬화가 가능하다
    /// </summary>
    /// <param name="skillType"></param>
    /// <param name="iD"></param>
    /// <param name="rank"></param>
    /// <param name="name"></param>
    /// <param name="useMP"></param>
    /// <param name="target"></param>
    /// <param name="targetNuber"></param>
    /// <param name="minHits"></param>
    /// <param name="maxHits"></param>
    /// <param name="power"></param>
    /// <param name="accuracy"></param>
    /// <param name="description"></param>
    /// <param name="addEffect"></param>
    /// <param name="effectValue"></param>
    //public SkillDataRec(SkillTypeSort skillType, int iD, int rank, string name, int useMP, WhichTarget target, NumberOfTarget? targetNuber, int minHits, int maxHits, int? power,
    //    int accuracy, string description, EffectType? addEffect, int? effectValue)
    //{
    //    SkillType = skillType;
    //    ID = iD;
    //    Rank = rank;
    //    Name = name;
    //    UseMP = useMP;
    //    Target = target;
    //    thisTargetNumber = targetNuber;
    //    MinHits = minHits;
    //    MaxHits = maxHits;
    //    Power = power;
    //    Accuracy = accuracy;
    //    Description = description;
    //    AddEffect = addEffect;
    //    EffectValue = effectValue;
    //}

    
    [JsonProperty] private SkillTypeSort SkillType { get; init; } //스킬 유형
    [JsonProperty] private int ID { get; init; }              //식별 ID
    [JsonProperty] private int Rank { get; init; }            //스킬 랭크
    [JsonProperty] private string Name { get; init; }         //스킬 이름
    [JsonProperty] private int UseMP { get; init; }           //소모 마나
    [JsonProperty] private WhichTarget Target { get; init; }  //적용 대상
    [JsonProperty] private NumberOfTarget thisTargetNumber { get; init; }   //스킬 대상 개체 수
    [JsonProperty] private int MinHits { get; init; }         //최소 횟수
    [JsonProperty] private int MaxHits { get; init; }         //최다 횟수
    [JsonProperty] private int? Power { get; init; }          //위력: 위력이 필요 하지 않은 스킬일 경우 null
    [JsonProperty] private int Accuracy { get; init; }        //명중률
    [JsonProperty] private string Description { get; init; }  //스킬 설명
    [JsonProperty] private EffectType? AddEffect { get; init; }        //부가 효과: 없을 경우 null
    [JsonProperty] private int? EffectValue { get; init; }                    //부가 효과의 수치: 없을 경우 null
    /*
    부가 효과의 수치에 대한 세부 설명
    -물리 스킬 중 크리티컬율 증가의 경우: 크리티컬율이 증가하는 고정 수치
    -만능 스킬의 경우: 흡수율 퍼센티지
    -상태이상 스킬의 경우: 해당 상태이상이 부여될 확률
    */
    [JsonProperty] private TargetStats? EffectStats { get; init; }        //추가 효과(흡수, 회복, 버프, 디버프기)에서 대상이 되는 스탯

    /// <summary>
    /// 스킬명 반환
    /// </summary>
    /// <returns></returns>
    public string ReturnName()
    {
        return Name;
    }

    /// <summary>
    /// ID값 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnID()
    {
        return ID;
    }

    /// <summary>
    /// 소모 마나 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnUseMP()
    {
        return UseMP;
    }

    /// <summary>
    /// 위력 값 반환
    /// </summary>
    /// <returns>스킬의 위력</returns>
    public int? ReturnPower()
    {
        return Power;
    }

    /// <summary>
    /// 최소 히트 수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnMinHit()
    {
        return MinHits;
    }

    /// <summary>
    /// 최대 히트 수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnMaxHit()
    {
        return MaxHits;
    }

    /// <summary>
    /// 스킬 타입 반환
    /// </summary>
    /// <returns></returns>
    public SkillTypeSort ReturnSkillType()
    {
        return SkillType;
    }

    public NumberOfTarget ReturnNumberOfTarget()
    {
        return thisTargetNumber;
    }
}

/// <summary>
/// 유저와 몬스터는 대조에 필요한 최소한의 정보만 가지고 있게 한다
/// </summary>
[System.Serializable]
[JsonObject(MemberSerialization.OptIn)]
public record SimplifySkillDataRec
{
    private int ID { get; init; }
    private string Name { get; init; }

    /// <summary>
    /// 악마 한정으로 기술이 필요한 곳(주인공은 null로 저장) 습득하기 위해 도달해야 하는 레벨
    /// </summary>
    private int? RequireLevel { get; init; }
}


public class SkillDatabaseManager
{
    //private static List<SkillDataBase> SkillDataBaseList;
    //private List<AttackSkillClass> AttackSkillList = new List<AttackSkillClass>();
    //private List<RecoverSkillClass> RecoverSkillList = new List<RecoverSkillClass>();
    //private List<SkillDataClass> SkillList = new List<SkillDataClass>();

    private List<SkillDataRec> SkillRecList = new List<SkillDataRec>();
    private Queue<Action> AfterInitJobQueue = new Queue<Action>();      //초기화 작업 후 순차적으로 진행시킬 작업 목록

    /// <summary>
    /// 임시용
    /// </summary>
    public void InitSkillDatabaseManager()
    {
        LoadSkillRecDatabasse();
    }


    private void LoadSkillRecDatabasse()
    {
        AsyncOperationHandle<TextAsset> TextAssetHandle;
        TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("SkillData");

        TextAssetHandle.Completed += Handle =>
        {
            if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
            {

            }
            SkillRecList = JsonConvert.DeserializeObject<List<SkillDataRec>>(TextAssetHandle.Result.text);

            while (AfterInitJobQueue.Count > 0)
            {
                Action action = AfterInitJobQueue.Dequeue();
                action?.Invoke();
            }
            AfterInitJobQueue = null;       //작업큐 비워주기
        };
    }

    public void AddJobQueueMethod(Action Method)
    {
        AfterInitJobQueue.Enqueue(Method);
    }

    /// <summary>
    /// ID값이 일치하는 스킬 반환
    /// </summary>
    /// <param name="SkilliD"></param>
    /// <returns></returns>
    public SkillDataRec ReturnSkillData(int SkilliD)
    {
        return SkillRecList.Find(x => (x.ReturnID()) == SkilliD);
    }


    /*
    추후 구현해야 하는 메소드
    1. ID값을 파라미터로 전달 받아서 ID값이 일치하는 레코드를 탐색 후 해당 레코드 반환(깊은 복사가 필요할지 얕은 복사가 필요할진 좀 더 고려 必)
    2. 스킬 변형이 발생하면 rank값을 파라미터로 전달 받아서 근사한 rank값들을 가진 레코드들을 추린 후 rand로 임의 하나 추출 및 반환
    
    */


    #region NotUse
    //
    ///// <summary>
    ///// 스킬 종류 대분류
    ///// </summary>
    //public enum SkillSort
    //{
    //    Attack,     //공격
    //    Recover,    //회복
    //    Support,    //기타 버프&디버프기
    //    Passive,     //패시브기
    //    Ailment      //상태이상기
    //}


    ///// <summary>
    ///// 공격 스킬 유형
    ///// </summary>
    //public enum AttackSkillType
    //{
    //    Physical,       //물리
    //    Gun,            //총
    //    Fire,           //아기
    //    Ice,            //부스
    //    Electric,       //지오
    //    Force,          //잔
    //    Light,          //하마
    //    Dark,           //무드
    //    Almighty        //메기도(만능)
    //}

    ///// <summary>
    ///// 회복 스킬 유형
    ///// </summary>
    //public enum RecoverSkillyType
    //{
    //    RecoverHP
    //}



    ///// <summary>
    ///// 공격 스킬의 부가 효과
    ///// </summary>
    //public enum AddtionalEffect
    //{
    //    None,           //효과 없음
    //    Critical,       //크리티컬 확률 증가
    //    DrainHP,        //HP 흡수
    //    DrainMP,        //MP 흡수
    //    DrainHPMP,      //HP, MP 흡수

    //}


    ///// <summary>
    ///// 보조 스킬의 효과
    ///// </summary>
    //public enum SupportEffect
    //{

    //}

    ///// <summary>
    ///// 보조 스킬 대상
    ///// </summary>
    //public enum SupportTarget
    //{
    //    Self,       //자기 자신
    //    Party,      //아군 전원
    //    Foe         //적 전원
    //}

    ///// <summary>
    ///// 공격, 회복, 버프가 공통적으로 가지는 부분
    ///// </summary>
    //public class Skill
    //{
    //    public string Name;         //스킬 이름
    //    public int ID;              //식별 ID
    //    public int UseMP;           //소모 마나
    //    public NumberOfTarget thisTargetNumber;   //스킬 대상 개체 수
    //    public int MinHits;         //최소 횟수
    //    public int MaxHits;         //최다 횟수
    //    public int Rank;            //스킬 랭크
    //    public int Accuracy;        //명중률
    //    public string Description;  //스킬 설명
    //}

    ///// <summary>
    ///// 공격 스킬 클래스
    ///// </summary>
    //[SerializeField]
    //public class AttackSkillClass : Skill
    //{
    //    public AttackSkillType thisSkillType;
    //    public int Power;     //위력
    //    public AddtionalEffect AddEffect;       //부가 효과
    //    public int? Value;                      //부가 효과가 존재할 경우 관련 수치-부가 효과가 없을 경우 null
    //}

    ///// <summary>
    ///// 회복 스킬 클래스
    ///// </summary>
    //[SerializeField]
    //public class RecoverSkillClass : Skill
    //{
    //    public RecoverSkillyType thisSkillType;
    //    public int Power;     //위력
    //}

    ///// <summary>
    ///// 보조 스킬 클래스
    ///// </summary>
    //[SerializeField]
    //public class SupportSkillClass : Skill
    //{
    //    public SupportTarget Target;       //스킬 대상이 아군인지 적인지
    //    public SupportEffect Effect;    //보조 효과
    //    int? Value;                     //보조 효과의 수치
    //}
    //[System.Serializable]
    //public class SkillDataClass
    //{
    //    public SkillTypeSort SkillType; //스킬 유형
    //    public int ID;              //식별 ID
    //    public int Rank;            //스킬 랭크
    //    public string Name;         //스킬 이름
    //    public int UseMP;           //소모 마나
    //    public WhichTarget Target;  //적용 대상
    //    public NumberOfTarget? thisTargetNumber;   //스킬 대상 개체 수
    //    public int MinHits;         //최소 횟수
    //    public int MaxHits;         //최다 횟수
    //    public int? Power;          //위력: 위력이 필요 하지 않은 스킬일 경우 null
    //    public int Accuracy;        //명중률
    //    public string Description;  //스킬 설명
    //    public EffectType? AddEffect;        //부가 효과: 없을 경우 null
    //    public int? EffectValue;                    //부가 효과의 수치: 없을 경우 null
    //    /*
    //    부가 효과의 수치에 대한 세부 설명
    //    -물리 스킬 중 크리티컬율 증가의 경우: 크리티컬율이 증가하는 고정 수치
    //    -만능 스킬의 경우: 흡수율 퍼센티지
    //    -상태이상 스킬의 경우: 해당 상태이상이 부여될 확률
    //    */
    //    public TargetStats? EffectStats;        //버프 또는 디버프기에서 대상이 되는 스탯
    //}

    ////public string GetSkillInfo(SkillSort GetSkillType, int SkilliD)
    ////{
    ////    switch(GetSkillType)
    ////    {
    ////        case SkillSort.Attack:
    ////            return AttackSkillList.Find(x => x.ID == SkilliD).Description;
    ////        //case SkillSort.Recover:
    ////        //    break;
    ////        //case SkillSort.Support:
    ////        //    break;
    ////        //case SkillSort.Ailment:
    ////        //    break;
    ////        //case SkillSort.Passive:
    ////            //break;
    ////        default:
    ////            return "데이터 오류";
    ////    }
    ////}

    /////// <summary>
    /////// 스킬 DB 불러들이기
    /////// </summary>
    ////private void LoadSkillDatabasse()
    ////{
    ////    AsyncOperationHandle<TextAsset> TextAssetHandle;
    ////    TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("SkillData");

    ////    TextAssetHandle.Completed += Handle =>
    ////    {
    ////        if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
    ////        {

    ////        }
    ////        SkillList = JsonConvert.DeserializeObject<List<SkillDataClass>>(TextAssetHandle.Result.text);
    ////    };
    ////}

    ///// <summary>
    ///// 악마 DB파일 정보값 목적의 간결한 클래스: 이걸 통해 대응하는 스킬 데이터를 찾는다
    ///// </summary>
    //public record SkillCell
    //{
    //    public string Name;
    //    public SkillTypeSort thisSkillType;
    //    public int SkilliD;
    //}
    
    #endregion

}
