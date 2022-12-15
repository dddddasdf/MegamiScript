using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private Image[] PlayerTurnMark;    //�÷��̾� �� ��ũ
    [SerializeField] private Image[] EnemyTurnMark;     //�� �� ��ũ
    [SerializeField] private TextMeshProUGUI TurnText;      //������ ������ ǥ���ϴ� �ؽ�Ʈ


    private void Awake()
    {
        //��Ʋ���� ���� �� �� ��ũ�� �����ִ� �� ���� ���Ͽ� �� �� �� �������� ���ִ� �۾�
        for (int i = 0; i < PlayerTurnMark.Length; i++)
            PlayerTurnMark[i].enabled = false;
        for (int i = 0; i < EnemyTurnMark.Length; i++)
            EnemyTurnMark[i].enabled = false;
    }

    /// <summary>
    /// �÷��̾��� ������ ǥ��
    /// </summary>
    /// <param name="ActivePartyMember">���� Ȱ�� ������ ��Ƽ ����� ��</param>
    public void PlacePlayerTurn(int ActivePartyMember)
    {
        for (int i = 0; i < ActivePartyMember; i++)
            PlayerTurnMark[i].enabled = true;   //���� Ȱ�� ������ ��Ƽ ����� ��ŭ ��ũ ������ ����

        TurnText.text = "<i><color=#3AEC37>Player</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// �÷��̾� ���� �Ҹ������� ��ũ�� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void HidePlayerMark(int TurnNumber)
    {
        PlayerTurnMark[TurnNumber - 1].enabled = false;
    }

    /// <summary>
    /// ���� ������ ǥ��
    /// </summary>
    /// <param name="ActiveEnemyMember">���� Ȱ�� ������ ���� ��</param>
    public void PlaceEnemyTurn(int ActiveEnemyMember)
    {
        for (int i = 0; i < ActiveEnemyMember; i++)
            EnemyTurnMark[i].enabled = true;   //���� Ȱ�� ������ ���� �� ��ŭ ��ũ ������ ����

        TurnText.text = "<i><color=#FB2F2F>Enemy</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// ���� ���� �Ҹ������� ��ũ�� ����
    /// </summary>
    /// <param name="TurnNumber"></param>
    public void HideEnemyMark(int TurnNumber)
    {
        EnemyTurnMark[TurnNumber - 1].enabled = false;
    }
}
