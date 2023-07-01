using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���� ���� ��Ƽ ��ü
/// </summary>
public class OnBattlePartyObject
{
    private PartyMemberData thisMemberData;     //���� ����� �޾ƿ��� ���� ��� ������
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
    �ɷ�ġ ��ȭ ����/������� 3ȸ ��ø ����
    ->���� ��û�� ������ ���� Ƚ���� 3ȸ �̸����� Ȯ���ϰ� �̸��� ��� �����ϰԲ�
    */
    private int NumberOfAttackBuff = 0;     //���ݷ� ���� ���� Ƚ��
    private int NumberOfDefenseBuff = 0;    //���� ���� ���� Ƚ��
    private int NumberOfAgilityBuff = 0;    //��ø��(����, ȸ��) ���� ���� Ƚ��

    private int NumberOfAttackDebuff = 0;   //���ݷ� ���� ����� Ƚ��
    private int NumberOfDefenseDebuff = 0;  //���� ���� ����� Ƚ��
    private int NumberOfAgilityDebuff = 0;  //��ø��(����, ȸ��) ���� ����� Ƚ��

    private bool IsPhysicEnhanced = false;   //���� ��� ����ġ��, ������ �ɾ� ����Ǹ� true�� �����ϰ� ���� �� ����� false�� ��ȯ�Ѵ�
    private bool IsMagicEnhanced = false;   //����Ʈ����Ʈ ��� ����ġ��, ������ �ɾ� ����Ǹ� true�� �����ϰ� ���� �� ����� false�� ��ȯ�Ѵ�


    /// <summary>
    /// ���� ���� ���� �ʱ�ȭ
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
    /// ���ݷ� ���� �κ�
    /// </summary>
    public void IncreaseAttack()
    {
        if (NumberOfAttackBuff < 3)
            NumberOfAttackBuff++;
    }

    /// <summary>
    /// ���� ���� �κ�
    /// </summary>
    public void IncreaseDefense()
    {
        if (NumberOfDefenseBuff < 3)
            NumberOfDefenseBuff++;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� �κ�
    /// </summary>
    public void IncreaseAgility()
    {
        if (NumberOfAgilityBuff < 3)
            NumberOfAgilityBuff++;

        //����� ������ ���߷��� ȸ���� ������ �����ؾ� �Ѵ�
    }

    /// <summary>
    /// ��� ���� ȿ�� ����
    /// </summary>
    public void ResetBuff()
    {
        NumberOfAttackBuff = 0;
        NumberOfDefenseBuff = 0;
        NumberOfAgilityBuff = 0;
        //���߷��� ȸ�Ǵ� ���� ��ġ�� �ְԲ�
        //�׷��� �̰� �����غ��� ��ų �� ���� ���� �� ���� Ƚ���� ���� �߰� �����ϵ��� �ϴ� �� ���� �� (�� ����, ȸ�� ���� ������ ��ü�� ���� X)
    }

    /// <summary>
    /// ���ݷ� ���� �κ�
    /// </summary>
    public void DecreaseAttack()
    {
        if (NumberOfAttackDebuff < 3)
            NumberOfAttackDebuff++;
    }

    /// <summary>
    /// ���� ���� �κ�
    /// </summary>
    public void DecreaseDefense()
    {
        if (NumberOfDefenseDebuff < 3)
            NumberOfDefenseDebuff++;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� �κ�
    /// </summary>
    public void DecreaseAgility()
    {
        if (NumberOfAgilityDebuff < 3)
            NumberOfAgilityDebuff++;

        //����� ������ ���߷��� ȸ���� ������ �����ؾ� �Ѵ�
    }

    /// <summary>
    /// ��� ����� ȿ�� ����
    /// </summary>
    public void ResetDebuff()
    {
        NumberOfAttackDebuff = 0;
        NumberOfDefenseDebuff = 0;
        NumberOfAgilityDebuff = 0;
        //���߷��� ȸ�Ǵ� ���� ��ġ�� �ְԲ�
        //�׷��� �̰� �����غ��� ��ų �� ���� ���� �� ���� Ƚ���� ���� �߰� �����ϵ��� �ϴ� �� ���� �� (�� ����, ȸ�� ���� ������ ��ü�� ���� X)
    }

    /// <summary>
    /// ���ݷ� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAttack()
    {
        return NumberOfAttackBuff;
    }

    /// <summary>
    /// ���� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedDefense()
    {
        return NumberOfDefenseBuff;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAgility()
    {
        return NumberOfAgilityBuff;
    }

    /// <summary>
    /// ���ݷ� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAttack()
    {
        return NumberOfAttackDebuff;
    }

    /// <summary>
    /// ���� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedDefense()
    {
        return NumberOfDefenseDebuff;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAgility()
    {
        return NumberOfAgilityDebuff;
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void SwitchPhysicEnhancing()
    {
        IsPhysicEnhanced = true;
    }

    /// <summary>
    /// ����Ʈ����Ʈ ����
    /// </summary>
    public void SwitchMagicEnhancing()
    {
        IsMagicEnhanced = true;
    }

    /// <summary>
    /// ���� ���� ��ȭ ����
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsPhysicEnhanced()
    {
        return IsPhysicEnhanced;
    }

    /// <summary>
    /// ���� ���� ��ȭ ����
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsMagicEnhanced()
    {
        return IsMagicEnhanced;
    }

    #endregion
}


/// <summary>
/// ������ ���� ���� �� ��ü
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
                break;      //��ų�� ���� ������ 0�� �ƴ� ���(�ٸ� ������ �Ǵ� null) �⺻ ���潺ų�� ��������̹Ƿ� �迭�� ���� �ʿ� ����

            ArrayOfLearnedSkiLLID.Add(CopyEnemyData.ReturnLearnSkiLLID(i));
        }

        this.IsNormalGun = CopyEnemyData.ReturnIsNormalGun();
        this.NormalMinHit = CopyEnemyData.ReturnNormalMinHit();
        this.NormalMaxHit = CopyEnemyData.ReturnNormalMaxHit();
    }

    
    //���� ����
    #region VariableField
    private int HP;
    private int MP;

    /// <summary>
    /// �Ǹ��� �ڵ����� �����ص� ��ų ID ����Ʈ
    /// </summary>
    private List<int> ArrayOfLearnedSkiLLID = new List<int>();

    #endregion


    //�Һ� ����
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
    private bool IsNormalGun { get; init; }        //�Ϲ� ������ �ѼӼ��� ��� true, �� �ܿ��� false
    private int NormalMinHit { get; init; }        //�Ϲ� ������ �ּ� Ÿ��: �Ϻ� �Ǹ��� �����ϸ� ��������δ� 1ȸ
    private int NormalMaxHit { get; init; }        //�Ϲ� ������ �ִ� Ÿ��: �Ϻ� �Ǹ��� �����ϸ� ��������δ� 1ȸ

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
    /// ���� ����/���� ��ų �Ӽ��� ���� ����~���� ��ȯ
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
    �ɷ�ġ ��ȭ ����/������� 3ȸ ��ø ����
    ->���� ��û�� ������ ���� Ƚ���� 3ȸ �̸����� Ȯ���ϰ� �̸��� ��� �����ϰԲ�
    */
    private int NumberOfAttackBuff = 0;     //���ݷ� ���� ���� Ƚ��
    private int NumberOfDefenseBuff = 0;    //���� ���� ���� Ƚ��
    private int NumberOfAgilityBuff = 0;    //��ø��(����, ȸ��) ���� ���� Ƚ��

    private int NumberOfAttackDebuff = 0;   //���ݷ� ���� ����� Ƚ��
    private int NumberOfDefenseDebuff = 0;  //���� ���� ����� Ƚ��
    private int NumberOfAgilityDebuff = 0;  //��ø��(����, ȸ��) ���� ����� Ƚ��

    private bool IsPhysicEnhanced = false;   //���� ��� ����ġ��, ������ �ɾ� ����Ǹ� true�� �����ϰ� ���� �� ����� false�� ��ȯ�Ѵ�
    private bool IsMagicEnhanced = false;   //����Ʈ����Ʈ ��� ����ġ��, ������ �ɾ� ����Ǹ� true�� �����ϰ� ���� �� ����� false�� ��ȯ�Ѵ�


    /// <summary>
    /// ���� ���� ���� �ʱ�ȭ
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
    /// ���ݷ� ���� �κ�
    /// </summary>
    public void IncreaseAttack()
    {
        if (NumberOfAttackBuff < 3)
            NumberOfAttackBuff++;
    }

    /// <summary>
    /// ���� ���� �κ�
    /// </summary>
    public void IncreaseDefense()
    {
        if (NumberOfDefenseBuff < 3)
            NumberOfDefenseBuff++;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� �κ�
    /// </summary>
    public void IncreaseAgility()
    {
        if (NumberOfAgilityBuff < 3)
            NumberOfAgilityBuff++;

        //����� ������ ���߷��� ȸ���� ������ �����ؾ� �Ѵ�
    }

    /// <summary>
    /// ��� ���� ȿ�� ����
    /// </summary>
    public void ResetBuff()
    {
        NumberOfAttackBuff = 0;
        NumberOfDefenseBuff = 0;
        NumberOfAgilityBuff = 0;
        //���߷��� ȸ�Ǵ� ���� ��ġ�� �ְԲ�
        //�׷��� �̰� �����غ��� ��ų �� ���� ���� �� ���� Ƚ���� ���� �߰� �����ϵ��� �ϴ� �� ���� �� (�� ����, ȸ�� ���� ������ ��ü�� ���� X)
    }

    /// <summary>
    /// ���ݷ� ���� �κ�
    /// </summary>
    public void DecreaseAttack()
    {
        if (NumberOfAttackDebuff < 3)
            NumberOfAttackDebuff++;
    }

    /// <summary>
    /// ���� ���� �κ�
    /// </summary>
    public void DecreaseDefense()
    {
        if (NumberOfDefenseDebuff < 3)
            NumberOfDefenseDebuff++;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� �κ�
    /// </summary>
    public void DecreaseAgility()
    {
        if (NumberOfAgilityDebuff < 3)
            NumberOfAgilityDebuff++;

        //����� ������ ���߷��� ȸ���� ������ �����ؾ� �Ѵ�
    }

    /// <summary>
    /// ��� ����� ȿ�� ����
    /// </summary>
    public void ResetDebuff()
    {
        NumberOfAttackDebuff = 0;
        NumberOfDefenseDebuff = 0;
        NumberOfAgilityDebuff = 0;
        //���߷��� ȸ�Ǵ� ���� ��ġ�� �ְԲ�
        //�׷��� �̰� �����غ��� ��ų �� ���� ���� �� ���� Ƚ���� ���� �߰� �����ϵ��� �ϴ� �� ���� �� (�� ����, ȸ�� ���� ������ ��ü�� ���� X)
    }

    /// <summary>
    /// ���ݷ� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAttack()
    {
        return NumberOfAttackBuff;
    }

    /// <summary>
    /// ���� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedDefense()
    {
        return NumberOfDefenseBuff;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnIncreasedAgility()
    {
        return NumberOfAgilityBuff;
    }

    /// <summary>
    /// ���ݷ� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAttack()
    {
        return NumberOfAttackDebuff;
    }

    /// <summary>
    /// ���� ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedDefense()
    {
        return NumberOfDefenseDebuff;
    }

    /// <summary>
    /// ��ø��(����, ȸ��) ���� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnDecreasedAgility()
    {
        return NumberOfAgilityDebuff;
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void SwitchPhysicEnhancing()
    {
        IsPhysicEnhanced = true;
    }

    /// <summary>
    /// ����Ʈ����Ʈ ����
    /// </summary>
    public void SwitchMagicEnhancing()
    {
        IsMagicEnhanced = true;
    }

    /// <summary>
    /// ���� ���� ��ȭ ����
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsPhysicEnhanced()
    {
        return IsPhysicEnhanced;
    }

    /// <summary>
    /// ���� ���� ��ȭ ����
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsMagicEnhanced()
    {
        return IsMagicEnhanced;
    }

    #endregion
}