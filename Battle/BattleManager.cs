using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject BattleCanvasObject;
    private List<MonsterData> BattleEnemyDataList;

    private BattleUI BattleUIScript;
    [SerializeField] private PartyUI PartyUIScript;
    [SerializeField] private TurnUI TurnUIScript;

    private PartyMember[] PartyMemberArray; //��Ƽ ������ �޾ƿ� �迭
    private int PartyMemberNumber = 0;  //���� �������� Ȱ�� ������ �Ʊ� �ο� ��


    #endregion

    private void Awake()
    {
        BattleEnemyDataList = new List<MonsterData>();
        BattleUIScript = BattleCanvasObject.GetComponent<BattleUI>();
        PartyMemberArray = new PartyMember[4];  //��Ƽ ���� �迭 �ʱ�ȭ
    }

    private void Start()
    {
        GetEnemyData();
        SetPartyData();
        SetPlayerTurn();
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

    #region CheckTurns

    //�ӽÿ�
    private void SetPlayerTurn()
    {
        TurnUIScript.PlacePlayerTurn(PartyMemberNumber);
    }

    #endregion
}
