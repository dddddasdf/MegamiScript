using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUI : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject FirstMember;    //첫번째 파티 멤버의 게임 오브젝트

    [SerializeField] private BattleMemberCell[] MemberScriptArray; //멤버 스크립트 배열


    #endregion

    //private void Awake()
    //{
    //    FirstMemberScript = FirstMember.GetComponent<BattlePartyMember>();
    //}

#nullable enable
    /// <summary>
    /// 전투 돌입 후 파티 리스트가 세팅 되고 UI에 정보 보내주기
    /// </summary>
    /// <param name="NowOnBattleObject"></param>
    public void InitPartyDisplay(List<OnBattleObject?> NowOnBattleObject)
    {
        for (int i = 0; i < 4; i++)
        {
            MemberScriptArray[i].SetMemberCell(NowOnBattleObject[i]);
        }
    }
#nullable disable

    /// <summary>
    /// 파티원 변동시 해당 슬롯 정보 갱신
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void CallSwapMember(int MemberNumber, OnBattleObject? TargetBattleObject)
    {
        MemberScriptArray[MemberNumber].SwapMemberData(TargetBattleObject);
    }


    /// <summary>
    /// 현재 체력 갱신
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainHP"></param>
    public void CallRefreshHPValue(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].RefreshHP();
    }

    /// <summary>
    /// 현재 마나 갱신
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainMP"></param>
    public void CallRefreshMPValue(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].RefreshMP();
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
