using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region SetVariables
    [SerializeField] private GameObject EnemySpawner; //�� ���� ��� ��ũ��Ʈ
    private EnemyRespawnManager EnemySpawnManager;
    private Coroutine EnemyCoroutine;
    private PlayerData PlayerDataScript;    //�÷��̾� ������ ���� �ִ� ��ũ��Ʈ

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            EnemySpawnManager = EnemySpawner.GetComponent<EnemyRespawnManager>();
            InitDatas();
            
            return;
        }
        Destroy(gameObject);
    }

    private void Start()
    {
        EnemySpawnManager.FindSpawnPosition();
    }

    #region Init
    /// <summary>
    /// ���� ������ ������ �ʱ�ȭ ����
    /// </summary>
    private void InitDatas()
    {
        EnemyData.InitEnemyData();   //���� ������ �ʱ�ȭ
        PlayerDataScript = new PlayerData();
        PlayerDataScript.SetPlayerTmp();
    }

    #endregion

    #region Player

    public PartyMember GetPlayerData()
    {
        return PlayerDataScript.ReturnPlayerData();
    }
    
    public PartyMember GetPartyData(int MemberNumber)
    {
        return PlayerDataScript.ReturnPartyData(MemberNumber);
    }

    #endregion

    #region Spawn


    #endregion


    #region Battle
    public void CallBattle()
    {
        LoadSceneManager.Instance.SetBattleScene();
    }

    public void EndBattle()
    {
        LoadSceneManager.Instance.EndBattleScene();
    }
    #endregion

}
