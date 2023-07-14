using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

//���� �Ǹ� ��ȣ �������� ���߿� �� ���� �Ѵ�(ȥ�� ����)
//��Ʈ����Ʈ �ѹ��� ��Ƽ �ѹ��� �� ���� ��������(��Ʈ ��������Ʈ�� �״�� �д�. ���� ���������� ������� �����Ƿ�)


/// <summary>
/// ������ �� �ý���
/// </summary>
[Flags]
public enum PressTurn
{
    None = 0, 
    ReduceAllTurn = 1 << 0,      //�����ִ� ��� �� �Ҹ�
    /*
    1. ���濡�� �ݻ� �Ǵ� ����� �Ӽ����� �������� ���
    2. ���ָ� �������� ���
    3. ��ȭ���� ������ ȭ���� ���� ���

    �����ִ� ���� ��� �Ҹ��ϰ� ��� ��� ������� ��ü
    */

    ReduceTwoTurn = 1 << 1,      //2�� �Ҹ�
    /*
    1. ���濡�� ��ȿ�� �Ӽ����� �������� ���
    2. ������ ������ ȸ������ ��� (�ϸ�, �ֻ��� ��簡 ������ �̽��� ����)
    3. ���� ���� �����ϰ� ���� �׳� ������ ���
    
    ������ ���� �켱������ ���� 2���� �Ҹ�
    1. 2���� ���� ���� �Ҹ�
    2. ���� ���� ������� ���� ���, 1���� ���� �ϰ� 1���� ������ �� ���� �Ҹ�
    3. ���� ���� �������� ���� ��� 2���� ������ �� ���� �Ҹ�
    */

    ReduceHalfTurn = 1 << 2,     //���� �� �Ҹ�
    /*
    1. ������ �Ϲ������� �������� ���(����, ����)
    2. ���� ���� Ÿ��
    
    ������ ���� �켱������ ���� �������� �Ҹ�
    1. 1���� ���� ���� �Ҹ�
    2. ���� ���� �������� ���� ���, 1���� ������ �� ���� �Ҹ�
    */

    ReduceHalfTurn_Party = 1 << 3,   //���� �� �Ҹ�(�÷��̾� ������� �Ͻ�ŵ �� ü���� �ൿ ����)
    /*
    1. ���� �Ʊ����� ���ʸ� �Ѱ��� ���
    2. �Ʊ� ��ȯ, ���� �Ǵ� ��ü�� ���� ���

    ������ ���� �켱������ ���� �������� �Ҹ�
    1. 1���� ���� ���� �Ҹ�
    2. ���� ���� �������� ���� ���, 1���� ������ �� ���� ���� ������ ����
    */

    ChangeHalfTurn = 1 << 4,     //������ �� ���� ���� ������ ����
    /*
    1. ũ��Ƽ���� �߻��ϰų� ������ ������ �Ӽ����� �������� ���
    2. ��ī��Ʈ���� ũ��Ƽ�� ��ī��Ʈ�� �ߵ��� ���(����� ������ �� �ֵ��� �ùٸ��� ����� ���)

    ������ ���� �켱������ ���� �� �Ҹ�
    1. 1���� ������ �� ���� ���� ������ ����
    2. ������ �� ���� �������� ���� ���, 1���� ���� �� �Ҹ�
    */
}



public class BattleManager : MonoBehaviour, IPartyObserver
{
    #region SetField

    /// <summary>
    /// �׽�Ʈ�� �ӽ� ����
    /// </summary>
    private PartyMemberManager TestPMM = new PartyMemberManager();
    private EnemyDatabaseManager TestEDM = new EnemyDatabaseManager();
    private SkillSystem SkillCalculator = new SkillSystem();


    private List<MonsterData> BattleEnemyDataList;

    [SerializeField] private BattleUIManager UIScript;

    
    private int PartyMemberNumber = 0;  //���� �������� Ȱ�� ������ �Ʊ� �ο� ��

    private int NumberOfPartyWholeTurn;     //��Ƽ �Ͽ��� �����ִ� ������ �� �� ��
    private int NumberOfPartyHalfTurn;      //��Ƽ �Ͽ��� �����ִ� ���� �� ��
    private int NumberOfEnemyWholeTurn;     //�� �Ͽ��� �����ִ� ������ �� �� ��
    private int NumberOfEnemyHalfTurn;      //�� �Ͽ��� �����ִ� ���� �� ��

    private int NumberOfWholeTurn = 0;          //������ �� �� ��
    private int NumberOfHalfTurn = 0;           //���� �� ��
    private int NumberOfUsedTurn = 0;           //�Ҹ��� �� ��ũ ��

    private bool IsPlayerPhase;              //�÷��̾� ������ Ȯ�ο�: true == �÷��̾� ������/false == �� ������
    private OnBattlePartyObject NowTurnCharacterOfParty;   //�÷��̾� ������ �� ���� �ൿ ���� �Ʊ�

#nullable enable
    private List<OnBattlePartyObject?> NowOnBattlePartyList = new List<OnBattlePartyObject?>(); //���� ������ ���� ���� ��Ʈ�� ����Ʈ
#nullable disable
    private List<OnBattlePartyObject> PartyTurnOrderList = new List<OnBattlePartyObject>();     //�ൿ�� ������ ����Ʈ<-���Ž�. ���� ���� ������ ������ �Ѵ�
    private List<int> PartyTurnOrderIndexList = new List<int>();     //�ൿ�� ������ �ε��� ����Ʈ
    private int NumberOfOnBattleParty = 0;           //���� �����ؼ� ���� ���� ��Ƽ�� ��

#nullable enable
    private List<OnBattleEnemyObject?> NowOnBattleEnemyList = new List<OnBattleEnemyObject?>();     //���� ���� ���� ��Ƽ
#nullable disable
    private List<int> EnemyTurnOrderList = new List<int>();
    private int NumberOfOnBattleEnemy = 0;          //���� �����ؼ� ���� ���� ���� ��

    private const int MaxNumberOfEntry = 4;       //�� ���� ��Ʈ���� ������ ��� ���� ��-�ʱ�ȭ�� �� ũ�� ������
    /*
    �Ʊ� �ൿ���� ����: ��Ʈ���� ���(����)�� �������� ���� �ൿ�Ѵ� (����5�� �ý��۰� ����)
    
    �� �ൿ���� ����: ���� �ӵ��� ���� ���� ����(�ӵ��� ���� ��� ����) 
    */

    private OnBattlePartyObject NowSelectedSwapFrom;     //�Ǹ��� ��ü�� �� ��Ʈ������ ��ü ��� �Ʊ� �����͸� ��� ����
    private PartyDemonData NowSelectedSwapTo;       //�Ǹ��� ��ü�� �� ���忡�� ��ü�� �Ʊ� �����͸� ��� ����

    private Queue<Action> JobQueue = new Queue<Action>();      //���������� �����ų �۾� ���
    
    public void AddJobQueueMethod(Action Method)
    {
        JobQueue.Enqueue(Method);
    }

    private Stack<NowOnMenu> OnMenuStack = new ();
    private NowOnMenu PopMenu;

    public static SkillCellData SelectedSkillDataBuffer = new();    //���õ� ��ų ���� �����
    private SkillTypeSort SelectedSkillTypeCaching;     //���� ���õ� ��ų�� Ÿ�� ĳ�̿�

    #endregion

    private void Awake()
    {
        //BattleEnemyDataList = new List<MonsterData>();

        //PartyMemberArray = new PartyMember[4];  //��Ƽ ���� �迭 �ʱ�ȭ
        NowOnBattlePartyList.Capacity = MaxNumberOfEntry;       //���ʿ��� ���Ҵ��� �Ͼ�� �ʵ��� �̸� ũ�� ����
        PartyTurnOrderList.Capacity = MaxNumberOfEntry;     //���ʿ��� ���Ҵ��� �Ͼ�� �ʵ��� �̸� ũ�� ����
        SelectedSkillDataBuffer = null;     //��ų ������ ���ۿ� ���� ���� ����ֱ�

        TestPMM.LoadTMP();
        TestEDM.InitEnemyDatabaseManager();
    }

    private void Start()
    {
        //GetEnemyData();
        //SetPartyData();
        //SetPlayerTurn();

        //�Ʒ��� ���� ���⼭ ��Ƽ ����Ʈ �ʱ�ȭ�� ���ϰ� �ֱ� ������ �۾�ť�� ���µ�, ���������� ����� �ʿ䰡 ���� (�� ������ ���� �׽�Ʈ�� ������ ť �̿� ������)
        TestPMM.AddJobQueueMethod(() => SetOnBattlePartyData());
        //AddJobQueueMethod(() => UIScript.InitPartyDisplay(NowOnBattlePartyList));
        //TestPMM.AddJobQueueMethod(() => UIScript.InitPartyDisplay(NowOnBattlePartyList));
        TestPMM.AddJobQueueMethod(() => SetPlayerPhase());
        TestEDM.AddJobQueueMethod(() => SetOnBattleEnemyData());
        TestEDM.AddJobQueueMethod(() => UIScript.InitEnemyisplay(NowOnBattleEnemyList));
    }


    private void OnDisable()
    {
        //GameManager.Instance.ReturnPartyManager().RemoveObserver(this);     //��Ƽ�Ŵ����� ������ ����Ʈ���� ����
        SelectedSkillDataBuffer = null;     //���� ��ü�� �� ���ۿ� ���� ���� �ʱ�ȭ
    }

    /// <summary>
    /// ��Ƽ �Ŵ������Լ� ���� �ȳ� ���� - ������ ����
    /// </summary>
    public void UpdateEntry()
    {

    }


    #region SetBattleInfo
    private void SetOnBattleEnemyData()
    {
        //EnemyDatabaseManager EnemyDataBase = new EnemyDatabaseManager();
        //List<int> EnemyIndexList = new List<int>();
        //EnemyIndexList = EnemyDataBase.ReturnList();
        //
        //for (int i = 0; i < EnemyIndexList.Count; i++)
        //{
        //    BattleEnemyDataList.Add(EnemyDataBase.ReturnEnemyData(EnemyIndexList[i]));  //�ε����� ���� ���������� ������ ������ ������ �����͸� ������
        //}
        //EnemyDataBase.ClearList();  //�ӽÿ� �ε��� ����Ʈ Ŭ����
        //
        //BattleUIScript.PrintEnemyData(BattleEnemyDataList);

        //�Ʒ��� ������ ���� �׽�Ʈ ���� ���� ����
        List<int> VirtualEnemyIndexList = new List<int>();
        VirtualEnemyIndexList.Add(2);
        VirtualEnemyIndexList.Add(1);
        VirtualEnemyIndexList.Add(2);
        VirtualEnemyIndexList.Add(1);

        for (int i = 0; i < VirtualEnemyIndexList.Count; i++)
        {
            OnBattleEnemyObject Tmp = new OnBattleEnemyObject(TestEDM.ReturnEnemyData(VirtualEnemyIndexList[i]));
            NowOnBattleEnemyList.Add(Tmp);
        }
    }

    /// <summary>
    /// ���� ���۽� ���� ��Ƽ ���� ����
    /// </summary>
    private void SetOnBattlePartyData()
    {
        //�÷��̾� �����͸� ���� ���� ��Ʈ���� ù��° ����� �ֱ�
        OnBattlePartyObject PlayerCharacterBattleData = new OnBattlePartyObject();

        //PlayerCharacterBattleData.SetMemberData(GameManager.Instance.ReturnPartyManager().ReturnPlayerCharacterData());
        //PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        //NowOnBattleEntryList.Add(PlayerCharacterBattleData);

        //�׽�Ʈ�� ����. �Ʒ� ���� ��
        PlayerCharacterBattleData.SetMemberData(TestPMM.ReturnPlayerCharacterData());
        PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        NowOnBattlePartyList.Add(PlayerCharacterBattleData);
        NumberOfOnBattleParty++;

        //���� �Ǹ� ����Ʈ ��������
        //for (int i = 0; i < 3; i++)
        //{
        //    if (GameManager.Instance.ReturnPartyManager().ReturnEntryDemonData(i) != null)
        //    {
        //        OnBattleObject PartyDemonBattleData = new OnBattleObject();
        //        PartyDemonBattleData.SetMemberData(GameManager.Instance.ReturnPartyManager().ReturnEntryDemonData(i));
        //        PartyDemonBattleData.SetIsPlayerCharacter(false);
        //        NowOnBattleEntryList.Add(PartyDemonBattleData);
        //    }
        //    else
        //    {
        //        NowOnBattleEntryList.Add(null);
        //    }
        //}

        //�׽�Ʈ��. �ʵ�� �����ų ���� �� �κ��� ����� �� ������ Ȱ��ȭ��Ų��
        for (int i = 0; i < 3; i++)
        {
            if (TestPMM.ReturnEntryDemonData(i) != null)
            {
                OnBattlePartyObject PartyDemonBattleData = new OnBattlePartyObject();
                PartyDemonBattleData.SetMemberData(TestPMM.ReturnEntryDemonData(i));
                PartyDemonBattleData.SetIsPlayerCharacter(false);
                NowOnBattlePartyList.Add(PartyDemonBattleData);
                NumberOfOnBattleParty++;
            }
            else
            {
                NowOnBattlePartyList.Add(null);
            }
        }

        UIScript.InitPartyDisplay(NowOnBattlePartyList);


        //BattlePlayerData = GameManager.Instance.GetPlayerData();

    }
    #endregion


    #region Skill

    private bool IsChangedSkillCaching = false;
    private SkillDataRec NowSelectedSkill;

    /// <summary>
    /// ���� �ൿ���� ĳ������ ��ų ������ �����ؼ� UI�� ǥ���� �� �ְ� �Ѵ�
    /// </summary>
    private void CallSetSkillList()
    {
        //SkillUIScript.AddJobQueueMethod(() => CallShowAffinityMark());
        UIScript.SetSkillList(NowOnBattlePartyList[PartyTurnOrderIndexList[0]]);
    }


    /// <summary>
    /// ��ų ����â���� ���� ��ư�� Ȱ��ȭ �� ��ų�� ���� ������ ��ũ ǥ���϶�� �ϴ� �Լ�
    /// </summary>
    public void NowActivatedSkillMark()
    {
        if (!UIScript.ReturnIsButtonDoubleClicked())
        {
            return;         //���� ��ų ��ư�� ����Ŭ���� �� �ƴϸ� ���� �ܰ�� �Ѿ�� �ʴ´�
        }

        SelectedSkillTypeCaching = SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnSkillType();

        if (SelectedSkillTypeCaching == SkillTypeSort.Recover || SelectedSkillTypeCaching == SkillTypeSort.Support)
        {
            //ȸ�� �Ǵ� ����Ʈ �迭: �Ʊ� Ÿ���� UI
        }
        else
        {
            //�� �� �� ���� �迭: �� Ÿ���� UI
            UIScript.ShowEnemyTargetSelectUI();     //�� ��ų�� �������� ����ִ� ȭ�� ȣ��
        }

        
        UIScript.ResetIsButtonChanged();        //���� Ŭ�� ������ ���� ���󺹱�
    }

    /// <summary>
    /// ���� �� ��ų �ߵ�
    /// </summary>
    public void ClickSelectedSkill()
    {
        switch (SelectedSkillTypeCaching)
        {
            case SkillTypeSort.Physical:
            case SkillTypeSort.Gun:
                ActivateAttackSkil(new CalculateSkillDamage(SkillCalculator.CalculatePhysicGunDamage));     //��������Ʈ�� ��ų �ý����� ����/�� ��� �޼ҵ� ����
                break;
            case SkillTypeSort.Fire:
            case SkillTypeSort.Ice:
            case SkillTypeSort.Electric:
            case SkillTypeSort.Force:
            case SkillTypeSort.Light:
            case SkillTypeSort.Dark:
            case SkillTypeSort.Almighty:
                ActivateAttackSkil(new CalculateSkillDamage(SkillCalculator.CalculateMagicDamage));     //��������Ʈ�� ��ų �ý����� ���� ��� �޼ҵ� ����
                break;
            case SkillTypeSort.Recover:
            case SkillTypeSort.Support:
            case SkillTypeSort.Ailment:
                break;
            default:
                return;
        }
    }

    /// <summary>
    /// ��ų �ý��� ���� ��ų ����� ��� �޼ҵ� ���޿� ��������Ʈ
    /// </summary>
    /// <param name="UsedSkill"></param>
    /// <param name="SkillUser"></param>
    /// <param name="Target"></param>
    /// <param name="SkillDamage"></param>
    private delegate void CalculateSkillDamage(SkillDataRec UsedSkill, IOnBattleObject SkillUser, IOnBattleObject Target, out int SkillDamage);

    private void ActivateAttackSkil(CalculateSkillDamage SkillSystemMethod)
    {
        int CalculatedDamage;       //���� �����
        int numberOfHit = SetNumberOfHit();       //���� ��Ʈ���� ���Ѵ�
        PressTurn reduceTurnFlag = PressTurn.None;       //������ ����ġ�� �� ������ ���� ����ϴ� ����
        SkillDataRec NowUsingSkill = SelectedSkillDataBuffer.ReturnSkillDataRec();      //���� ���� ��ų�� ����

        if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
        {
            //�̱� Ÿ�� ��ų: ���� Ÿ�ٵ� ���� ������ �ʿ�� �Ѵ�
            for (int i = 0; i < numberOfHit; i++)
            {
                //���� Ƚ����ŭ �ݺ���
                SkillSystemMethod(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], UIScript.ReturnTargetedEnemyData(), out CalculatedDamage);
                SavePressTurnFlag(UIScript.ReturnTargetedEnemyData().ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
            }
        }
        else if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.All)
        {
            //��ü Ÿ�� ��ų: ��ü Ÿ�� ����� ���� Ƚ�� 1�� ����
            for (int i = 0; i < NowOnBattleEnemyList.Count; i++)
            {
                //���� ���� ���� �� ��ü����ŭ ���� �ݺ�
                SkillSystemMethod(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[i], out CalculatedDamage);
                SavePressTurnFlag(NowOnBattleEnemyList[i].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
            }
        }
        else
        {
            //�ٴܱ�(��üX ��Ʈ���� ��� ����)
            int randomIndex;        //���� ��� �ε����� ���� ����

            //���� Ƚ����ŭ �ݺ���
            for (int i = 0; i < numberOfHit; i++)
            {
                randomIndex = UnityEngine.Random.Range(0, NowOnBattleEnemyList.Count - 1);
                SkillSystemMethod(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[randomIndex], out CalculatedDamage);
                SavePressTurnFlag(NowOnBattleEnemyList[randomIndex].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
            }
        }
        EndAttackTurn(reduceTurnFlag);    //���� ���μ����� ��ġ�� �ش� ĳ������ �� ����
    }

    /// <summary>
    /// ���� ��ų���� ���� Ÿ�� ����
    /// </summary>
    /// <returns></returns>
    private int SetNumberOfHit()
    {
        int minHit = SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnMinHit();
        int maxHit = SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnMaxHit();
        if (minHit == maxHit)
        {
            //�ִ� ��Ʈ���� �ּ� ��Ʈ���� ���ٸ� ���� ���� ���� �ʿ� ���� �ٷ� ��ȯ
            return SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnMaxHit();
        }
        else
        {
            //�ƴ϶�� ������ ��Ʈ���� ��� ��ȯ
            return UnityEngine.Random.Range(minHit, maxHit);
        }
    }

    /// <summary>
    /// ����� ��ų�� ȿ���� ���� �÷��� ����
    /// </summary>
    /// <param name="skillAffinityOfUsedSkill"></param>
    /// <param name="reduceTurnFlag"></param>
    private void SavePressTurnFlag(SkillAffinities? skillAffinityOfUsedSkill, ref PressTurn reduceTurnFlag)
    {
        switch (skillAffinityOfUsedSkill)
        {
            case SkillAffinities.Weak:
                reduceTurnFlag |= PressTurn.ChangeHalfTurn;     //����
                break;
            case SkillAffinities.Void:
                reduceTurnFlag |= PressTurn.ReduceTwoTurn;      //��ȿ
                break;
            case SkillAffinities.Reflect:
            case SkillAffinities.Drain:
                reduceTurnFlag |= PressTurn.ReduceAllTurn;      //�ݻ�, ���
                break;
            case SkillAffinities.Resist:
            default:
                reduceTurnFlag |= PressTurn.ReduceHalfTurn;     //����, �ƹ��͵� ����
                break;
        }
    }

    /// <summary>
    /// ������ �÷��׸� �������� ���� �� �� ���� ����
    /// </summary>
    /// <param name="reduceTurnFlag"></param>
    private void EndAttackTurn(PressTurn reduceTurnFlag)
    {
        //������ ����ģ �� ������ �÷��װ� ���� �� ���� �켱���������� if ���� �ɾ�д�->�켱������ ���� �� ������ ��� �ش� ��Ģ�� ���� �� ����
        PressTurn toReduceTurn = PressTurn.None;

        if (reduceTurnFlag.HasFlag(PressTurn.ReduceAllTurn))
        {
            toReduceTurn = PressTurn.ReduceAllTurn;
        }
        else if (reduceTurnFlag.HasFlag(PressTurn.ReduceTwoTurn))
        {
            toReduceTurn = PressTurn.ReduceAllTurn;
        }
        else if (reduceTurnFlag.HasFlag(PressTurn.ChangeHalfTurn))
        {
            toReduceTurn = PressTurn.ChangeHalfTurn;
        }
        else if (reduceTurnFlag.HasFlag(PressTurn.ReduceHalfTurn))
        {
            toReduceTurn = PressTurn.ReduceHalfTurn;
        }

        ReduceTurn(toReduceTurn);
    }


    #endregion


    /// <summary>
    /// ���� �Ǹ��� ����� ��� ��Ʈ������ ���ŵǰ� �������� ���ư���
    /// </summary>
    public void RemoveInEntry(OnBattlePartyObject Target)
    {
        Target.ReturnMemberData();
        NumberOfOnBattleParty--;
    }

    /// <summary>
    /// ���忡 �ִ� �Ǹ��� ��Ʈ���� �ִ� ���� �Ǹ� ��ü
    /// </summary>
    public void ChangeEntry()
    {
        //��ü UI ��� ����
    }

    public void NowSelectedEntryTarget()
    {

    }


    #region CheckTurns

    /// <summary>
    /// �÷��̾� ������ ��ȯ
    /// </summary>
    private void SetPlayerPhase()
    {
        UIScript.PlacePlayerTurn(PartyMemberNumber);
        IsPlayerPhase = true;

        if (PartyTurnOrderList.Count != 0)
            PartyTurnOrderList.Clear();          //���� �ൿ���� ����Ʈ�� ������� �ʴٸ� �� �� ����ش�

        if (PartyTurnOrderIndexList.Count != 0)
            PartyTurnOrderIndexList.Clear();

        //SortPartyTurn();        //�ӵ��� ���� �ൿ���� ������ �Ѵ�
        FillNumberOfTurn();
        SetPartyTurnOrder();
        
    }

    /// <summary>
    /// ������ ��ȯ�� �ൿ�ϼ��� �˸°� ä���ִ� �Լ�
    /// </summary>
    private void FillNumberOfTurn()
    {
        if (IsPlayerPhase)
        {
            NumberOfWholeTurn = NumberOfOnBattleParty;           //ä������ ������ �� �� = ���� ���� ���� ��Ƽ �����
            NumberOfHalfTurn = 0;                           //���� �� �� 0���� �ʱ�ȭ
            UIScript.PlacePlayerTurn(NumberOfOnBattleParty);         //���� ���� ��Ƽ �������ŭ �� ��ũ ǥ��
        }
        else
        {
            //���� ��Ƽ���� ���� ���� ���� �������ŭ ������ �� �� ���� ä���ش�
            //�� ���ڸ�ŭ ��Ʋ UI �����ڿ��� �� �������� ä���� �����Ѵ�
            //Ư�� ������ ���� ���ܰ� ������ ������ �װͿ� ���� �Ű澲�� �ʴ´�
        }

    }

    /// <summary>
    /// ���� ���� ��Ƽ���� �� ��⿭ ����
    /// </summary>
    private void SetPartyTurnOrder()
    {
        //�� ���� ��⿭�� ����ִٸ� �ӵ��� ���� �� ���� �����ؼ� ����Ʈ�� ä���ش�
        if (PartyTurnOrderIndexList.Count == 0)
        {
            for (int i = 0; i < NowOnBattlePartyList.Count; i++)
            {
                if (NowOnBattlePartyList[i] != null)
                {
                    PartyTurnOrderList.Add(NowOnBattlePartyList[i]);     //�ൿ���� ����Ʈ�� ���� ��Ʈ�� �� ����� Add�� �־��ش�
                    PartyTurnOrderIndexList.Add(i);
                }
            }

            PartyTurnOrderList.Sort((Character1, Character2) => Character1.ReturnMemberData().ReturnAg().CompareTo(Character1.ReturnMemberData().ReturnAg()));
            PartyTurnOrderIndexList.Sort((n1, n2) => NowOnBattlePartyList[n2].ReturnMemberData().ReturnAg().CompareTo(NowOnBattlePartyList[n1].ReturnMemberData().ReturnAg()));
        }

        UIScript.ResetIsButtonChanged();
        if (NowOnBattlePartyList[PartyTurnOrderIndexList[0]].ReturnIsPlayerCharacter())
            UIScript.EnableActButtonPlayerTurn();       //�÷��̾� ���̸� ��Ȱ��ȭ �� ��ư�� Ȱ��ȭ
        else
            UIScript.DisableActButtonPartyTurn();       //�� �� ��Ƽ�� ���̸� �Ϻ� ��ư ��Ȱ��ȭ

        UIScript.ActiveTurn(PartyTurnOrderIndexList[0]);        //���� �ൿ�ϰ� �Ǵ� ĳ������ �� Ȱ��ȭ ǥ��
        CallSetSkillList();
    }

    
    /// <summary>
    /// �ӵ��� ���� ��Ƽ �ൿ������ �����ϴ� �Լ�
    /// </summary>
    private void SortPartyTurn()
    {
        for (int i = 0; i < NowOnBattlePartyList.Count; i++)
        {
            if (NowOnBattlePartyList[i] != null)
            {
                PartyTurnOrderList.Add(NowOnBattlePartyList[i]);     //�ൿ���� ����Ʈ�� ���� ��Ʈ�� �� ����� Add�� �־��ش�
                PartyTurnOrderIndexList.Add(i);
                
            }
        }

        PartyTurnOrderList.Sort((Character1, Character2) => Character1.ReturnMemberData().ReturnAg().CompareTo(Character1.ReturnMemberData().ReturnAg()));
        PartyTurnOrderIndexList.Sort((n1, n2) => NowOnBattlePartyList[n1].ReturnMemberData().ReturnAg().CompareTo(NowOnBattlePartyList[n2].ReturnMemberData().ReturnAg()));
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
                {
                    NumberOfWholeTurn = 0;
                    NumberOfHalfTurn = 0;
                    //������ ���� �� ��� ������ ��ȯ �Լ� ����
                    //�� ���� �� ������ ��ȯ ���� �Լ� ����
                    //�����غ��� �����͵� ���� ���� ���� 0 ���ϰ� �Ǹ� ������ �Ѿ���� �׼� ���� �ɾ�θ� �ǳ�?

                    UIScript.HideAllMark(IsPlayerPhase);        //��� ���� �� ���߱�

                    //if (IsPlayerPhase)
                    //    TurnUIScript.HideAllPlayerMark();
                    //else
                    //    TurnUIScript.HideAllEnemyMark();
                }
                break;
            case PressTurn.ReduceTwoTurn:
                {
                    if (NumberOfHalfTurn >= 2)
                    {
                        //������ �� 2�� ����
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 2, IsPlayerPhase);       //���� �� 2�� ���߱�
                        NumberOfHalfTurn -= 2;
                    }
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn >= 1)
                    {
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //������ �� 1�� ���߱�
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 2;
                    }
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn == 0)
                    {
                        //���� �ൿ �ϼ��� ���� �� 1���� �����ִٸ� 2�� ������ ��ǻ� ���� ������ ��������
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;
                    }
                    else if (NumberOfWholeTurn == 1)
                    {
                        //���� �ൿ �ϼ��� ������ �� 1���� �����ִٸ� 2�� ������ ��ǻ� ���� ������ ��������
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //������ �� 1�� ���߱�
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 2, IsPlayerPhase);        //������ �� 2�� ���߱�
                        NumberOfWholeTurn -= 2;
                        NumberOfUsedTurn += 2;
                    }
                }
                break;
            case PressTurn.ReduceHalfTurn:
                {
                    if (NumberOfHalfTurn >= 1)
                    {
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //������ �� 1�� ���߱�
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                }
                break;
            case PressTurn.ReduceHalfTurn_Party:
                {
                    if (NumberOfHalfTurn >= 1)
                    {
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;      //�����ִ� ���� �� 1�� ����
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        if (IsPlayerPhase)
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleParty);       //���� �� 1�� �߰�
                        else
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleEnemy);       //���� �� 1�� �߰�
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }    
                }
                break;
            case PressTurn.ChangeHalfTurn:
                {
                    if (NumberOfWholeTurn == 0 && NumberOfHalfTurn != 0)
                    {
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);
                        NumberOfHalfTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        if (IsPlayerPhase)
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleParty);       //���� �� 1�� �߰�
                        else
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleEnemy);       //���� �� 1�� �߰�
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }
                }
                break;
        }

        if (NumberOfWholeTurn <= 0 && NumberOfHalfTurn <= 0)
        {
            IsPlayerPhase = !IsPlayerPhase;   //��� ���� �ൿ�� ���� 0�� �Ǿ��� ��� ���� ������� �Ѿ��
            //������ ��ȯ �Լ� ȣ��
            NumberOfUsedTurn = 0;
        }
        else
        {
            if (IsPlayerPhase)
            {
                UIScript.DeactiveTurn(PartyTurnOrderIndexList[0]);      //�ൿ���� ��ģ ĳ������ �� �Ϸ� ǥ��
                PartyTurnOrderIndexList.RemoveAt(0);           //�ൿ���� ��ģ ĳ���ʹ� �� ��⿭���� ������
                SetPartyTurnOrder();
                UIScript.ReturnActMenu();
            }
            else
            {

            }
        }
    }


    /// <summary>
    /// �Ͻ�ŵ ��ư�� ����
    /// </summary>
    public void PressTrunSkip()
    {
        if (!UIScript.ReturnIsButtonDoubleClicked())
        {
            return;     //���� ��ư�� �ٽ� ���� �� �ƴ϶�� ���� �ൿ ������ �ʿ�X
        }

        ReduceTurn(PressTurn.ReduceHalfTurn_Party);
    }

    //�Ʒ��� �� �ѱ�� ���� üũ�� �Լ��� - ���� ����
    public void TestOne()
    {
        ReduceTurn(PressTurn.ReduceAllTurn);
    }

    public void TestTwo()
    {
        ReduceTurn(PressTurn.ReduceTwoTurn);
    }

    public void TestThree()
    {
        ReduceTurn(PressTurn.ReduceHalfTurn);
    }

    public void TestFour()
    {
        ReduceTurn(PressTurn.ReduceHalfTurn_Party);
    }

    public void TestFive()
    {
        ReduceTurn(PressTurn.ChangeHalfTurn);
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

#region Dummy
/// <summary>
/// ����� ���ݽ�ų�� ���� ��ų�� ���
/// </summary>
//private void ActivatePhysicSkill()
//{
//    int CalculatedDamage;       //���� �����
//    int numberOfHit = SetNumberOfHit();       //���� ��Ʈ���� ���Ѵ�
//    PressTurn reduceTurnFlag = PressTurn.None;       //������ ����ġ�� �� ������ ���� ����ϴ� ����
//    SkillDataRec NowUsingSkill = SelectedSkillDataBuffer.ReturnSkillDataRec();      //���� ���� ��ų�� ����

//    if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
//    {
//        //�̱� Ÿ�� ��ų: ���� Ÿ�ٵ� ���� ������ �ʿ�� �Ѵ�
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            //���� Ƚ����ŭ �ݺ���
//            SkillCalculator.CalculatePhysicGunDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], UIScript.ReturnTargetedEnemyData(), out CalculatedDamage);
//            SavePressTurnFlag(UIScript.ReturnTargetedEnemyData().ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.All)
//    {
//        //��ü Ÿ�� ��ų: ��ü Ÿ�� ����� ���� Ƚ�� 1�� ����
//        for (int i = 0; i < NowOnBattleEnemyList.Count; i++)
//        {
//            //���� ���� ���� �� ��ü����ŭ ���� �ݺ�
//            SkillCalculator.CalculatePhysicGunDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[i], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[i].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else
//    {
//        //�ٴܱ�(��üX ��Ʈ���� ��� ����)
//        int randomIndex;        //���� ��� �ε����� ���� ����

//        //���� Ƚ����ŭ �ݺ���
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            randomIndex = UnityEngine.Random.Range(0, NowOnBattleEnemyList.Count - 1);
//            SkillCalculator.CalculatePhysicGunDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[randomIndex], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[randomIndex].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    EndAttackTurn(reduceTurnFlag);    //���� ���μ����� ��ġ�� �ش� ĳ������ �� ����
//}


///// <summary>
///// ����� ���ݽ�ų�� ���� ��ų�� ���
///// </summary>
//private void ActivateMagicSkill()
//{
//    int CalculatedDamage;       //���� �����
//    int numberOfHit = SetNumberOfHit();       //���� ��Ʈ���� ���Ѵ�
//    PressTurn reduceTurnFlag = PressTurn.None;       //������ ����ġ�� �� ������ ���� ����ϴ� ����
//    SkillDataRec NowUsingSkill = SelectedSkillDataBuffer.ReturnSkillDataRec();      //���� ���� ��ų�� ����

//    if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
//    {
//        //�̱� Ÿ�� ��ų: ���� Ÿ�ٵ� ���� ������ �ʿ�� �Ѵ�
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            //���� Ƚ����ŭ �ݺ���
//            SkillCalculator.CalculateMagicDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], UIScript.ReturnTargetedEnemyData(), out CalculatedDamage);
//            SavePressTurnFlag(UIScript.ReturnTargetedEnemyData().ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.All)
//    {
//        //��ü Ÿ�� ��ų: ��ü Ÿ�� ����� ���� Ƚ�� 1�� ����
//        for (int i = 0; i < NowOnBattleEnemyList.Count; i++)
//        {
//            //���� ���� ���� �� ��ü����ŭ ���� �ݺ�
//            SkillCalculator.CalculateMagicDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[i], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[i].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else
//    {
//        //�ٴܱ�(��üX ��Ʈ���� ��� ����)
//        int randomIndex;        //���� ��� �ε����� ���� ����

//        //���� Ƚ����ŭ �ݺ���
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            randomIndex = UnityEngine.Random.Range(0, NowOnBattleEnemyList.Count - 1);
//            SkillCalculator.CalculateMagicDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[randomIndex], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[randomIndex].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    EndAttackTurn(reduceTurnFlag);    //���� ���μ����� ��ġ�� �ش� ĳ������ �� ����
//}
#endregion
