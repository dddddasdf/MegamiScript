using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투에 참전 중인 객체
/// </summary>
public class OnBattleObject : PartyMemberData
{  
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


public class OnBattleParty
{
 
}
