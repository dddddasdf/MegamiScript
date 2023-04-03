using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���� ���� ��ü
/// </summary>
public class OnBattleObject : PartyMemberData
{  
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


public class OnBattleParty
{
 
}
