using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �� �ý���
/// </summary>
public enum PressTurn
{
    ReduceAllTurn,      //�����ִ� ��� �� �Ҹ�
    /*
    1. ���濡�� �ݻ� �Ǵ� ����� �Ӽ����� �������� ���
    2. ���ָ� �������� ���
    3. ��ȭ���� ������ ȭ���� ���� ���

    �����ִ� ���� ��� �Ҹ��ϰ� ��� ��� ������� ��ü
    */

    ReduceTwoTurn,      //2�� �Ҹ�
    /*
    1. ���濡�� ��ȿ�� �Ӽ����� �������� ���
    2. ������ ������ ȸ������ ��� (�ϸ�, �ֻ��� ��簡 ������ �̽��� ����)
    3. ���� ���� �����ϰ� ���� �׳� ������ ���
    
    ������ ���� �켱������ ���� 2���� �Ҹ�
    1. 2���� ���� ���� �Ҹ�
    2. ���� ���� ������� ���� ���, 1���� ���� �ϰ� 1���� ������ �� ���� �Ҹ�
    3. ���� ���� �������� ���� ��� 2���� ������ �� ���� �Ҹ�
    */

    ReduceHalfTurn,     //���� �� �Ҹ�
    /*
    1. ������ �Ϲ������� �������� ���(����, ����)
    2. ���� ���� Ÿ��
    
    ������ ���� �켱������ ���� �������� �Ҹ�
    1. 1���� ���� ���� �Ҹ�
    2. ���� ���� �������� ���� ���, 1���� ������ �� ���� �Ҹ�
    */

    ReduceHalfTurn_Party,   //���� �� �Ҹ�(�÷��̾� ������ ����)
    /*
    1. ���� �Ʊ����� ���ʸ� �Ѱ��� ���
    2. �Ʊ� ��ȯ, ���� �Ǵ� ��ü�� ���� ���

    ������ ���� �켱������ ���� �������� �Ҹ�
    1. 1���� ���� ���� �Ҹ�
    2. ���� ���� �������� ���� ���, 1���� ������ �� ���� ���� ������ ����
    */

    ChangeHalfTurn,     //������ �� ���� ���� ������ ����
    /*
    1. ũ��Ƽ���� �߻��ϰų� ������ ������ �Ӽ����� �������� ���
    2. ��ī��Ʈ���� ũ��Ƽ�� ��ī��Ʈ�� �ߵ��� ���(����� ������ �� �ֵ��� �ùٸ��� ����� ���)

    ������ ���� �켱������ ���� �� �Ҹ�
    1. 1���� ������ �� ���� ���� ������ ����
    2. ������ �� ���� �������� ���� ���, 1���� ���� �� �Ҹ�
    */
}

public class BattleManager : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject BattleCanvasObject;
    private List<MonsterData> BattleEnemyDataList;

    [SerializeField] private BattleUI BattleUIScript;
    [SerializeField] private PartyUI PartyUIScript;
    [SerializeField] private TurnUI TurnUIScript;
    [SerializeField] private BattleSkillMenuUI SkillUIScript;

    private PartyMember[] PartyMemberArray; //��Ƽ ������ �޾ƿ� �迭
    private int PartyMemberNumber = 0;  //���� �������� Ȱ�� ������ �Ʊ� �ο� ��

    private int NumberOfPartyWholeTurn;     //��Ƽ �Ͽ��� �����ִ� ������ �� �� ��
    private int NumberOfPartyHalfTurn;      //��Ƽ �Ͽ��� �����ִ� ���� �� ��
    private int NumberOfEnemyWholeTurn;     //�� �Ͽ��� �����ִ� ������ �� �� ��
    private int NumberOfEnemyHalfTurn;      //�� �Ͽ��� �����ִ� ���� �� ��

    private int NumberOfWholeTurn;          //������ �� �� ��
    private int NumberOfHalfTurn;           //���� �� ��

    private bool IsPlayerPhase;              //�÷��̾� ������ Ȯ�ο�: true == �÷��̾� ������/false == �� ������
    private OnBattleObject NowTurnCharacterOfParty;   //�÷��̾� ������ �� ���� �ൿ ���� �Ʊ�


    #endregion

    private void Awake()
    {
        //BattleEnemyDataList = new List<MonsterData>();

        //PartyMemberArray = new PartyMember[4];  //��Ƽ ���� �迭 �ʱ�ȭ
    }

    private void Start()
    {
        //GetEnemyData();
        //SetPartyData();
        //SetPlayerTurn();
    }

    #region SetBattleInfo
    private void GetEnemyData()
    {
        EnemyData EnemyDataBase = new EnemyData();
        List<int> EnemyIndexList = new List<int>();
        EnemyIndexList = EnemyDataBase.ReturnList();

        for (int i = 0; i < EnemyIndexList.Count; i++)
        {
            BattleEnemyDataList.Add(EnemyDataBase.ReturnEnemyData(EnemyIndexList[i]));  //�ε����� ���� ���������� ������ ������ ������ �����͸� ������
        }
        EnemyDataBase.ClearList();  //�ӽÿ� �ε��� ����Ʈ Ŭ����

        BattleUIScript.PrintEnemyData(BattleEnemyDataList);
    }

    /// <summary>
    /// ���� ���۽� UI�� ��Ƽ ���� ����
    /// </summary>
    private void SetPartyData()
    {
        //BattlePlayerData = GameManager.Instance.GetPlayerData();

        for (int i = 0; i < 4; i++)
        {
            PartyMemberArray[i] = GameManager.Instance.GetPartyData(i); //���� ���� ����� �迭�� ��Ƽ�� ������ �޾ƿ�

            if (PartyMemberArray[i].IsEmptyMember)
            {
                PartyUIScript.SetEmptyMember(i);
            }
            else
            {
                PartyUIScript.SetUIText(i, PartyMemberArray[i].Name, PartyMemberArray[i].MaxHP, PartyMemberArray[i].MaxMP);
                PartyUIScript.SetHPValue(i, PartyMemberArray[i].RemainHP);
                PartyUIScript.SetMPValue(i, PartyMemberArray[i].RemainMP);

                PartyMemberNumber++;    //Ȱ�� ������ �Ʊ� �ο� �� ����
            }
        }
    }
    #endregion

    #region Skill

    /// <summary>
    /// �ൿ�� ��ü�� ���� �ൿ���� ĳ������ ��ų ������ ����
    /// </summary>
    public void CallSetSkillList()
    {
        SkillUIScript.SetSkillList(NowTurnCharacterOfParty);
    }


    /// <summary>
    /// ��ų ����â���� ��ų�� ����� �� � ��ų���� Ȯ���ϰ� � UI�� �ѷ����� �����ϴ� ��
    /// </summary>
    public void CheckSelectedSkill()
    {
        
    }



    #endregion


    #region CheckTurns

    //�ӽÿ�
    private void SetPlayerTurn()
    {
        TurnUIScript.PlacePlayerTurn(PartyMemberNumber);
    }

    /// <summary>
    /// ������ ��ȯ�� �ൿ�ϼ��� �˸°� ä���ִ� �Լ�
    /// </summary>
    private void FillNumberOfTurn()
    {
        if (IsPlayerPhase)
        {
            //�÷��̾��� ��Ƽ���� ���� ���� ���� �������ŭ ������ �� �� ���� ä���ش�
            //�� ���ڸ�ŭ ��Ʋ UI �����ڿ��� �� �������� ä���� �����Ѵ�
        }
        else
        {
            //���� ��Ƽ���� ���� ���� ���� �������ŭ ������ �� �� ���� ä���ش�
            //�� ���ڸ�ŭ ��Ʋ UI �����ڿ��� �� �������� ä���� �����Ѵ�
        }

    }

    
    /// <summary>
    /// ������ �� �꿡 ���� ���� ������� �����ִ� �� ���� �����ϴ� �Լ�
    /// </summary>
    /// <param name="TurnFlag">� �ൿ�� ���ߴ���</param>
    private void ReduceTurn(PressTurn TurnFlag)
    {
        switch (TurnFlag)
        {
            case PressTurn.ReduceAllTurn:
                NumberOfWholeTurn = 0;
                NumberOfHalfTurn = 0;
                //������ ���� �� ��� ������ ��ȯ �Լ� ����
                //�� ���� �� ������ ��ȯ ���� �Լ� ����
                //�����غ��� �����͵� ���� ���� ���� 0 ���ϰ� �Ǹ� ������ �Ѿ���� �׼� ���� �ɾ�θ� �ǳ�?
                break;
            case PressTurn.ReduceTwoTurn:
                {
                    if (NumberOfHalfTurn >= 2)
                        NumberOfHalfTurn -= 2;
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn >= 1)
                    {
                        NumberOfHalfTurn -= 1;
                        NumberOfWholeTurn -= 1;
                    }
                    else
                        NumberOfWholeTurn -= 2;
                }
                break;
            case PressTurn.ReduceHalfTurn:
                {
                    if (NumberOfHalfTurn >= 1)
                        NumberOfHalfTurn -= 1;
                    else
                        NumberOfWholeTurn -= 1;
                }
                break;
            case PressTurn.ReduceHalfTurn_Party:
                {
                    if (NumberOfHalfTurn >= 1)
                        NumberOfHalfTurn -= 1;
                    else
                    {
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }    
                }
                break;
            case PressTurn.ChangeHalfTurn:
                {
                    if (NumberOfWholeTurn >= 1)
                    {
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }
                    else
                        NumberOfHalfTurn -= 1;
                }
                break;
        }

        if (NumberOfWholeTurn == 0 && NumberOfHalfTurn == 0)
            IsPlayerPhase = !IsPlayerPhase;   //��� ���� �ൿ�� ���� 0�� �Ǿ��� ��� ���� ������� �Ѿ��
    }

    #endregion
}

/*
 �����̻��

�����̸� �ް��ؼ� �ɸ��� (���� 90�� ���� ��)
�����̸� ����� �氨?
������ ��?��
�̽��� ������ �߰��ϰ��� ����
��ȿ�� �� 2�� ����->����/�� �迭�� �޸� �߰� ȿ���δ� �������� �ʴ´�
 */