using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem
{
    private float IncreaseValueOfAttack = 0.2f;        //���ݷ� ���� ������ ���� �ö󰡴� ����� �������� ��ġ�� 20%
    private float DecreaseValueOfDefense = 0.2f;        //���ݷ� ���� ������ ���� �ö󰡴� ����� �������� ��ġ�� 20%
    private float DebuffAttackValueOfSick = 0.75f;      //���⿡ ���� ���̴� ���ݷ��� 25%
    private float IncreaseValueOfEnhance = 1.5f;        //���� or ����Ʈ����Ʈ�� �ö󰡴� ����� �������� ��ġ�� 250%

    /// <summary>
    /// ���ΰ��� �Ϲ� ���� ����� ���
    /// </summary>
    /// <param name="User"></param>
    /// <param name="Target"></param>
    /// <param name="NormalAttackDamage"></param>
    /// <param name="IsSwordAttack">���ΰ��� ��� ���ݰ� "��"��� �������� ������</param>
    public void CalculatePlayerNormalAttackDamage(OnBattlePartyObject User, OnBattlePartyObject Target, bool IsSwordAttack,out int NormalAttackDamage)
    {
        /*
        �ּ� ������=(���� ����+��)/3(�Ҽ� �� ���� �ݿø�)
         */
        float WeaponPower = 1f;          //���ΰ��� ��쿡�� ���� ������ �޾ƿ���, ���� �Ǹ��� ���� ������ ��ģ��


        //���ΰ� ���� ������ �޾ƿ��� �۾��� �ʿ��ϴ�

        float RoughDamage;
        int RandomNumber = Random.Range(0, 16);     //���� 0~15

        if (IsSwordAttack)
        {
            //��� ����
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnDx()) * 0.33f + RandomNumber;
        }
        else
        {
            //�� ��� ����
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnSt()) * 0.33f + RandomNumber;
        }

        NormalAttackDamage = (int)RoughDamage;
    }

    /// <summary>
    /// �Ǹ��� �Ϲ� ���� ����� ���
    /// </summary>
    /// <param name="User"></param>
    /// <param name="Target"></param>
    /// <param name="NormalAttackDamage"></param>
    public void CalculateDemonNormalAttackDamage(OnBattlePartyObject User, OnBattlePartyObject Target, out int NormalAttackDamage)
    {
        /*
        �ּ� ������=(���� ����+��)/3(�Ҽ� �� ���� �ݿø�)
        ���� ������ ��� ��Ʈ������ ������ �������� �Ǹ��� �ʱ� ������ Ȱ���� ��
         */
        float WeaponPower = 1f;          //���ΰ��� ��쿡�� ���� ������ �޾ƿ���, ���� �Ǹ��� ���� ������ ��ģ��

        int MinHit = User.ReturnMemberData().ReturnNormalMinHit();
        int MaxHit = User.ReturnMemberData().ReturnNormalMaxHit();
        float AverageHit = (MinHit + MaxHit) * 0.5f;

        switch (AverageHit)
        {
            case 1f:
                WeaponPower = 4.3f *User.ReturnMemberData().ReturnBeginningLv() + 49.6f;
                break;
            case 1.5f:
                //�������� 1~2ȸ
                WeaponPower = 3f * User.ReturnMemberData().ReturnBeginningLv() + 35f;
                break;
            case 2f:
                //��� ������ 2ȸ or 1~3ȸ
                WeaponPower = 2.2f * User.ReturnMemberData().ReturnBeginningLv() + 26.8f;
                break;
            case 2.5f:
                //��� ������ 2~3ȸ
                WeaponPower = 1.5f * User.ReturnMemberData().ReturnBeginningLv() + 42.5f;
                break;
        }

        float RoughDamage;
        int RandomNumber = Random.Range(0, 16);     //���� 0~15
        
        if (User.ReturnMemberData().ReturnIsNormalGun())
        {
            //�⺻ ������ ���� Ư���� �Ǹ����� �� �������� ����Ѵ�
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnDx()) * 0.33f + RandomNumber;
        }
        else
        {
            //�� �ܴ� �Ϲ������� �� ������ �̿��Ѵ�
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnSt()) * 0.33f + RandomNumber;
        }

        NormalAttackDamage = (int)RoughDamage;
    }

    /// <summary>
    /// ���� ����� ��� �Լ�
    /// </summary>
    /// <param name="UsedSkill"></param>
    /// <param name="User"></param>
    /// <param name="Target"></param>
    /// <param name="PhysicDamage"></param>
    public void CalculatePhysicDamage(SkillDataRec UsedSkill, OnBattlePartyObject User, OnBattlePartyObject Target, out int PhysicDamage)
    {
        /*
        { (��ų ���� + (�� * 0.75 + �� * 1.5) ) �� ���� ��� �� ���� ���� �� ��� ���� Ƚ�� + ��ų + ���� } + ����(0~15)  (�Ҽ��� ���� �߶󳻱�)
        ������ �ٸ��� ���������� ������ ������ ��� ���� ���� ���� ���
        */
        int SkillPower = (int)UsedSkill.ReturnPower();
        int StOfUser = User.ReturnMemberData().ReturnSt();     //��ų�� ����� ĳ������ �� ���� �޾ƿ���
        int DxOfUser = User.ReturnMemberData().ReturnDx();     //��ų�� ����� ĳ������ �� ���� �޾ƿ���
        float RoughDamage;          //�ʹ� ��갪 (���� ������� �Ҽ��� ���ϸ� �߶󳽴�)

        float AffinityRevision;     //���� ������
        if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Weak)
        {
            //������ 1.5��
            AffinityRevision = 1.5f;
        }
        else if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Resist)
        {
            //������ 0.5��
            AffinityRevision = 0.5f;
        }
        else
        {
            //�� �ܴ� ���� ����
            AffinityRevision = 1f;
        }

        float AverageHit = (UsedSkill.ReturnMinHit() + UsedSkill.ReturnMaxHit()) * 0.5f;        //��� ���� Ƚ���� (�ּ� ��Ʈ��+�ִ� ��Ʈ��) / 2

        #region SupportCoeffceint   //���� ��� ��� ����

        float AttackCoeffcient = 1f;    //���� ���1: ������� ���ݷ� ���� ���� Ƚ���� ���� 20%P �տ���
        float DefenseCoeffceint = 1f;   //���� ���2: ������ ���� ���� ���� Ƚ���� ���� 20%P �տ���
        float TotalSupportCoeffceint;   //���� ���: ��� ���� ������� ��� �Ϸ��� ��
        for (int i = 0; i < User.ReturnIncreasedAttack(); i++)
        {
            AttackCoeffcient += IncreaseValueOfAttack;
        }
        for (int i = 0; i < Target.ReturnDecreasedDefense(); i++)
        {
            DefenseCoeffceint += DecreaseValueOfDefense;
        }
        TotalSupportCoeffceint = AttackCoeffcient * DefenseCoeffceint;

        if (User.ReturnIsPhysicEnhanced())
            TotalSupportCoeffceint += IncreaseValueOfEnhance;      //����Ʈ����Ʈ�� ����� ���¸� ����� ������ �߰� ����

        if (User.ReturnMemberData().ReturnIsSick())
            TotalSupportCoeffceint *= DebuffAttackValueOfSick;  //���⿡ �ɸ� ���¸� ��� ��� ���� �� ���������� 25% ����
        #endregion

        int RandomNumber = Random.Range(0, 16);     //���� 0~15

        RoughDamage = ((SkillPower + (StOfUser * 0.75f + DxOfUser * 1.5f)) * TotalSupportCoeffceint * AffinityRevision / AverageHit) + RandomNumber;

        PhysicDamage = (int)RoughDamage;
    }

    /// <summary>
    /// ���� ����� ��� �Լ�
    /// </summary>
    /// <param name="UsedSkill">����� ��ų</param>
    /// <param name="User">�����</param>
    /// <param name="MagicDamage">���� ���</param>
    public void CalculateMagicDamage(SkillDataRec UsedSkill, OnBattlePartyObject User, OnBattlePartyObject Target, out int MagicDamage)
    {
        /*
        { (��ų ���� + �� �� 4.5 ) �� 3 �� ���� ��� �� ���� ���� �� ��� ���� Ƚ�� + ��ų + ���� } + ����(0~15) (�Ҽ��� ���� �߶󳻱�)

        */
        int SkillPower = (int)UsedSkill.ReturnPower();
        int MaOfUser = User.ReturnMemberData().ReturnMa();     //��ų�� ����� ĳ������ �� ���� �޾ƿ���
        float RoughDamage;          //�ʹ� ��갪 (���� ������� �Ҽ��� ���ϸ� �߶󳽴�)

        float AffinityRevision;     //���� ������
        if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Weak)
        {
            //������ 1.5��
            AffinityRevision = 1.5f;
        }
        else if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Resist)
        {
            //������ 0.5��
            AffinityRevision = 0.5f;
        }
        else
        {
            //�� �ܴ� ���� ����
            AffinityRevision = 1f;
        }

        float AverageHit = (UsedSkill.ReturnMinHit() + UsedSkill.ReturnMaxHit()) * 0.5f;        //��� ���� Ƚ���� (�ּ� ��Ʈ��+�ִ� ��Ʈ��) / 2

        #region SupportCoeffceint   //���� ��� ��� ����

        float AttackCoeffcient = 1f;    //���� ���1: ������� ���ݷ� ���� ���� Ƚ���� ���� 20%P �տ���
        float DefenseCoeffceint = 1f;   //���� ���2: ������ ���� ���� ���� Ƚ���� ���� 20%P �տ���
        float TotalSupportCoeffceint;   //���� ���: ��� ���� ������� ��� �Ϸ��� ��
        for (int i = 0; i < User.ReturnIncreasedAttack(); i++)
        {
            AttackCoeffcient += IncreaseValueOfAttack;
        }
        for (int i = 0; i < Target.ReturnDecreasedDefense(); i++)
        {
            DefenseCoeffceint += DecreaseValueOfDefense;
        }
        TotalSupportCoeffceint = AttackCoeffcient * DefenseCoeffceint;

        if (User.ReturnIsMagicEnhanced())
            TotalSupportCoeffceint += IncreaseValueOfEnhance;      //����Ʈ����Ʈ�� ����� ���¸� ����� ������ �߰� ����

        if (User.ReturnMemberData().ReturnIsSick())
            TotalSupportCoeffceint *= DebuffAttackValueOfSick;  //���⿡ �ɸ� ���¸� ��� ��� ���� �� ���������� 25% ����

        #endregion

        int RandomNumber = Random.Range(0, 16);     //���� 0~15

        RoughDamage = ((SkillPower + MaOfUser * 4.5f) * 0.33f * TotalSupportCoeffceint * AffinityRevision / AverageHit) + RandomNumber;

        MagicDamage = (int)RoughDamage;
    }
}
