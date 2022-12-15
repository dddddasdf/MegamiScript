using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// ��Ƽ ��� ������ ������ ����ü
/// </summary>
public struct PartyMember
{
    public bool IsEmptyMember;  //�ش� ��ȣ�� ����� �������� Ȯ��
    public string Name; //�̸�
    public int Level;   //���� ����
    public int EXP;
    public int MaxHP;   //�ִ� ü��
    public int RemainHP;    //���� ü��
    public int MaxMP;   //�ִ� ����
    public int RemainMP;    //���� ����
    public Status PlayerStatus;

    //���� ��ų ����Ʈ ���� ��
}

public class PlayerData
{
    private PartyMember PlayerStruct;   //��Ƽ�� �÷��̾� ĳ���ʹ� ù��° ����� ����
    private PartyMember[] PartyMemberArray; //��Ƽ ��� �迭

    /// <summary>
    /// �ӽÿ� ���� ���� ������ ����
    /// </summary>
    public void SetPlayerTmp()
    {
        PartyMemberArray = new PartyMember[4];
        
        PartyMemberArray[0].IsEmptyMember = false;
        PartyMemberArray[0].Name = "ȫ�浿";
        PartyMemberArray[0].Level = 10;
        PartyMemberArray[0].EXP = 50;
        PartyMemberArray[0].MaxHP = 100;
        PartyMemberArray[0].RemainHP = 70;
        PartyMemberArray[0].MaxMP = 70;
        PartyMemberArray[0].RemainMP = 70;

        PartyMemberArray[1].IsEmptyMember = false;
        PartyMemberArray[1].Name = "�� ���ν�Ʈ";
        PartyMemberArray[1].Level = 10;
        PartyMemberArray[1].EXP = 50;
        PartyMemberArray[1].MaxHP = 80;
        PartyMemberArray[1].RemainHP = 80;
        PartyMemberArray[1].MaxMP = 50;
        PartyMemberArray[1].RemainMP = 27;

        PartyMemberArray[2].IsEmptyMember = false;
        PartyMemberArray[2].Name = "������";
        PartyMemberArray[2].Level = 10;
        PartyMemberArray[2].EXP = 50;
        PartyMemberArray[2].MaxHP = 93;
        PartyMemberArray[2].RemainHP = 58;
        PartyMemberArray[2].MaxMP = 128;
        PartyMemberArray[2].RemainMP = 99;

        PartyMemberArray[3].IsEmptyMember = true;
        PartyMemberArray[3].Name = "";
        PartyMemberArray[3].Level = 0;
        PartyMemberArray[3].EXP = 0;
        PartyMemberArray[3].MaxHP = 0;
        PartyMemberArray[3].RemainHP = 0;
        PartyMemberArray[3].MaxMP = 0;
        PartyMemberArray[3].RemainMP = 0;
    }

    public PartyMember ReturnPlayerData()
    {
        return PlayerStruct;
    }

    /// <summary>
    /// ������ ������ �� ��Ƽ ���� ��ȯ
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <returns></returns>
    public PartyMember ReturnPartyData(int MemberNumber)
    {
        return PartyMemberArray[MemberNumber];
    }
}
