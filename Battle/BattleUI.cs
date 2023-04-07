using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleUI : MonoBehaviour
{
    #region SetVariables
    [SerializeField] private GameObject BattleManagerObject;
    private BattleManager BattleManagerScript;

    [SerializeField] private Button Test;
    [SerializeField] private TextMeshProUGUI TestText;
    [SerializeField] private TextMeshProUGUI TestPlayerText;



    #endregion

    private void OnEnable()
    {
        //BattleManagerScript = BattleManagerObject.GetComponent<BattleManager>();
    }

    public void TestFn()
    {
        GameManager.Instance.EndBattle();
    }

    /// <summary>
    /// 몬스터 정보 출력 - 임시용
    /// </summary>
    public void PrintEnemyData(List<MonsterData> BattleEnemyDataList)
    {
        StringBuilder EnemyDataSB = new StringBuilder();    //몬스터의 정보를 받아쓸 텍스트

        for (int i = 0; i < BattleEnemyDataList.Count; i++)
        {
            EnemyDataSB.Append($"{BattleEnemyDataList[i].Name}\n체력: {BattleEnemyDataList[i].HP}\n");
        }

        TestText.text = EnemyDataSB.ToString();
    }

    /// <summary>
    /// 유저 정보 출력 - 임시용
    /// </summary>
    /// <param name="BattlePlayerData"></param>
    public void PrintPlayerData(PartyMember BattlePlayerData)
    {
        StringBuilder PlayerSB = new StringBuilder();

        PlayerSB.Append($"{BattlePlayerData.Name}\nHP {BattlePlayerData.MaxHP}\nMP {BattlePlayerData.MaxMP}");

        TestPlayerText.text = PlayerSB.ToString();
    }
}
