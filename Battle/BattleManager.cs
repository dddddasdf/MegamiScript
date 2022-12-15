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

    private PartyMember[] PartyMemberArray; //파티 정보를 받아올 배열
    private int PartyMemberNumber = 0;  //현재 전투에서 활동 가능한 아군 인원 수


    #endregion

    private void Awake()
    {
        BattleEnemyDataList = new List<MonsterData>();
        BattleUIScript = BattleCanvasObject.GetComponent<BattleUI>();
        PartyMemberArray = new PartyMember[4];  //파티 정보 배열 초기화
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
            BattleEnemyDataList.Add(EnemyDataBase.ReturnEnemyData(EnemyIndexList[i]));  //인덱스를 통해 순차적으로 전투에 나오는 적들의 데이터를 가져옴
        }
        EnemyDataBase.ClearList();  //임시용 인덱스 리스트 클리어

        BattleUIScript.PrintEnemyData(BattleEnemyDataList);
    }

    /// <summary>
    /// 전투 시작시 UI에 파티 정보 세팅
    /// </summary>
    private void SetPartyData()
    {
        //BattlePlayerData = GameManager.Instance.GetPlayerData();

        for (int i = 0; i < 4; i++)
        {
            PartyMemberArray[i] = GameManager.Instance.GetPartyData(i); //전투 정보 저장용 배열에 파티의 정보를 받아옴

            if (PartyMemberArray[i].IsEmptyMember)
            {
                PartyUIScript.SetEmptyMember(i);
            }
            else
            {
                PartyUIScript.SetUIText(i, PartyMemberArray[i].Name, PartyMemberArray[i].MaxHP, PartyMemberArray[i].MaxMP);
                PartyUIScript.SetHPValue(i, PartyMemberArray[i].RemainHP);
                PartyUIScript.SetMPValue(i, PartyMemberArray[i].RemainMP);

                PartyMemberNumber++;    //활동 가능한 아군 인원 수 증가
            }
        }
    }
    #endregion

    #region CheckTurns

    //임시용
    private void SetPlayerTurn()
    {
        TurnUIScript.PlacePlayerTurn(PartyMemberNumber);
    }

    #endregion
}
