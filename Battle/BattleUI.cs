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
    /// ���� ���� ��� - �ӽÿ�
    /// </summary>
    public void PrintEnemyData(List<MonsterData> BattleEnemyDataList)
    {
        StringBuilder EnemyDataSB = new StringBuilder();    //������ ������ �޾ƾ� �ؽ�Ʈ

        for (int i = 0; i < BattleEnemyDataList.Count; i++)
        {
            EnemyDataSB.Append($"{BattleEnemyDataList[i].Name}\nü��: {BattleEnemyDataList[i].HP}\n");
        }

        TestText.text = EnemyDataSB.ToString();
    }

    /// <summary>
    /// ���� ���� ��� - �ӽÿ�
    /// </summary>
    /// <param name="BattlePlayerData"></param>
    public void PrintPlayerData(PartyMember BattlePlayerData)
    {
        StringBuilder PlayerSB = new StringBuilder();

        PlayerSB.Append($"{BattlePlayerData.Name}\nHP {BattlePlayerData.MaxHP}\nMP {BattlePlayerData.MaxMP}");

        TestPlayerText.text = PlayerSB.ToString();
    }
}
