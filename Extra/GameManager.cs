using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region SetVariables
    [SerializeField] private GameObject EnemySpawner; //적 생성 담당 스크립트
    private EnemyRespawnManager DemonSpawnManager;
    private Coroutine EnemyCoroutine;
    private PlayerData PlayerDataScript;    //플레이어 정보를 갖고 있는 스크립트

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DemonSpawnManager = EnemySpawner.GetComponent<EnemyRespawnManager>();
            InitDatas();
            
            return;
        }
        Destroy(gameObject);
    }

    private void Start()
    {
        DemonSpawnManager.FindSpawnPosition();
    }

    #region Init
    /// <summary>
    /// 게임 구동시 데이터 초기화 선언
    /// </summary>
    private void InitDatas()
    {
        EnemyData.InitEnemyData();   //몬스터 데이터 초기화
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
        DemonSpawnManager.StopSpawn();  //스폰 매니저에게 스폰 중지 요청
        LoadSceneManager.Instance.SetBattleScene();

    }

    public void EndBattle()
    {
        LoadSceneManager.Instance.EndBattleScene();
        DemonSpawnManager.StartSpawn();
    }
    #endregion

}
