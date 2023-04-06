using System;
using System.Collections;
using System.Collections.Generic;

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

public class PartyMemberData
{
    #region Field
    private string Name { get; init; }     //이름
    private int MaxHP;       //최대 체력
    private int NowHP;       //현재 체력
    private int MaxMP;       //최대 마나
    private int NowMP;       //현재 마나
    private bool IsDead;     //사망 판정

    private int St;          //힘-칼 기본 공격 위력 상승, 물리 스킬 위력 소폭 상승
    private int Dx;          //기-총 기본 공격 위력 상승, 물리 스킬 위력 대폭 상승
    private int Ma;          //마-마법 스킬 위력 상승
    private int Ag;          //속-선제턴 확률, 행동순서, 명중률, 회피율 상승
    private int Lu;          //운-크리티컬, 회피율, 상태이상 성공률, 상태이상 회복속도, 아이템 드랍률 상승

    private AilmentType NowAilment;  //현재 걸려있는 상태이상

    private int BeginningLevel;     //초기 레벨
    private int Level;       //현재 레벨
    private int EXP;         //현재 경험치

    public List<SkillDataRec> LearnedSkillList;     //습득한 스킬 리스트

    private SkillAffinities? AffinityPhysical { get; init; }
    private SkillAffinities? AffinityGun { get; init; }
    private SkillAffinities? AffinityFire { get; init; }
    private SkillAffinities? AffinityIce { get; init; }
    private SkillAffinities? AffinityElectric { get; init; }
    private SkillAffinities? AffinityForce { get; init; }
    private SkillAffinities? AffinityLight { get; init; }
    private SkillAffinities? AffinityDark { get; init; }

    private bool IsNormalGun { get; init; }        //일반 공격이 총속성일 경우 true, 그 외에는 false
    private int NormalMinHit { get; init; }        //일반 공격의 최소 타수: 일부 악마를 제외하면 통상적으로는 1회
    private int NormalMaxHit { get; init; }        //일반 공격의 최대 타수: 일부 악마를 제외하면 통상적으로는 1회


    #endregion

    /// <summary>
    /// 체력 감소 함수
    /// </summary>
    /// <param name="Value">대미지 수치</param>
    public void LossHP(int Value)
    {
        NowHP -= Value;

        //현재 체력이 0 이하가 되면 사망 처리
        if (NowHP <= 0)
        {
            NowHP = 0;
            IsDead = true;
        }
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


    #endregion
}

public class PartyDemonData : PartyMemberData
{
    private int ID;          //악마 식별 번호
    private bool IsEntry;    //출전 중인지 확인용

    

    public int ReturnID()
    {
        return ID;
    }

    public bool ReturnIsEntry()
    {
        return IsEntry;
    }

    public void SwapEntry(bool Value)
    {
        IsEntry = Value;
    }
}

public class PlayerCharacterData : PartyMemberData
{

}

public class PartyMemberManager
{
    private List<PartyDemonData> PartyDemonList;        //전체 동료 악마 리스트
    private List<PartyDemonData> EntryDemonList;        //출전 동료 악마 리스트(총 셋)

    /// <summary>
    /// 게임 구동 후 현재 스톡에 있는 동료 악마 리스트 초기화
    /// </summary>
    private void InitPartyDemonList()
    {

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
        EntryDemonList[EntryIndex].SwapEntry(false);
        EntryDemonList[EntryIndex] = PartyDemonList[PartyIndex];
    }

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