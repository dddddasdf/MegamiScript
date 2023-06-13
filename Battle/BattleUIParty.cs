using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleUIManager : MonoBehaviour
{
    #region SetField

    //��� �ִ��� �𸣰��� ���߿� �����
    //[SerializeField] private GameObject FirstMember;    //ù��° ��Ƽ ����� ���� ������Ʈ

    [SerializeField] private BattleMemberCell[] MemberCellArray; //��� ��ũ��Ʈ �迭


    #endregion

    //private void Awake()
    //{
    //    FirstMemberScript = FirstMember.GetComponent<BattlePartyMember>();
    //}

#nullable enable
    /// <summary>
    /// ���� ���� �� ��Ƽ ����Ʈ�� ���� �ǰ� UI�� ���� �����ֱ�
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
    /// ��Ƽ�� ������ �ش� ���� ���� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void CallSwapMember(int MemberNumber, OnBattlePartyObject? TargetBattleObject)
    {
        MemberCellArray[MemberNumber].SwapMemberData(TargetBattleObject);
    }


    /// <summary>
    /// ���� ü�� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainHP"></param>
    public void CallRefreshHPValue(int MemberNumber)
    {
        MemberCellArray[MemberNumber].RefreshHP();
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainMP"></param>
    public void CallRefreshMPValue(int MemberNumber)
    {
        MemberCellArray[MemberNumber].RefreshMP();
    }

    /// <summary>
    /// ������ ������ Ȯ���ϰ� ������ ���� ����
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
��ü�� ���� �Ǹ��� �ٲ� �Ͽ� ���� ������ �ƴ� ��ũ�� ������� �۾� ó�� �ؾ� ��,,, 
 
 */