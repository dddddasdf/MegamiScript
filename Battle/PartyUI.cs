using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyUI : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject FirstMember;    //ù��° ��Ƽ ����� ���� ������Ʈ

    [SerializeField] private BattlePartyMember[] MemberScriptArray; //��� ��ũ��Ʈ �迭


    #endregion

    //private void Awake()
    //{
    //    FirstMemberScript = FirstMember.GetComponent<BattlePartyMember>();
    //}

    /// <summary>
    /// �ش� �����ӿ� ���� ǥ��
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void SetEmptyMember(int MemberNumber)
    {
        MemberScriptArray[MemberNumber].SetEmptyText();
    }

    /// <summary>
    /// �ش� ��� UI�� �̸�, �ִ� ü��, ���� ����
    /// </summary>
    /// <param name="MemberNumber"> ���° ��� </param>
    public void SetUIText(int MemberNumber, string Name, int MaxHp, int MaxMP)
    {
        MemberScriptArray[MemberNumber].GetName(Name);
        MemberScriptArray[MemberNumber].GetMaxHPMP(MaxHp, MaxMP);
    }

    /// <summary>
    /// ���� ü�� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainHP"></param>
    public void SetHPValue(int MemberNumber, int RemainHP)
    {
        MemberScriptArray[MemberNumber].GetHPValue(RemainHP);
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <param name="RemainMP"></param>
    public void SetMPValue(int MemberNumber, int RemainMP)
    {
        MemberScriptArray[MemberNumber].GetMPValue(RemainMP);
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
