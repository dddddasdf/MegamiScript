using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �Ӽ��� ���� ����~����
/// </summary>
public enum SkillAffinities
{
    Weak,   //���� �
    Resist, //���� ұ
    Void,   //��ȿ ��
    Reflect,    //�ݻ� ��
    Drain   //��� ��
}

[Flags]
public enum AilmentType
{
    None = 0   //�����̻� ����

    , Poison = 1 << 0    //�� - �������� �ൿ�� ü�� ����, �ʵ忡�� �̵��� ü�� ����/�ڵ� ȸ��X
    , Sick = 1 << 1      //���� - �������� ���ݷ� 25% �϶�, ȸ���� 0%�� �϶�, �� ���Ḷ�� 5%�� Ȯ���� Ÿ �Ʊ� ĳ���Ϳ��� ����/�ڵ� ȸ��X
    , Panic = 1 << 2     //ȥ�� - ���� �Ұ���, ������� �ൿ�� / ���� �� �� ���� ����� �ڵ� ȸ��
    , Bind = 1 << 3      //��� - �ൿ �Ұ���, ��� ������ �Ǹ����� ���� ��ȭ�� �÷��� ȿ�� / ���� �� �� ���� ����� �ڵ� ȸ��
    , Sleep = 1 << 4      //���� - �ൿ �Ұ���, �ǰݽ� 25% �߰� �����, �ǰݽ� ȸ������ ���� / ���� �� �� ���� ����� �ڵ� ȸ��

    , All = int.MaxValue    //ġ��� ����
}

public class PartyMemberData
{
    #region Field
    private string Name { get; init; }     //�̸�
    private int MaxHP;       //�ִ� ü��
    private int NowHP;       //���� ü��
    private int MaxMP;       //�ִ� ����
    private int NowMP;       //���� ����
    private bool IsDead;     //��� ����

    private int St;          //��-Į �⺻ ���� ���� ���, ���� ��ų ���� ���� ���
    private int Dx;          //��-�� �⺻ ���� ���� ���, ���� ��ų ���� ���� ���
    private int Ma;          //��-���� ��ų ���� ���
    private int Ag;          //��-������ Ȯ��, �ൿ����, ���߷�, ȸ���� ���
    private int Lu;          //��-ũ��Ƽ��, ȸ����, �����̻� ������, �����̻� ȸ���ӵ�, ������ ����� ���

    private AilmentType NowAilment;  //���� �ɷ��ִ� �����̻�

    private int BeginningLevel;     //�ʱ� ����
    private int Level;       //���� ����
    private int EXP;         //���� ����ġ

    public List<SkillDataRec> LearnedSkillList;     //������ ��ų ����Ʈ

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

    /// <summary>
    /// ü�� ���� �Լ�
    /// </summary>
    /// <param name="Value">����� ��ġ</param>
    public void LossHP(int Value)
    {
        NowHP -= Value;

        //���� ü���� 0 ���ϰ� �Ǹ� ��� ó��
        if (NowHP <= 0)
        {
            NowHP = 0;
            IsDead = true;
        }
    }

    /// <summary>
    /// ��Ȱ
    /// </summary>
    /// <param name="Value">��Ȱ�� ������ ��Ȱ�� ü��</param>
    public void Revival(int Value)
    {
        IsDead = false;
        NowHP = Value;
    }

    /// <summary>
    /// �� ��ų, ���� ������ ü�� ȸ��
    /// </summary>
    /// <param name="Value">ȸ�� ��ġ</param>
    public void RecoverHP(int? Value, out int RecovredHPValue)
    {
        if (Value == null)
        {
            //Ǯü�� ȸ������ ��� �ణ�� ��길 ��ġ�� �ٷ� ����
            RecovredHPValue = MaxHP - NowHP;
            NowHP = MaxHP;
        }
        
        int Tmp = (int)Value;   //����ȯ�� ����

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
    /// �� ��ų, ���� ������ ���� ȸ��
    /// </summary>
    /// <param name="Value">ȸ�� ��ġ</param>
    public void RecoverMP(int? Value, out int RecovredMPValue)
    {
        if (Value == null)
        {
            //Ǯ���� ȸ������ ��� �ణ�� ��길 ��ġ�� �ٷ� ����
            RecovredMPValue = MaxMP - NowMP;
            NowMP = MaxMP;
        }

        int Tmp = (int)Value;   //����ȯ�� ����

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
    /// �����̻� ����
    /// </summary>
    /// <param name="newAil">�߰��� �����̻�</param>
    public void SetAil(AilmentType newAil)
    {
        NowAilment |= newAil;
    }

    /// <summary>
    /// �ɷ� �ִ� �����̻� ����
    /// </summary>
    /// <param name="CureAilmentType">���� ��� �����̻�</param>
    public void CureAil(AilmentType CureAilmentType)
    {
        //���� ������ �ִ� �����̻��� ���� ��� �� �̻��� ó�� ���� ����
        if (NowAilment == AilmentType.None)
            return;             
        
        //���� ġ���Ⱑ ��� �����̻� ġ������ ��� ������ �ִ� �����̻��� ��������� ����
        if (CureAilmentType.HasFlag(AilmentType.All))
        {
            NowAilment = AilmentType.None;
            return;
        }

        /*
        HasFlag�� ���� ��� �����̻� ����Ʈ�� �ش� �����̻��� �����ϴ��� Ȯ���ϰ�, ���� ��� ĳ���Ͱ� �ش� �����̻��� ������ �ִ��� Ȯ�� �� �´ٸ� �ش� �����̻��� ���� 
        */
        //�� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Poison) && NowAilment.HasFlag(AilmentType.Poison))
        {
            NowAilment &= ~AilmentType.Poison;
        }

        //���� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Sick) && NowAilment.HasFlag(AilmentType.Sick))
        {
            NowAilment &= ~AilmentType.Sick;
        }

        //�ӹ� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Bind) && NowAilment.HasFlag(AilmentType.Bind))
        {
            NowAilment &= ~AilmentType.Bind;
        }

        //ȥ�� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Panic) && NowAilment.HasFlag(AilmentType.Panic))
        {
            NowAilment &= ~AilmentType.Panic;
        }

        //���� ġ��
        if (CureAilmentType.HasFlag(AilmentType.Sleep) && NowAilment.HasFlag(AilmentType.Sleep))
        {
            NowAilment &= ~AilmentType.Sleep;
        }
    }

    /// <summary>
    /// �޽� ��� �Ǵ� �������� �̿��ؼ� ��� ���� ȸ�� 
    /// </summary>
    public void RecoverAllByRest()
    {
        NowHP = MaxHP;
        NowMP = MaxMP;
        CureAil(AilmentType.All);
    }


    #region ReturnStatus

    /// <summary>
    /// �̸� ��ȯ
    /// </summary>
    /// <returns></returns>
    public string ReturnName()
    {
        return Name;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnSt()
    {
        return St;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnDx()
    {
        return Dx;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnMa()
    {
        return Ma;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnAg()
    {
        return Ag;
    }

    /// <summary>
    /// �� ���� ��ġ ��ȯ
    /// </summary>
    /// <returns>�� ����</returns>
    public int ReturnLu()
    {
        return Lu;
    }

    /// <summary>
    /// ��ü�� ����/���� ��ȯ
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
    /// �Ϲ� ������ ��Ÿ������ ����
    /// </summary>
    /// <returns>��Ÿ���� ��� true</returns>
    public bool ReturnIsNormalGun()
    {
        return IsNormalGun;
    }

    /// <summary>
    /// �Ϲ� ���� �ּ� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnNormalMinHit()
    {
        return NormalMinHit;
    }

    /// <summary>
    /// �Ϲ� ���� �ִ� Ƚ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnNormalMaxHit()
    {
        return NormalMaxHit;
    }

    /// <summary>
    /// ���⿡ �ɷȴ��� �ľǿ�: �ɷ����� ���� ������� �ɾ�� �ϹǷ�
    /// </summary>
    /// <returns></returns>
    public bool ReturnIsSick()
    {
        return (NowAilment.HasFlag(AilmentType.Sick));
    }

    /// <summary>
    /// �ʱ� ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnBeginningLv()
    {
        return BeginningLevel;
    }

    /// <summary>
    /// ������ ��ų ���� ��ȯ
    /// </summary>
    /// <returns>������ ��ų ����</returns>
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
    private int ID;          //�Ǹ� �ĺ� ��ȣ
    private bool IsEntry;    //���� ������ Ȯ�ο�

    

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
    private List<PartyDemonData> PartyDemonList;        //��ü ���� �Ǹ� ����Ʈ
    private List<PartyDemonData> EntryDemonList;        //���� ���� �Ǹ� ����Ʈ(�� ��)

    /// <summary>
    /// ���� ���� �� ���� ���忡 �ִ� ���� �Ǹ� ����Ʈ �ʱ�ȭ
    /// </summary>
    private void InitPartyDemonList()
    {

    }

    /// <summary>
    /// ���� ���� �� ���� ���� ���� ���� �Ǹ� ����Ʈ �ʱ�ȭ
    /// </summary>
    private void InitEntryDemonList()
    {

    }

    /// <summary>
    /// ���� ���� �Ǹ� ��ü
    /// </summary>
    /// <param name="PartyIndex"></param>
    /// <param name="EntryIndex"></param>
    public void SwapEntryDemon(int PartyIndex, int EntryIndex)
    {
        EntryDemonList[EntryIndex].SwapEntry(false);
        EntryDemonList[EntryIndex] = PartyDemonList[PartyIndex];
    }

    /// <summary>
    /// ��ü���� ȸ�� �Լ� ȣ��
    /// </summary>
    /// <param name="Target">���� ���</param>
    /// <param name="Value">ȸ�� ��ġ</param>
    /// <param name="RecovredHPValue">���� ȸ���� ��ġ</param>
    public void CallRecoverHP(PartyDemonData Target, int? Value, out int RecovredHPValue)
    {
        Target.RecoverHP(Value, out RecovredHPValue);
    }

    /// <summary>
    /// �̸��� ��ȯ�϶�� ��ü���� ����
    /// </summary>
    /// <param name="Target">���</param>
    /// <returns>����� �̸�</returns>
    public string CallReturnName(PartyDemonData Target)
    {
        return Target.ReturnName();
    }

    /// <summary>
    /// ���ݽ� ����/���� üũ�϶�� ��ü���� ����
    /// </summary>
    /// <param name="Target">������ �޴� ��Ƽ��</param>
    /// <param name="AttackedSkillType">���� ��ų�� Ÿ��</param>
    /// <returns></returns>
    public SkillAffinities? CallReturnAffinity(PartyDemonData Target, SkillTypeSort AttackedSkillType)
    {
        return Target.ReturnMatchingAffinity(AttackedSkillType);
    }

    
}