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

public enum AilmentType
{
    Poison,     //�� - �������� �ൿ�� ü�� ����, �ʵ忡�� �̵��� ü�� ����/�ڵ� ȸ��X
    Sick,       //���� - �������� ���ݷ� 25% �϶�, ȸ���� 0%�� �϶�, �� ���Ḷ�� 5%�� Ȯ���� Ÿ �Ʊ� ĳ���Ϳ��� ����/�ڵ� ȸ��X
    Panic,      //ȥ�� - ���� �Ұ���, ������� �ൿ�� / ���� �� �� ���� ����� �ڵ� ȸ��
    Bind,       //��� - �ൿ �Ұ���, ��� ������ �Ǹ����� ���� ��ȭ�� �÷��� ȿ�� / ���� �� �� ���� ����� �ڵ� ȸ��
    Sleep,      //���� - �ൿ �Ұ���, �ǰݽ� 25% �߰� �����, �ǰݽ� ȸ������ ���� / ���� �� �� ���� ����� �ڵ� ȸ��
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

    private AilmentType? NowAilment;  //���� �ɷ��ִ� �����̻�-���� ��� null, �����̻��� ��ø���� �ʴ´�: ������ �ɷ����� ��� �� �����̻��� �����

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
    public void RecoverHP(int Value)
    {
        NowHP += Value;
    }

    public void SetAil(AilmentType newAil)
    {
        NowAilment = newAil;
    }

    /// <summary>
    /// �ɷ� �ִ� �����̻� ����
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
    /// ��ü�� ����/���� ��ȯ
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
    /// �Ϲ� ������ ��Ÿ������ ����
    /// </summary>
    /// <returns>��Ÿ���� ��� true</returns>
    public bool ReturnIsNormalGun()
    {
        return IsNormalGun;
    }

    #endregion
}

public class PartyDemonData : PartyMemberData
{
    public int ID;          //�Ǹ� �ĺ� ��ȣ
    public bool IsEntry;    //���� ������ Ȯ�ο�
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
        EntryDemonList[EntryIndex].IsEntry = false;
        EntryDemonList[EntryIndex] = PartyDemonList[PartyIndex];
    }

    /// <summary>
    /// ��ü���� ȸ�� �Լ� ȣ��
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="Value"></param>
    public void CallRecoverHP(PartyDemonData Target, int Value)
    {
        Target.RecoverHP(Value);
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
        return Target.ReturnThisAff(AttackedSkillType);
    }

    
}