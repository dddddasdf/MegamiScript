using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSystem
{
    private float IncreaseValueOfAttack = 0.2f;        //공격력 증가 버프에 의해 올라가는 대미지 증가율의 수치는 20%
    private float DecreaseValueOfDefense = 0.2f;        //공격력 증가 버프에 의해 올라가는 대미지 증가율의 수치는 20%
    private float DebuffAttackValueOfSick = 0.75f;      //감기에 의해 깎이는 공격력은 25%
    private float IncreaseValueOfEnhance = 1.5f;        //차지 or 컨센트레이트에 올라가는 대미지 증가율의 수치는 250%

    /// <summary>
    /// 주인공용 일반 공격 대미지 계산
    /// </summary>
    /// <param name="User"></param>
    /// <param name="Target"></param>
    /// <param name="NormalAttackDamage"></param>
    /// <param name="IsSwordAttack">주인공은 통상 공격과 "총"통상 공격으로 나뉜다</param>
    public void CalculatePlayerNormalAttackDamage(OnBattlePartyObject User, OnBattlePartyObject Target, bool IsSwordAttack,out int NormalAttackDamage)
    {
        /*
        최소 데미지=(무기 위력+힘)/3(소수 점 이하 반올림)
         */
        float WeaponPower = 1f;          //주인공의 경우에만 무기 위력을 받아오고, 동료 악마는 별도 계산식을 거친다


        //주인공 무기 데이터 받아오는 작업이 필요하다

        float RoughDamage;
        int RandomNumber = Random.Range(0, 16);     //난수 0~15

        if (IsSwordAttack)
        {
            //통상 공격
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnDx()) * 0.33f + RandomNumber;
        }
        else
        {
            //총 통상 공격
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnSt()) * 0.33f + RandomNumber;
        }

        NormalAttackDamage = (int)RoughDamage;
    }

    /// <summary>
    /// 악마용 일반 공격 대미지 계산
    /// </summary>
    /// <param name="User"></param>
    /// <param name="Target"></param>
    /// <param name="NormalAttackDamage"></param>
    public void CalculateDemonNormalAttackDamage(OnBattlePartyObject User, OnBattlePartyObject Target, out int NormalAttackDamage)
    {
        /*
        최소 데미지=(무기 위력+힘)/3(소수 점 이하 반올림)
        무기 위력은 평균 히트수마다 상이한 보정값에 악마의 초기 레벨을 활용한 값
         */
        float WeaponPower = 1f;          //주인공의 경우에만 무기 위력을 받아오고, 동료 악마는 별도 계산식을 거친다

        int MinHit = User.ReturnMemberData().ReturnNormalMinHit();
        int MaxHit = User.ReturnMemberData().ReturnNormalMaxHit();
        float AverageHit = (MinHit + MaxHit) * 0.5f;

        switch (AverageHit)
        {
            case 1f:
                WeaponPower = 4.3f *User.ReturnMemberData().ReturnBeginningLv() + 49.6f;
                break;
            case 1.5f:
                //통상공격이 1~2회
                WeaponPower = 3f * User.ReturnMemberData().ReturnBeginningLv() + 35f;
                break;
            case 2f:
                //통상 공격이 2회 or 1~3회
                WeaponPower = 2.2f * User.ReturnMemberData().ReturnBeginningLv() + 26.8f;
                break;
            case 2.5f:
                //통상 공격이 2~3회
                WeaponPower = 1.5f * User.ReturnMemberData().ReturnBeginningLv() + 42.5f;
                break;
        }

        float RoughDamage;
        int RandomNumber = Random.Range(0, 16);     //난수 0~15
        
        if (User.ReturnMemberData().ReturnIsNormalGun())
        {
            //기본 공격이 총인 특수한 악마들은 기 스탯으로 계산한다
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnDx()) * 0.33f + RandomNumber;
        }
        else
        {
            //그 외는 일반적으로 힘 스탯을 이용한다
            RoughDamage = (WeaponPower + User.ReturnMemberData().ReturnSt()) * 0.33f + RandomNumber;
        }

        NormalAttackDamage = (int)RoughDamage;
    }

    /// <summary>
    /// 물리 대미지 계산 함수
    /// </summary>
    /// <param name="UsedSkill"></param>
    /// <param name="User"></param>
    /// <param name="Target"></param>
    /// <param name="PhysicDamage"></param>
    public void CalculatePhysicDamage(SkillDataRec UsedSkill, OnBattlePartyObject User, OnBattlePartyObject Target, out int PhysicDamage)
    {
        /*
        { (스킬 위력 + (힘 * 0.75 + 기 * 1.5) ) × 보조 계수 × 약점 보정 ÷ 평균 공격 횟수 + 스킬 + 보정 } + 난수(0~15)  (소수점 이하 잘라내기)
        마법과 다르게 공식적으로 밝혀진 계산식은 없어서 유저 추정 계산식 사용
        */
        int SkillPower = (int)UsedSkill.ReturnPower();
        int StOfUser = User.ReturnMemberData().ReturnSt();     //스킬을 사용한 캐릭터의 힘 스탯 받아오기
        int DxOfUser = User.ReturnMemberData().ReturnDx();     //스킬을 사용한 캐릭터의 기 스탯 받아오기
        float RoughDamage;          //초벌 계산값 (최종 대미지는 소수점 이하를 잘라낸다)

        float AffinityRevision;     //약점 보정값
        if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Weak)
        {
            //약점시 1.5배
            AffinityRevision = 1.5f;
        }
        else if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Resist)
        {
            //내성시 0.5배
            AffinityRevision = 0.5f;
        }
        else
        {
            //그 외는 보정 없음
            AffinityRevision = 1f;
        }

        float AverageHit = (UsedSkill.ReturnMinHit() + UsedSkill.ReturnMaxHit()) * 0.5f;        //평균 공격 횟수는 (최소 히트수+최대 히트수) / 2

        #region SupportCoeffceint   //보조 계수 계산 구간

        float AttackCoeffcient = 1f;    //보조 계수1: 사용자의 공격력 증가 버프 횟수에 따라 20%P 합연산
        float DefenseCoeffceint = 1f;   //보조 계수2: 상대방의 방어력 감소 버프 횟수에 따라 20%P 합연산
        float TotalSupportCoeffceint;   //최종 계수: 모든 보조 계수들을 계산 완료한 값
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
            TotalSupportCoeffceint += IncreaseValueOfEnhance;      //컨센트레이트가 적용된 상태면 대미지 합으로 추가 증가

        if (User.ReturnMemberData().ReturnIsSick())
            TotalSupportCoeffceint *= DebuffAttackValueOfSick;  //감기에 걸린 상태면 모든 계수 보정 후 최종적으로 25% 차감
        #endregion

        int RandomNumber = Random.Range(0, 16);     //난수 0~15

        RoughDamage = ((SkillPower + (StOfUser * 0.75f + DxOfUser * 1.5f)) * TotalSupportCoeffceint * AffinityRevision / AverageHit) + RandomNumber;

        PhysicDamage = (int)RoughDamage;
    }

    /// <summary>
    /// 마법 대미지 계산 함수
    /// </summary>
    /// <param name="UsedSkill">사용한 스킬</param>
    /// <param name="User">사용자</param>
    /// <param name="MagicDamage">적용 대상</param>
    public void CalculateMagicDamage(SkillDataRec UsedSkill, OnBattlePartyObject User, OnBattlePartyObject Target, out int MagicDamage)
    {
        /*
        { (스킬 위력 + 마 × 4.5 ) ÷ 3 × 보조 계수 × 약점 보정 ÷ 평균 공격 횟수 + 스킬 + 보정 } + 난수(0~15) (소수점 이하 잘라내기)

        */
        int SkillPower = (int)UsedSkill.ReturnPower();
        int MaOfUser = User.ReturnMemberData().ReturnMa();     //스킬을 사용한 캐릭터의 마 스탯 받아오기
        float RoughDamage;          //초벌 계산값 (최종 대미지는 소수점 이하를 잘라낸다)

        float AffinityRevision;     //약점 보정값
        if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Weak)
        {
            //약점시 1.5배
            AffinityRevision = 1.5f;
        }
        else if (Target.ReturnMemberData().ReturnMatchingAffinity(UsedSkill.ReturnSkillType()) == SkillAffinities.Resist)
        {
            //내성시 0.5배
            AffinityRevision = 0.5f;
        }
        else
        {
            //그 외는 보정 없음
            AffinityRevision = 1f;
        }

        float AverageHit = (UsedSkill.ReturnMinHit() + UsedSkill.ReturnMaxHit()) * 0.5f;        //평균 공격 횟수는 (최소 히트수+최대 히트수) / 2

        #region SupportCoeffceint   //보조 계수 계산 구간

        float AttackCoeffcient = 1f;    //보조 계수1: 사용자의 공격력 증가 버프 횟수에 따라 20%P 합연산
        float DefenseCoeffceint = 1f;   //보조 계수2: 상대방의 방어력 감소 버프 횟수에 따라 20%P 합연산
        float TotalSupportCoeffceint;   //최종 계수: 모든 보조 계수들을 계산 완료한 값
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
            TotalSupportCoeffceint += IncreaseValueOfEnhance;      //컨센트레이트가 적용된 상태면 대미지 합으로 추가 증가

        if (User.ReturnMemberData().ReturnIsSick())
            TotalSupportCoeffceint *= DebuffAttackValueOfSick;  //감기에 걸린 상태면 모든 계수 보정 후 최종적으로 25% 차감

        #endregion

        int RandomNumber = Random.Range(0, 16);     //난수 0~15

        RoughDamage = ((SkillPower + MaOfUser * 4.5f) * 0.33f * TotalSupportCoeffceint * AffinityRevision / AverageHit) + RandomNumber;

        MagicDamage = (int)RoughDamage;
    }
}
