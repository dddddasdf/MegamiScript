using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region SetVariables
    [SerializeField] private GameObject EnemySpawner; //�� ���� ��� ��ũ��Ʈ
    private EnemyRespawnManager DemonSpawnManager;
    private Coroutine EnemyCoroutine;
    private PlayerDataManager PlayerDataScript;    //�÷��̾� ������ ���� �ִ� ��ũ��Ʈ
    private PartyMemberManager PartyManager;        //��Ƽ �����

    public SkillDatabaseManager SkillDBCaching;    //��ų DB ��ũ��Ʈ

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DemonSpawnManager = EnemySpawner.GetComponent<EnemyRespawnManager>();
            InitDataFiles();
            
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
    /// ���� ������ ������ �ʱ�ȭ ����
    /// </summary>
    private void InitDataFiles()
    {
        EnemyData.InitEnemyData();   //���� ������ �ʱ�ȭ
        HelpTextData.InitHelpTextData();    //���� �ؽ�Ʈ ������ �ʱ�ȭ
        PlayerDataScript = new PlayerDataManager();
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
        DemonSpawnManager.StopSpawn();  //���� �Ŵ������� ���� ���� ��û
        LoadSceneManager.Instance.SetBattleScene();

    }

    public void EndBattle()
    {
        LoadSceneManager.Instance.EndBattleScene();
        DemonSpawnManager.StartSpawn();
    }
    #endregion

    public PartyMemberManager ReturnPartyManager()
    {
        return PartyManager;
    }
}
