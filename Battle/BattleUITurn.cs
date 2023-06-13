using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class BattleUIManager : MonoBehaviour
{
    [SerializeField] private Image[] PlayerWholeTurnMark;    //�÷��̾� ������ �� ��ũ
    [SerializeField] private Image[] EnemyWholeTurnMark;     //�� ������ �� ��ũ
    [SerializeField] private Image[] HalfTurnMark;    //�÷��̾� ���� �� ��ũ
    [SerializeField] private TextMeshProUGUI TurnText;      //������ ������ ǥ���ϴ� �ؽ�Ʈ


    
    private void InitTurnUI()
    {
        //��Ʋ���� ���� �� �� ��ũ�� �����ִ� �� ���� ���Ͽ� �� �� �� �������� ���ִ� �۾�
        for (int i = 0; i < PlayerWholeTurnMark.Length; i++)
            PlayerWholeTurnMark[i].enabled = false;
        for (int i = 0; i < EnemyWholeTurnMark.Length; i++)
            EnemyWholeTurnMark[i].enabled = false;
    }

    /// <summary>
    /// ������ ��ȯ�� ���� �� �ο��� ���缭 ������ �� ��ġ
    /// </summary>
    /// <param name="NumberOfActiveMember">���� ���� �ο�</param>
    /// <param name="IsPlayerTurn">�÷��̾��� ���ΰ�?</param>
    public void PlaceTurn(int NumberOfActiveMember, bool IsPlayerTurn)
    {
        if (IsPlayerTurn)
        {
            for (int i = 0; i < NumberOfActiveMember; i++)
                PlayerWholeTurnMark[i].enabled = true;   //���� Ȱ�� ������ ��Ƽ ����� ��ŭ ��ũ ������ ����

            TurnText.text = "<i><color=#3AEC37>Player</color> <color=#FFFFFF>Turn</color></i>";
        }
        else
        {

        }
    }

    /// <summary>
    /// ��� �� ����
    /// </summary>
    /// <param name="IsPlayerTurn">�÷��̾��� ���ΰ�?</param>
    public void HideAllMark(bool IsPlayerTurn)
    {
        if (IsPlayerTurn)
        {
            foreach (Image TurnMark in PlayerWholeTurnMark)
            {
                TurnMark.enabled = false;
            }
        }
        else
        {
            foreach (Image TurnMark in EnemyWholeTurnMark)
            {
                TurnMark.enabled = false;
            }
        }
        HideAllHalfTurn();
    }

    /// <summary>
    /// �Ҹ��� ������ �ϼ���ŭ ��ũ ���߱�
    /// </summary>
    /// <param name="NumberOfRemainTurn">�����ִ� ������ �ϼ�</param>
    /// <param name="NumberOfUsedTurn">�Ҹ��� ������ �ϼ�</param>
    /// <param name="IsPlayerTurn">�÷��̾��� ���ΰ�?</param>
    public void HideWholeTurnMark(int NumberOfRemainTurn, int NumberOfUsedTurn, bool IsPlayerTurn)
    {
        if (IsPlayerTurn)
        {
            for (int i = 1; i <= NumberOfUsedTurn; i++)
                PlayerWholeTurnMark[NumberOfRemainTurn - i].enabled = false;
        }
        else
        {
            for (int i = 1; i <= NumberOfUsedTurn; i++)
                EnemyWholeTurnMark[NumberOfRemainTurn - i].enabled = false;
        }
    }

    /// <summary>
    /// ���� �� ����->���� �� ��ũ ǥ���ϱ�
    /// </summary>
    /// <param name="NumberOfRemainTurn">�����ִ� ������ �ϼ�</param>
    public void PlaceHalfTurnMark(int NumberOfRemainTurn, int NumberOfRemainHalfTurn, int NumberOfUsedTurn, int NumberOfOnBattle)
    {
        if (NumberOfRemainHalfTurn == 0)
        {
            HalfTurnMark[NumberOfRemainTurn - 1].enabled = true;
            //�����ִ� ������ �ϼ��� �ε����� ���� �ϼ��� �ε����� ġȯ�Ѵٰ� �����ϸ�... ���ϴ�...
        }
        else
        {
            for (int i = (NumberOfOnBattle - NumberOfUsedTurn - 1); i >= 0; i--)
            {
                if (HalfTurnMark[i].enabled == false)
                {
                    HalfTurnMark[i].enabled = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// ���� �� �Ҹ�->���� �� ���߱�
    /// </summary>
    /// <param name="NumberOfRemainTurn">�����ִ� ������ �ϼ�</param>
    /// <param name="NumberOfHalfTurn">�����ִ� ���� �ϼ�</param>
    /// <param name="NumberOfUsedHalfTurn">�Ҹ��� ���� �ϼ�</param>
    public void HideHalfTurnMark(int NumberOfRemainTurn, int NumberOfHalfTurn, int NumberOfUsedHalfTurn, bool IsPlayerTurn)
    {
        if (NumberOfUsedHalfTurn == 1)
        {
            HalfTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;

            if (IsPlayerTurn)
            {
                PlayerWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
            }
            else
            {
                EnemyWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
            }
        }
        else if (NumberOfUsedHalfTurn == 2)
        {
            HalfTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
            HalfTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 2].enabled = false;

            if (IsPlayerTurn)
            {
                PlayerWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
                PlayerWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 2].enabled = false;
            }
            else
            {
                EnemyWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
                EnemyWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 2].enabled = false;
            }
        }

        //�����ִ� ������ �ϼ��� ���� ���� �ϼ��� ���ϰ� 1 ���� ����� �� �̹��� �ε���
        //���� �� ǥ�ð� ������� �� �� ������ �� ǥ�� ���� ���� ������� ���̹Ƿ� ���ÿ� ó�����ش�
        //�� ���� �ݺ��� ������ �� �ʿ䰡 ������ �;... ���� �ϼ��� �� ���� �ִ� 2������ �Ҹ��ϹǷ�
    }

    /// <summary>
    /// �÷��̾��� ������ ǥ��
    /// </summary>
    /// <param name="ActivePartyMember">���� Ȱ�� ������ ��Ƽ ����� ��</param>
    public void PlacePlayerTurn(int ActivePartyMember)
    {
        for (int i = 0; i < ActivePartyMember; i++)
            PlayerWholeTurnMark[i].enabled = true;   //���� Ȱ�� ������ ��Ƽ ����� ��ŭ ��ũ ������ ����

        TurnText.text = "<i><color=#3AEC37>Player</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// �÷��̾� ���� �Ҹ������� ��ũ�� ����
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void HidePlayerMark(int TurnNumber)
    {
        PlayerWholeTurnMark[TurnNumber - 1].enabled = false;
    }

    /// <summary>
    /// ��� �� ������ �����ִ� ��� ���� �������� ����-�÷��̾�
    /// </summary>
    public void HideAllPlayerMark()
    {
        foreach (Image TurnMark in PlayerWholeTurnMark)
        {
            TurnMark.enabled = false;
        }
        HideAllHalfTurn();
    }

    /// <summary>
    /// ���� ������ ǥ��
    /// </summary>
    /// <param name="ActiveEnemyMember">���� Ȱ�� ������ ���� ��</param>
    public void PlaceEnemyTurn(int ActiveEnemyMember)
    {
        for (int i = 0; i < ActiveEnemyMember; i++)
            EnemyWholeTurnMark[i].enabled = true;   //���� Ȱ�� ������ ���� �� ��ŭ ��ũ ������ ����

        TurnText.text = "<i><color=#FB2F2F>Enemy</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// ���� ���� �Ҹ������� ��ũ�� ����
    /// </summary>
    /// <param name="TurnNumber"></param>
    public void HideEnemyMark(int TurnNumber)
    {
        EnemyWholeTurnMark[TurnNumber - 1].enabled = false;
    }

    /// <summary>
    /// ��� �� ������ �����ִ� ��� ���� �������� ����-��
    /// </summary>
    public void HideAllEnemyMark()
    {
        foreach (Image TurnMark in EnemyWholeTurnMark)
        {
            TurnMark.enabled = false;
        }
        HideAllHalfTurn();
    }

    /// <summary>
    /// ������ ǥ��
    /// </summary>
    public void PlaceHalfTurn(int RemainWholeTurn, int HalfTurn)
    {

    }

    /// <summary>
    /// ��� ���� �� ������ ����
    /// </summary>
    public void HideAllHalfTurn()
    {
        foreach (Image TurnMark in HalfTurnMark)
        {
            TurnMark.enabled = false;
        }
    }
}
