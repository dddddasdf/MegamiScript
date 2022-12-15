using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUI : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject FirstMember;    //첫번째 파티 멤버의 게임 오브젝트

    [SerializeField] private BattlePartyMember[] MemberScriptArray; //멤버 스크립트 배열


    #endregion

    //private void Awake()
    //{
    //    FirstMemberScript = FirstMember.GetComponent<BattlePartyMember>();
    //}

    /// <summary>
    /// 해당 프레임에 공석 표시
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void SetEmptyMember(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].SetEmptyText();
    }

    /// <summary>
    /// 해당 멤버 UI의 이름, 최대 체력, 마나 설정
    /// </summary>
    /// <param name="MemberNumber"> 몇번째 멤버 </param>
    public void SetUIText(int MemberNumber, string Name, int MaxHp, int MaxMP)
    {
        MemberScriptArray[MemberNumber].GetName(Name);
        MemberScriptArray[MemberNumber].GetMaxHPMP(MaxHp, MaxMP);
    }

    /// <summary>
    /// 현재 체력 전달
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainHP"></param>
    public void SetHPValue(int MemberNumber, int RemainHP)
    {
        MemberScriptArray[MemberNumber].GetHPValue(RemainHP);
    }

    /// <summary>
    /// 현재 마나 전달
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainMP"></param>
    public void SetMPValue(int MemberNumber, int RemainMP)
    {
        MemberScriptArray[MemberNumber].GetMPValue(RemainMP);
    }

    /// <summary>
    /// 누구의 턴인지 확인하고 프레임 색상 변경
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void ActiveTurn(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].NowTurn();
    }
}
