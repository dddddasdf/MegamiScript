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

public enum AilmentType
{
    Poison,     //독 - 전투에서 행동시 체력 감소, 필드에서 이동시 체력 감소/자동 회복X
    Sick,       //감기 - 전투에서 공격력 25% 하락, 회피율 0%로 하락, 턴 종료마다 5%의 확률로 타 아군 캐릭터에게 감염/자동 회복X
    Panic,      //혼란 - 조작 불가능, 마음대로 행동함 / 전투 중 및 전투 종료시 자동 회복
    Bind,       //결박 - 행동 불가능, 결박 상태의 악마에게 각종 대화시 플러스 효과 / 전투 중 및 전투 종료시 자동 회복
    Sleep,      //수면 - 행동 불가능, 피격시 25% 추가 대미지, 피격시 회복되지 않음 / 전투 중 및 전투 종료시 자동 회복
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

    private AilmentType? NowAilment;  //현재 걸려있는 상태이상-없을 경우 null, 상태이상은 중첩되지 않는다: 기존에 걸려있을 경우 새 상태이상이 덮어씌움

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
    public void RecoverHP(int Value)
    {
        NowHP += Value;
    }

    public void SetAil(AilmentType newAil)
    {
        NowAilment = newAil;
    }

    /// <summary>
    /// 걸려 있는 상태이상 제거
    /// </summary>
    public void DeleteAil()
    {
        NowAilment = null;
    }


    #region ReturnStatus
    
    public string ReturnName()
    {
        return Name;
    }


    /// <summary>
    /// 객체의 약점/내성 반환
    /// </summary>
    /// <param name="AttackedSkillType"></param>
    /// <returns></returns>
    public SkillAffinities? ReturnThisAff(SkillTypeSort AttackedSkillType)
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

    #endregion
}

public class PartyDemonData : PartyMemberData
{
    public int ID;          //악마 식별 번호
    public bool IsEntry;    //출전 중인지 확인용
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
        EntryDemonList[EntryIndex].IsEntry = false;
        EntryDemonList[EntryIndex] = PartyDemonList[PartyIndex];
    }

    /// <summary>
    /// 객체에게 회복 함수 호출
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="Value"></param>
    public void CallRecoverHP(PartyDemonData Target, int Value)
    {
        Target.RecoverHP(Value);
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
        return Target.ReturnThisAff(AttackedSkillType);
    }

    
}