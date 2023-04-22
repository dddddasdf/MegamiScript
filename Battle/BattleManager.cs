using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class BattleManager : MonoBehaviour, IPartyObserver
{
    #region SetVariables

    /// <summary>
    /// �׽�Ʈ�� �ӽ� ����
    /// </summary>
    private PartyMemberManager TestPMM = new PartyMemberManager();

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

    private int NumberOfWholeTurn = 0;          //������ �� �� ��
    private int NumberOfHalfTurn = 0;           //���� �� ��
    private int NumberOfUsedTurn = 0;           //�Ҹ��� �� ��ũ ��

    private bool IsPlayerPhase;              //�÷��̾� ������ Ȯ�ο�: true == �÷��̾� ������/false == �� ������
    private OnBattleObject NowTurnCharacterOfParty;   //�÷��̾� ������ �� ���� �ൿ ���� �Ʊ�

#nullable enable
    private List<OnBattleObject?> NowOnBattleEntryList = new List<OnBattleObject?>(); //���� ������ ���� ���� ��Ʈ�� ����Ʈ
#nullable disable
    private List<OnBattleObject> TurnOrderList = new List<OnBattleObject>();     //�ൿ�� ������ ����Ʈ
    private List<int> TurnOrderIndexList = new List<int>();     //�ൿ�� ������ �ε��� ����Ʈ
    private int NumberOfOnBattleParty = 0;           //���� �����ؼ� ���� ���� ��Ƽ�� ��

    private int NumberOfOnBattleEnemy = 0;          //���� �����ؼ� ���� ���� ���� ��

    private int MaxNumberOfEntry = 4;       //�� ���� ��Ʈ���� ������ ��� ���� ��-�ʱ�ȭ�� �� ũ�� ������
    /*
    �Ʊ� �ൿ���� ����: ��Ʈ���� ���(����)�� �������� ���� �ൿ�Ѵ� (����5�� �ý��۰� ����)
    
    �� �ൿ���� ����: ���� �ӵ��� ���� ���� ����(�ӵ��� ���� ��� ����) 
    */

    private OnBattleObject NowSelectedSwapFrom;     //�Ǹ��� ��ü�� �� ��Ʈ������ ��ü ��� �Ʊ� �����͸� ��� ����
    private PartyDemonData NowSelectedSwapTo;       //�Ǹ��� ��ü�� �� ���忡�� ��ü�� �Ʊ� �����͸� ��� ����


    #endregion

    private void Awake()
    {
        //BattleEnemyDataList = new List<MonsterData>();

        //PartyMemberArray = new PartyMember[4];  //��Ƽ ���� �迭 �ʱ�ȭ
        NowOnBattleEntryList.Capacity = MaxNumberOfEntry;       //���ʿ��� ���Ҵ��� �Ͼ�� �ʵ��� �̸� ũ�� ����
        TurnOrderList.Capacity = MaxNumberOfEntry;     //���ʿ��� ���Ҵ��� �Ͼ�� �ʵ��� �̸� ũ�� ����

        TestPMM.LoadTMP();
    }

    private void Start()
    {
        //GetEnemyData();
        //SetPartyData();
        //SetPlayerTurn();
        int i = 0;
        TestPMM.AddJobQueueMethod(()=> SetOnBattlePartyData());
        TestPMM.AddJobQueueMethod(() => PartyUIScript.InitPartyDisplay(NowOnBattleEntryList));
        TestPMM.AddJobQueueMethod(() => SetPlayerPhase());
        //TestScript.Instance.ReturnClass().AddJobQueueMethod(() => TestPMM.SaveTMP());
    }


    private void OnDisable()
    {
        //GameManager.Instance.ReturnPartyManager().RemoveObserver(this);     //��Ƽ�Ŵ����� ������ ����Ʈ���� ����
    }

    /// <summary>
    /// ��Ƽ �Ŵ������Լ� ���� �ȳ� ����
    /// </summary>
    public void UpdateEntry()
    {

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
    /// ���� ���۽� ���� ��Ƽ ���� ����
    /// </summary>
    private void SetOnBattlePartyData()
    {
        //�÷��̾� �����͸� ���� ���� ��Ʈ���� ù��° ����� �ֱ�
        OnBattleObject PlayerCharacterBattleData = new OnBattleObject();

        //PlayerCharacterBattleData.SetMemberData(GameManager.Instance.ReturnPartyManager().ReturnPlayerCharacterData());
        //PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        //NowOnBattleEntryList.Add(PlayerCharacterBattleData);

        //�׽�Ʈ�� ����. �Ʒ� ���� ��
        PlayerCharacterBattleData.SetMemberData(TestPMM.ReturnPlayerCharacterData());
        PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        NowOnBattleEntryList.Add(PlayerCharacterBattleData);
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

        for (int i = 0; i < 3; i++)
        {
            if (TestPMM.ReturnEntryDemonData(i) != null)
            {
                OnBattleObject PartyDemonBattleData = new OnBattleObject();
                PartyDemonBattleData.SetMemberData(TestPMM.ReturnEntryDemonData(i));
                PartyDemonBattleData.SetIsPlayerCharacter(false);
                NowOnBattleEntryList.Add(PartyDemonBattleData);
                NumberOfOnBattleParty++;
            }
            else
            {
                NowOnBattleEntryList.Add(null);
            }
        }


        //BattlePlayerData = GameManager.Instance.GetPlayerData();
        //�Ʒ��δ� �� ���� ���� ���� Ȯ������ ���� ��.
        //for (int i = 0; i < 4; i++)
        //{
        //    PartyMemberArray[i] = GameManager.Instance.GetPartyData(i); //���� ���� ����� �迭�� ��Ƽ�� ������ �޾ƿ�
        //
        //    if (PartyMemberArray[i].IsEmptyMember)
        //    {
        //        PartyUIScript.SetEmptyMember(i);
        //    }
        //    else
        //    {
        //        PartyUIScript.SetUIText(i, PartyMemberArray[i].Name, PartyMemberArray[i].MaxHP, PartyMemberArray[i].MaxMP);
        //        PartyUIScript.SetHPValue(i, PartyMemberArray[i].RemainHP);
        //        PartyUIScript.SetMPValue(i, PartyMemberArray[i].RemainMP);
        //
        //        PartyMemberNumber++;    //Ȱ�� ������ �Ʊ� �ο� �� ����
        //    }
        //}
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


    /// <summary>
    /// ���� �Ǹ��� ����� ��� ��Ʈ������ ���ŵǰ� �������� ���ư���
    /// </summary>
    public void RemoveInEntry(OnBattleObject Target)
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
        TurnUIScript.PlacePlayerTurn(PartyMemberNumber);
        IsPlayerPhase = true;

        if (TurnOrderList.Count != 0)
            TurnOrderList.Clear();          //���� �ൿ���� ����Ʈ�� ������� �ʴٸ� �� �� ����ش�

        if (TurnOrderIndexList.Count != 0)
            TurnOrderIndexList.Clear();

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
            TurnUIScript.PlacePlayerTurn(NumberOfOnBattleParty);         //���� ���� ��Ƽ �������ŭ �� ��ũ ǥ��
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
        //�� ���� ��⿭�� ����ִٸ� �ӵ��� ���� �� ���� ����
        if (TurnOrderIndexList.Count == 0)
        {
            for (int i = 0; i < NowOnBattleEntryList.Count; i++)
            {
                if (NowOnBattleEntryList[i] != null)
                {
                    TurnOrderList.Add(NowOnBattleEntryList[i]);     //�ൿ���� ����Ʈ�� ���� ��Ʈ�� �� ����� Add�� �־��ش�
                    TurnOrderIndexList.Add(i);
                }
            }

            TurnOrderList.Sort((Character1, Character2) => Character1.ReturnMemberData().ReturnAg().CompareTo(Character1.ReturnMemberData().ReturnAg()));
            TurnOrderIndexList.Sort((n1, n2) => NowOnBattleEntryList[n2].ReturnMemberData().ReturnAg().CompareTo(NowOnBattleEntryList[n1].ReturnMemberData().ReturnAg()));
        }
        
        PartyUIScript.ActiveTurn(TurnOrderIndexList[0]);        //���� �ൿ�ϰ� �Ǵ� ĳ������ �� Ȱ��ȭ ǥ��
    }

    
    /// <summary>
    /// �ӵ��� ���� ��Ƽ �ൿ������ �����ϴ� �Լ�
    /// </summary>
    private void SortPartyTurn()
    {
        for (int i = 0; i < NowOnBattleEntryList.Count; i++)
        {
            if (NowOnBattleEntryList[i] != null)
            {
                TurnOrderList.Add(NowOnBattleEntryList[i]);     //�ൿ���� ����Ʈ�� ���� ��Ʈ�� �� ����� Add�� �־��ش�
                TurnOrderIndexList.Add(i);
                
            }
        }

        TurnOrderList.Sort((Character1, Character2) => Character1.ReturnMemberData().ReturnAg().CompareTo(Character1.ReturnMemberData().ReturnAg()));
        TurnOrderIndexList.Sort((n1, n2) => NowOnBattleEntryList[n1].ReturnMemberData().ReturnAg().CompareTo(NowOnBattleEntryList[n2].ReturnMemberData().ReturnAg()));
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

                    TurnUIScript.HideAllMark(IsPlayerPhase);        //��� ���� �� ���߱�

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
                        TurnUIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 2, IsPlayerPhase);       //���� �� 2�� ���߱�
                        NumberOfHalfTurn -= 2;
                    }
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn >= 1)
                    {
                        TurnUIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;
                        TurnUIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //������ �� 1�� ���߱�
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 2;
                    }
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn == 0)
                    {
                        //���� �ൿ �ϼ��� ���� �� 1���� �����ִٸ� 2�� ������ ��ǻ� ���� ������ ��������
                        TurnUIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;
                    }
                    else if (NumberOfWholeTurn == 1)
                    {
                        //���� �ൿ �ϼ��� ������ �� 1���� �����ִٸ� 2�� ������ ��ǻ� ���� ������ ��������
                        TurnUIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //������ �� 1�� ���߱�
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        TurnUIScript.HideWholeTurnMark(NumberOfWholeTurn, 2, IsPlayerPhase);        //������ �� 2�� ���߱�
                        NumberOfWholeTurn -= 2;
                        NumberOfUsedTurn += 2;
                    }
                }
                break;
            case PressTurn.ReduceHalfTurn:
                {
                    if (NumberOfHalfTurn >= 1)
                    {
                        TurnUIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        TurnUIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //������ �� 1�� ���߱�
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                }
                break;
            case PressTurn.ReduceHalfTurn_Party:
                {
                    if (NumberOfHalfTurn >= 1)
                    {
                        TurnUIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //���� �� 1�� ���߱�
                        NumberOfHalfTurn -= 1;      //�����ִ� ���� �� 1�� ����
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        if (IsPlayerPhase)
                            TurnUIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleParty);       //���� �� 1�� �߰�
                        else
                            TurnUIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleEnemy);       //���� �� 1�� �߰�
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }    
                }
                break;
            case PressTurn.ChangeHalfTurn:
                {
                    if (NumberOfWholeTurn == 0 && NumberOfHalfTurn != 0)
                    {
                        TurnUIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);
                        NumberOfHalfTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        if (IsPlayerPhase)
                            TurnUIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleParty);       //���� �� 1�� �߰�
                        else
                            TurnUIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleEnemy);       //���� �� 1�� �߰�
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
                PartyUIScript.DeactiveTurn(TurnOrderIndexList[0]);      //�ൿ���� ��ģ ĳ������ �� �Ϸ� ǥ��
                TurnOrderIndexList.RemoveAt(0);           //�ൿ���� ��ģ ĳ���ʹ� �� ��⿭���� ������
                SetPartyTurnOrder();
            }
            else
            {

            }
        }
    }

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