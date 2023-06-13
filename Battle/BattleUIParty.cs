using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIManager : MonoBehaviour
{
    #region SetField

    //↓왜 있는지 모르겠음 나중에 지우기
    //[SerializeField] private GameObject FirstMember;    //첫번째 파티 멤버의 게임 오브젝트

    [SerializeField] private BattleMemberCell[] MemberCellArray; //멤버 스크립트 배열


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
    public void InitPartyDisplay(List<OnBattlePartyObject?> NowOnBattleObject)
    {
        for (int i = 0; i < 4; i++)
        {
            MemberCellArray[i].SetMemberCell(NowOnBattleObject[i]);
        }
    }
#nullable disable

    /// <summary>
    /// 파티원 변동시 해당 슬롯 정보 갱신
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void CallSwapMember(int MemberNumber, OnBattlePartyObject? TargetBattleObject)
    {
        MemberCellArray[MemberNumber].SwapMemberData(TargetBattleObject);
    }


    /// <summary>
    /// 현재 체력 갱신
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainHP"></param>
    public void CallRefreshHPValue(int MemberNumber)
    {
        MemberCellArray[MemberNumber].RefreshHP();
    }

    /// <summary>
    /// 현재 마나 갱신
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainMP"></param>
    public void CallRefreshMPValue(int MemberNumber)
    {
        MemberCellArray[MemberNumber].RefreshMP();
    }

    /// <summary>
    /// 누구의 턴인지 확인하고 프레임 색상 변경
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void ActiveTurn(int MemberNumber)
    {
        MemberCellArray[MemberNumber].NowTurn();
    }
    
    public void DeactiveTurn(int MemberNumber)
    {
        MemberCellArray[MemberNumber].EndTurn();
    }
}


/*
교체한 동료 악마의 바뀐 턴에 따라 현재턴 아님 스크린 덧씌우는 작업 처리 해야 됨,,, 
 
 */