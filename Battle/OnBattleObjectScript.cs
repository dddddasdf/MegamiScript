using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투에 참전 중인 파티 객체
/// </summary>
public class OnBattlePartyObject
{
    private PartyMemberData thisMemberData;     //얕은 복사로 받아오는 원본 멤버 데이터
    private bool IsPlayerCharacter;

    public void SetMemberData(PartyMemberData MemberData)
    {
        thisMemberData = MemberData;
    }

    public PartyMemberData ReturnMemberData()
    {
        return thisMemberData;
    }

    public bool ReturnIsPlayerCharacter()
    {
        return IsPlayerCharacter;
    }

    public void SetIsPlayerCharacter(bool Value)
    {
        IsPlayerCharacter = Value;
    }


    #region ManagingBuffDebuff
    /*
    능력치 강화 버프/디버프는 3회 중첩 가능
    ->버프 요청이 들어오면 적용 횟수가 3회 미만인지 확인하고 미만일 경우 적용하게끔
    */
    private int NumberOfAttackBuff = 0;     //공격력 증가 버프 횟수
    private int NumberOfDefenseBuff = 0;    //방어력 증가 버프 횟수
    private int NumberOfAgilityBuff = 0;    //민첩성(명중, 회피) 증가 버프 횟수

    private int NumberOfAttackDebuff = 0;   //공격력 감소 디버프 횟수
    private int NumberOfDefenseDebuff = 0;  //방어력 감소 디버프 횟수
    private int NumberOfAgilityDebuff = 0;  //민첩성(명중, 회피) 감소 디버프 횟수

    private bool IsPhysicEnhanced = false;   //차지 사용 스위치용, 버프를 걸어 적용되면 true로 변경하고 다음 턴 종료시 false로 변환한다
    private bool IsMagicEnhanced = false;   //컨센트레이트 사용 스위치용, 버프를 걸어 적용되면 true로 변경하고 다음 턴 종료시 false로 변환한다


    /// <summary>
    /// 버프 관련 변수 초기화
    /// </summary>
    public void InitBuffField()
    {
        NumberOfAttackBuff = 0;
        NumberOfDefenseBuff = 0;
        NumberOfAgilityBuff = 0;

        NumberOfAttackDebuff = 0;
        NumberOfDefenseDebuff = 0;
        NumberOfAgilityDebuff = 0;

        IsPhysicEnhanced = true;
        IsMagicEnhanced = true;
    }



    /// <summary>
    /// 공격력 증가 부분
    /// </summary>
    public void IncreaseAttack()
    {
        if (NumberOfAttackBuff < 3)
            NumberOfAttackBuff++;
    }

    /// <summary>
    /// 방어력 증가 부분
    /// </summary>
    public void IncreaseDefense()
    {
        if (NumberOfDefenseBuff < 3)
            NumberOfDefenseBuff++;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 증가 부분
    /// </summary>
    public void IncreaseAgility()
    {
        if (NumberOfAgilityBuff < 3)
            NumberOfAgilityBuff++;

        //여기는 별도로 명중률과 회피율 증가도 구현해야 한다
    }

    /// <summary>
    /// 모든 버프 효과 제거
    /// </summary>
    public void ResetBuff()
    {
        NumberOfAttackBuff = 0;
        NumberOfDefenseBuff = 0;
        NumberOfAgilityBuff = 0;
        //명중률과 회피는 감소 조치도 있게끔
        //그런데 이거 생각해보니 스킬 쓸 때랑 맞을 때 버프 횟수에 따라 추가 보정하도록 하는 게 나을 듯 (즉 명중, 회피 저장 변수값 자체는 변동 X)
    }

    /// <summary>
    /// 공격력 감소 부분
    /// </summary>
    public void DecreaseAttack()
    {
        if (NumberOfAttackDebuff < 3)
            NumberOfAttackDebuff++;
    }

    /// <summary>
    /// 방어력 감소 부분
    /// </summary>
    public void DecreaseDefense()
    {
        if (NumberOfDefenseDebuff < 3)
            NumberOfDefenseDebuff++;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 감소 부분
    /// </summary>
    public void DecreaseAgility()
    {
        if (NumberOfAgilityDebuff < 3)
            NumberOfAgilityDebuff++;

        //여기는 별도로 명중률과 회피율 증가도 구현해야 한다
    }

    /// <summary>
    /// 모든 디버프 효과 제거
    /// </summary>
    public void ResetDebuff()
    {
        NumberOfAttackDebuff = 0;
        NumberOfDefenseDebuff = 0;
        NumberOfAgilityDebuff = 0;
        //명중률과 회피는 감소 조치도 있게끔
        //그런데 이거 생각해보니 스킬 쓸 때랑 맞을 때 버프 횟수에 따라 추가 보정하도록 하는 게 나을 듯 (즉 명중, 회피 저장 변수값 자체는 변동 X)
    }

    /// <summary>
    /// 공격력 증가 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAttack()
    {
        return NumberOfAttackBuff;
    }

    /// <summary>
    /// 방어력 증가 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedDefense()
    {
        return NumberOfDefenseBuff;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 증가 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAgility()
    {
        return NumberOfAgilityBuff;
    }

    /// <summary>
    /// 공격력 감소 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAttack()
    {
        return NumberOfAttackDebuff;
    }

    /// <summary>
    /// 방어력 감소 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedDefense()
    {
        return NumberOfDefenseDebuff;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 감소 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAgility()
    {
        return NumberOfAgilityDebuff;
    }

    /// <summary>
    /// 차지 사용시
    /// </summary>
    public void SwitchPhysicEnhancing()
    {
        IsPhysicEnhanced = true;
    }

    /// <summary>
    /// 컨센트레이트 사용시
    /// </summary>
    public void SwitchMagicEnhancing()
    {
        IsMagicEnhanced = true;
    }

    /// <summary>
    /// 물리 공격 강화 여부
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsPhysicEnhanced()
    {
        return IsPhysicEnhanced;
    }

    /// <summary>
    /// 마법 공격 강화 여부
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsMagicEnhanced()
    {
        return IsMagicEnhanced;
    }

    #endregion
}


/// <summary>
/// 전투에 참전 중인 적 개체
/// </summary>
public class OnBattleEnemyObject
{
    public OnBattleEnemyObject(EnemyDataRec CopyEnemyData)
    {
        this.ID = CopyEnemyData.ReturnID();
        this.Name = CopyEnemyData.ReturnName();
        this.Tribe = CopyEnemyData.ReturnTrbie();
        this.Level = CopyEnemyData.ReturnLevel();
        this.HP = CopyEnemyData.ReturnHP();
        this.MP = CopyEnemyData.ReturnMP();
        this.St = CopyEnemyData.ReturnSt();
        this.Dx = CopyEnemyData.ReturnDx();
        this.Ma = CopyEnemyData.ReturnMa();
        this.Ag = CopyEnemyData.ReturnAg();
        this.Lu = CopyEnemyData.ReturnLu();
        this.AffinityPhysical = CopyEnemyData.ReturnAffinity(SkillTypeSort.Physical);
        this.AffinityGun = CopyEnemyData.ReturnAffinity(SkillTypeSort.Gun);
        this.AffinityFire = CopyEnemyData.ReturnAffinity(SkillTypeSort.Fire);
        this.AffinityIce = CopyEnemyData.ReturnAffinity(SkillTypeSort.Ice);
        this.AffinityElectric = CopyEnemyData.ReturnAffinity(SkillTypeSort.Electric);
        this.AffinityForce = CopyEnemyData.ReturnAffinity(SkillTypeSort.Force);
        this.AffinityLight = CopyEnemyData.ReturnAffinity(SkillTypeSort.Light);
        this.AffinityDark = CopyEnemyData.ReturnAffinity(SkillTypeSort.Dark);

        int? t;
        for (int i = 1; i <= 7; i++)
        {
            t = CopyEnemyData.ReturnLearnSkillLevel(i);

            if (t != 0)
                break;      //스킬의 배우는 레벨이 0이 아닐 경우(다른 정숫값 또는 null) 기본 습득스킬은 여기까지이므로 배열에 넣을 필요 없다

            ArrayOfLearnedSkiLLID.Add(CopyEnemyData.ReturnLearnSkiLLID(i));
        }

        this.IsNormalGun = CopyEnemyData.ReturnIsNormalGun();
        this.NormalMinHit = CopyEnemyData.ReturnNormalMinHit();
        this.NormalMaxHit = CopyEnemyData.ReturnNormalMaxHit();
    }

    
    //가변 변수
    #region VariableField
    private int HP;
    private int MP;

    /// <summary>
    /// 악마가 자동으로 습득해둔 스킬 ID 리스트
    /// </summary>
    private List<int> ArrayOfLearnedSkiLLID = new List<int>();

    #endregion


    //불변 변수
    #region InvariableField

    private int ID { get; init; }
    private string Name { get; init; }
    private string Tribe { get; init; }
    private int Level { get; init; }
    private int St { get; init; }
    private int Dx { get; init; }
    private int Ma { get; init; }
    private int Ag { get; init; }
    private int Lu { get; init; }
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


    public int ReturnID()
    {
        return ID;
    }

    public string ReturnName()
    {
        return Name;
    }

    public int ReturnHP()
    {
        return HP;
    }

    public string ReturnTribe()
    {
        return Tribe;
    }

    public int ReturnLevel()
    {
        return Level;
    }

    /// <summary>
    /// 공격 받은/받을 스킬 속성에 대한 약점~강점 반환
    /// </summary>
    /// <param name="AttackSkill"></param>
    /// <returns></returns>
    public SkillAffinities? ReturnAffinity(SkillTypeSort AttackSkill)
    {
        switch (AttackSkill)
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


    #region ManagingBuffDebuff
    /*
    능력치 강화 버프/디버프는 3회 중첩 가능
    ->버프 요청이 들어오면 적용 횟수가 3회 미만인지 확인하고 미만일 경우 적용하게끔
    */
    private int NumberOfAttackBuff = 0;     //공격력 증가 버프 횟수
    private int NumberOfDefenseBuff = 0;    //방어력 증가 버프 횟수
    private int NumberOfAgilityBuff = 0;    //민첩성(명중, 회피) 증가 버프 횟수

    private int NumberOfAttackDebuff = 0;   //공격력 감소 디버프 횟수
    private int NumberOfDefenseDebuff = 0;  //방어력 감소 디버프 횟수
    private int NumberOfAgilityDebuff = 0;  //민첩성(명중, 회피) 감소 디버프 횟수

    private bool IsPhysicEnhanced = false;   //차지 사용 스위치용, 버프를 걸어 적용되면 true로 변경하고 다음 턴 종료시 false로 변환한다
    private bool IsMagicEnhanced = false;   //컨센트레이트 사용 스위치용, 버프를 걸어 적용되면 true로 변경하고 다음 턴 종료시 false로 변환한다


    /// <summary>
    /// 버프 관련 변수 초기화
    /// </summary>
    public void InitBuffField()
    {
        NumberOfAttackBuff = 0;
        NumberOfDefenseBuff = 0;
        NumberOfAgilityBuff = 0;

        NumberOfAttackDebuff = 0;
        NumberOfDefenseDebuff = 0;
        NumberOfAgilityDebuff = 0;

        IsPhysicEnhanced = true;
        IsMagicEnhanced = true;
    }



    /// <summary>
    /// 공격력 증가 부분
    /// </summary>
    public void IncreaseAttack()
    {
        if (NumberOfAttackBuff < 3)
            NumberOfAttackBuff++;
    }

    /// <summary>
    /// 방어력 증가 부분
    /// </summary>
    public void IncreaseDefense()
    {
        if (NumberOfDefenseBuff < 3)
            NumberOfDefenseBuff++;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 증가 부분
    /// </summary>
    public void IncreaseAgility()
    {
        if (NumberOfAgilityBuff < 3)
            NumberOfAgilityBuff++;

        //여기는 별도로 명중률과 회피율 증가도 구현해야 한다
    }

    /// <summary>
    /// 모든 버프 효과 제거
    /// </summary>
    public void ResetBuff()
    {
        NumberOfAttackBuff = 0;
        NumberOfDefenseBuff = 0;
        NumberOfAgilityBuff = 0;
        //명중률과 회피는 감소 조치도 있게끔
        //그런데 이거 생각해보니 스킬 쓸 때랑 맞을 때 버프 횟수에 따라 추가 보정하도록 하는 게 나을 듯 (즉 명중, 회피 저장 변수값 자체는 변동 X)
    }

    /// <summary>
    /// 공격력 감소 부분
    /// </summary>
    public void DecreaseAttack()
    {
        if (NumberOfAttackDebuff < 3)
            NumberOfAttackDebuff++;
    }

    /// <summary>
    /// 방어력 감소 부분
    /// </summary>
    public void DecreaseDefense()
    {
        if (NumberOfDefenseDebuff < 3)
            NumberOfDefenseDebuff++;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 감소 부분
    /// </summary>
    public void DecreaseAgility()
    {
        if (NumberOfAgilityDebuff < 3)
            NumberOfAgilityDebuff++;

        //여기는 별도로 명중률과 회피율 증가도 구현해야 한다
    }

    /// <summary>
    /// 모든 디버프 효과 제거
    /// </summary>
    public void ResetDebuff()
    {
        NumberOfAttackDebuff = 0;
        NumberOfDefenseDebuff = 0;
        NumberOfAgilityDebuff = 0;
        //명중률과 회피는 감소 조치도 있게끔
        //그런데 이거 생각해보니 스킬 쓸 때랑 맞을 때 버프 횟수에 따라 추가 보정하도록 하는 게 나을 듯 (즉 명중, 회피 저장 변수값 자체는 변동 X)
    }

    /// <summary>
    /// 공격력 증가 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAttack()
    {
        return NumberOfAttackBuff;
    }

    /// <summary>
    /// 방어력 증가 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedDefense()
    {
        return NumberOfDefenseBuff;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 증가 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAgility()
    {
        return NumberOfAgilityBuff;
    }

    /// <summary>
    /// 공격력 감소 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAttack()
    {
        return NumberOfAttackDebuff;
    }

    /// <summary>
    /// 방어력 감소 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedDefense()
    {
        return NumberOfDefenseDebuff;
    }

    /// <summary>
    /// 민첩성(명중, 회피) 감소 횟수 반환
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAgility()
    {
        return NumberOfAgilityDebuff;
    }

    /// <summary>
    /// 차지 사용시
    /// </summary>
    public void SwitchPhysicEnhancing()
    {
        IsPhysicEnhanced = true;
    }

    /// <summary>
    /// 컨센트레이트 사용시
    /// </summary>
    public void SwitchMagicEnhancing()
    {
        IsMagicEnhanced = true;
    }

    /// <summary>
    /// 물리 공격 강화 여부
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsPhysicEnhanced()
    {
        return IsPhysicEnhanced;
    }

    /// <summary>
    /// 마법 공격 강화 여부
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsMagicEnhanced()
    {
        return IsMagicEnhanced;
    }

    #endregion
}