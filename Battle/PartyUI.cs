using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUI : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject FirstMember;    //ù��° ��Ƽ ����� ���� ������Ʈ

    [SerializeField] private BattleMemberCell[] MemberScriptArray; //��� ��ũ��Ʈ �迭


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
    public void InitPartyDisplay(List<OnBattleObject?> NowOnBattleObject)
    {
        for (int i = 0; i < 4; i++)
        {
            MemberScriptArray[i].SetMemberCell(NowOnBattleObject[i]);
        }
    }
#nullable disable

    /// <summary>
    /// ��Ƽ�� ������ �ش� ���� ���� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void CallSwapMember(int MemberNumber, OnBattleObject? TargetBattleObject)
    {
        MemberScriptArray[MemberNumber].SwapMemberData(TargetBattleObject);
    }


    /// <summary>
    /// ���� ü�� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainHP"></param>
    public void CallRefreshHPValue(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].RefreshHP();
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainMP"></param>
    public void CallRefreshMPValue(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].RefreshMP();
    }

    /// <summary>
    /// ������ ������ Ȯ���ϰ� ������ ���� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void ActiveTurn(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].NowTurn();
    }
}
