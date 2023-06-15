using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

//지금 악마 번호 꼬여있음 나중에 손 봐야 한다(혼선 없게)
//포트레이트 넘버와 파티 넘버를 손 보는 방향으로(도트 스프라이트는 그대로 둔다. 데몬 엑셀파일이 만들어져 있으므로)


/// <summary>
/// 프레스 턴 시스템
/// </summary>
public enum PressTurn
{
    ReduceAllTurn,      //남아있는 모든 턴 소모
    /*
    1. 상대방에게 반사 또는 흡수인 속성으로 공격했을 경우
    2. 도주를 실패했을 경우
    3. 대화에서 상대방을 화나게 했을 경우

    남아있는 턴을 모두 소모하고 즉시 상대 페이즈로 교체
    */

    ReduceTwoTurn,      //2턴 소모
    /*
    1. 상대방에게 무효인 속성으로 공격했을 경우
    2. 상대방이 공격을 회피했을 경우 (하마, 주살의 즉사가 실패한 미스는 제외)
    3. 적과 협상에 실패하고 적이 그냥 떠났을 경우
    
    다음과 같은 우선순위를 거쳐 2턴을 소모
    1. 2개의 절반 턴을 소모
    2. 절반 턴이 충분하지 않을 경우, 1개의 절반 턴과 1개의 온전한 한 턴을 소모
    3. 절반 턴이 존재하지 않을 경우 2개의 온전한 한 턴을 소모
    */

    ReduceHalfTurn,     //절반 턴 소모
    /*
    1. 공격을 일반적으로 끝마쳤을 경우(적중, 내성)
    2. 적과 협상 타결
    
    다음과 같은 우선순위를 거쳐 절반턴을 소모
    1. 1개의 절반 턴을 소모
    2. 절반 턴이 존재하지 않을 경우, 1개의 온전한 한 턴을 소모
    */

    ReduceHalfTurn_Party,   //절반 턴 소모(플레이어 페이즈 전용)
    /*
    1. 다음 아군에게 차례를 넘겼을 경우
    2. 아군 소환, 복귀 또는 교체를 했을 경우

    다음과 같은 우선순위를 거쳐 절반턴을 소모
    1. 1개의 절반 턴을 소모
    2. 절반 턴이 존재하지 않을 경우, 1개의 온전한 한 턴을 절반 턴으로 변경
    */

    ChangeHalfTurn,     //온전한 한 턴을 절반 턴으로 변경
    /*
    1. 크리티컬이 발생하거나 상대방의 약점인 속성으로 공격했을 경우
    2. 스카우트에서 크리티컬 스카우트가 발동할 경우(대상이 만족될 수 있도록 올바르게 골랐을 경우)

    다음과 같은 우선순위를 거쳐 턴 소모
    1. 1개의 온전한 한 턴을 절반 턴으로 변경
    2. 온전한 한 턴이 존재하지 않을 경우, 1개의 절반 턴 소모
    */
}


/// <summary>
/// 현재 어떤 메뉴창에 진입한 건지 기록용
/// </summary>
public enum NowOnMenu
{
    Act,
    SkillMenu,
    ItemMenu,
    Talk,
    Change,
    Escape,
    Pass,
    SkillSelected,
    ItemSelected
}

public class BattleManager : MonoBehaviour, IPartyObserver
{
    #region SetField

    /// <summary>
    /// 테스트용 임시 변수
    /// </summary>
    private PartyMemberManager TestPMM = new PartyMemberManager();
    private EnemyDatabaseManager TestEDM = new EnemyDatabaseManager();


    private List<MonsterData> BattleEnemyDataList;

    [SerializeField] private BattleUIManager UIScript;

    
    private int PartyMemberNumber = 0;  //현재 전투에서 활동 가능한 아군 인원 수

    private int NumberOfPartyWholeTurn;     //파티 턴에서 남아있는 온전한 한 턴 수
    private int NumberOfPartyHalfTurn;      //파티 턴에서 남아있는 절반 턴 수
    private int NumberOfEnemyWholeTurn;     //적 턴에서 남아있는 온전한 한 턴 수
    private int NumberOfEnemyHalfTurn;      //적 턴에서 남아있는 절반 턴 수

    private int NumberOfWholeTurn = 0;          //온전한 한 턴 수
    private int NumberOfHalfTurn = 0;           //절반 턴 수
    private int NumberOfUsedTurn = 0;           //소모한 턴 마크 수

    private bool IsPlayerPhase;              //플레이어 페이즈 확인용: true == 플레이어 페이즈/false == 적 페이즈
    private OnBattlePartyObject NowTurnCharacterOfParty;   //플레이어 페이즈 중 현재 행동 중인 아군

#nullable enable
    private List<OnBattlePartyObject?> NowOnBattlePartyList = new List<OnBattlePartyObject?>(); //현재 전투에 참전 중인 엔트리 리스트
#nullable disable
    private List<OnBattlePartyObject> PartyTurnOrderList = new List<OnBattlePartyObject>();     //행동턴 배정용 리스트<-레거시. 전투 구현 끝나면 지워야 한다
    private List<int> PartyTurnOrderIndexList = new List<int>();     //행동턴 배정용 인덱스 리스트
    private int NumberOfOnBattleParty = 0;           //현재 생존해서 전투 중인 파티원 수

#nullable enable
    private List<OnBattleEnemyObject?> NowOnBattleEnemyList = new List<OnBattleEnemyObject?>();
#nullable disable
    private List<int> EnemyTurnOrderList = new List<int>();
    private int NumberOfOnBattleEnemy = 0;          //현재 생존해서 전투 중인 적의 수

    private int MaxNumberOfEntry = 4;       //한 번에 엔트리에 들어오는 멤버 수는 넷-초기화할 때 크기 지정용
    /*
    아군 행동순서 배정: 엔트리의 상단(좌측)에 있을수록 먼저 행동한다 (진여5의 시스템과 동일)
    
    적 행동순서 배정: 적의 속도에 따라 순서 배정(속도가 같을 경우 랜덤) 
    */

    private OnBattlePartyObject NowSelectedSwapFrom;     //악마를 교체할 때 엔트리에서 교체 대상 아군 데이터를 담는 변수
    private PartyDemonData NowSelectedSwapTo;       //악마를 교체할 때 스톡에서 교체할 아군 데이터를 담는 변수

    private Queue<Action> JobQueue = new Queue<Action>();      //순차적으로 진행시킬 작업 목록
    public void AddJobQueueMethod(Action Method)
    {
        JobQueue.Enqueue(Method);
    }

    private Stack<NowOnMenu> OnMenuStack = new ();
    private NowOnMenu PopMenu;

    #endregion

    private void Awake()
    {
        //BattleEnemyDataList = new List<MonsterData>();

        //PartyMemberArray = new PartyMember[4];  //파티 정보 배열 초기화
        NowOnBattlePartyList.Capacity = MaxNumberOfEntry;       //불필요한 재할당이 일어나지 않도록 미리 크기 설정
        PartyTurnOrderList.Capacity = MaxNumberOfEntry;     //불필요한 재할당이 일어나지 않도록 미리 크기 설정

        TestPMM.LoadTMP();
        TestEDM.InitEnemyDatabaseManager();
    }

    private void Start()
    {
        //GetEnemyData();
        //SetPartyData();
        //SetPlayerTurn();

        //아래는 현재 여기서 파티 리스트 초기화도 겸하고 있기 때문에 작업큐를 쓰는데, 실전에서는 사용할 필요가 없다 (즉 전투만 보는 테스트가 끝나면 큐 이용 지워라)
        TestPMM.AddJobQueueMethod(() => SetOnBattlePartyData());
        //AddJobQueueMethod(() => UIScript.InitPartyDisplay(NowOnBattlePartyList));
        //TestPMM.AddJobQueueMethod(() => UIScript.InitPartyDisplay(NowOnBattlePartyList));
        TestPMM.AddJobQueueMethod(() => SetPlayerPhase());
        TestEDM.AddJobQueueMethod(() => SetOnBattleEnemyData());
        TestEDM.AddJobQueueMethod(() => UIScript.InitEnemyisplay(NowOnBattleEnemyList));
    }


    private void OnDisable()
    {
        //GameManager.Instance.ReturnPartyManager().RemoveObserver(this);     //파티매니저의 옵저버 리스트에서 해제
    }

    /// <summary>
    /// 파티 매니저에게서 갱신 안내 받음 - 옵저버 패턴
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
        //    BattleEnemyDataList.Add(EnemyDataBase.ReturnEnemyData(EnemyIndexList[i]));  //인덱스를 통해 순차적으로 전투에 나오는 적들의 데이터를 가져옴
        //}
        //EnemyDataBase.ClearList();  //임시용 인덱스 리스트 클리어
        //
        //BattleUIScript.PrintEnemyData(BattleEnemyDataList);

        //아래로 전투만 보는 테스트 빌드 한정 내용
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
    /// 전투 시작시 현재 파티 정보 세팅
    /// </summary>
    private void SetOnBattlePartyData()
    {
        //플레이어 데이터를 전투 참여 엔트리의 첫번째 멤버로 넣기
        OnBattlePartyObject PlayerCharacterBattleData = new OnBattlePartyObject();

        //PlayerCharacterBattleData.SetMemberData(GameManager.Instance.ReturnPartyManager().ReturnPlayerCharacterData());
        //PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        //NowOnBattleEntryList.Add(PlayerCharacterBattleData);

        //테스트용 빌드. 아래 지울 것
        PlayerCharacterBattleData.SetMemberData(TestPMM.ReturnPlayerCharacterData());
        PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        NowOnBattlePartyList.Add(PlayerCharacterBattleData);
        NumberOfOnBattleParty++;

        //동료 악마 리스트 가져오기
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

        //테스트용. 필드와 연계시킬 때는 이 부분을 지우고 위 문단을 활성화시킨다
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
    /// 현재 행동턴인 캐릭터의 스킬 데이터 전달해서 UI가 표시할 수 있게 한다
    /// </summary>
    private void CallSetSkillList()
    {
        //SkillUIScript.AddJobQueueMethod(() => CallShowAffinityMark());
        UIScript.SetSkillList(NowOnBattlePartyList[PartyTurnOrderIndexList[0]]);
    }


    /// <summary>
    /// 스킬 선택창에서 현재 버튼이 활성화 된 스킬에 따라 적에게 마크 표시하라고 하는 함수
    /// </summary>
    public void CheckNowActivatedSkillMark()
    {
        
    }

    /// <summary>
    /// 현재 고른 스킬이 최종적으로 타격할 적에게 마크 표시하라고 하는 함수
    /// </summary>
    public void CheckSelectedSkillMark()
    {

    }



    #endregion


    /// <summary>
    /// 동료 악마가 사망할 경우 엔트리에서 제거되고 스톡으로 돌아간다
    /// </summary>
    public void RemoveInEntry(OnBattlePartyObject Target)
    {
        Target.ReturnMemberData();
        NumberOfOnBattleParty--;
    }

    /// <summary>
    /// 스톡에 있는 악마와 엔트리에 있는 동료 악마 교체
    /// </summary>
    public void ChangeEntry()
    {
        //교체 UI 출력 전달
    }

    public void NowSelectedEntryTarget()
    {

    }


    #region CheckTurns

    /// <summary>
    /// 플레이어 페이즈 전환
    /// </summary>
    private void SetPlayerPhase()
    {
        UIScript.PlacePlayerTurn(PartyMemberNumber);
        IsPlayerPhase = true;

        if (PartyTurnOrderList.Count != 0)
            PartyTurnOrderList.Clear();          //만약 행동순서 리스트가 비어있지 않다면 한 번 비워준다

        if (PartyTurnOrderIndexList.Count != 0)
            PartyTurnOrderIndexList.Clear();

        //SortPartyTurn();        //속도에 따른 행동순서 배정을 한다
        FillNumberOfTurn();
        SetPartyTurnOrder();
        
    }

    /// <summary>
    /// 페이즈 전환시 행동턴수를 알맞게 채워주는 함수
    /// </summary>
    private void FillNumberOfTurn()
    {
        if (IsPlayerPhase)
        {
            NumberOfWholeTurn = NumberOfOnBattleParty;           //채워지는 온전한 턴 수 = 현재 생존 중인 파티 멤버수
            NumberOfHalfTurn = 0;                           //절반 턴 수 0으로 초기화
            UIScript.PlacePlayerTurn(NumberOfOnBattleParty);         //생존 중인 파티 멤버수만큼 턴 마크 표시
        }
        else
        {
            //적의 파티에서 현재 생존 중인 멤버수만큼 온전한 한 턴 수를 채워준다
            //그 숫자만큼 배틀 UI 관리자에게 턴 아이콘을 채우라고 전달한다
            //특정 보스전 같은 예외가 있지만 지금은 그것에 대해 신경쓰지 않는다
        }

    }

    /// <summary>
    /// 현재 턴인 파티원과 턴 대기열 관리
    /// </summary>
    private void SetPartyTurnOrder()
    {
        //턴 순서 대기열이 비어있다면 속도에 따라 턴 순서 배정해서 리스트를 채워준다
        if (PartyTurnOrderIndexList.Count == 0)
        {
            for (int i = 0; i < NowOnBattlePartyList.Count; i++)
            {
                if (NowOnBattlePartyList[i] != null)
                {
                    PartyTurnOrderList.Add(NowOnBattlePartyList[i]);     //행동순서 리스트에 현재 엔트리 된 멤버를 Add로 넣어준다
                    PartyTurnOrderIndexList.Add(i);
                }
            }

            PartyTurnOrderList.Sort((Character1, Character2) => Character1.ReturnMemberData().ReturnAg().CompareTo(Character1.ReturnMemberData().ReturnAg()));
            PartyTurnOrderIndexList.Sort((n1, n2) => NowOnBattlePartyList[n2].ReturnMemberData().ReturnAg().CompareTo(NowOnBattlePartyList[n1].ReturnMemberData().ReturnAg()));
        }

        UIScript.ActiveTurn(PartyTurnOrderIndexList[0]);        //현재 행동하게 되는 캐릭터의 턴 활성화 표시
        CallSetSkillList();
    }

    
    /// <summary>
    /// 속도에 따라 파티 행동순서를 정렬하는 함수
    /// </summary>
    private void SortPartyTurn()
    {
        for (int i = 0; i < NowOnBattlePartyList.Count; i++)
        {
            if (NowOnBattlePartyList[i] != null)
            {
                PartyTurnOrderList.Add(NowOnBattlePartyList[i]);     //행동순서 리스트에 현재 엔트리 된 멤버를 Add로 넣어준다
                PartyTurnOrderIndexList.Add(i);
                
            }
        }

        PartyTurnOrderList.Sort((Character1, Character2) => Character1.ReturnMemberData().ReturnAg().CompareTo(Character1.ReturnMemberData().ReturnAg()));
        PartyTurnOrderIndexList.Sort((n1, n2) => NowOnBattlePartyList[n1].ReturnMemberData().ReturnAg().CompareTo(NowOnBattlePartyList[n2].ReturnMemberData().ReturnAg()));
    }

    
    /// <summary>
    /// 프레스 턴 룰에 따라 현재 페이즈에서 남아있는 턴 수를 차감하는 함수
    /// </summary>
    /// <param name="TurnFlag">어떤 행동을 취했느냐</param>
    private void ReduceTurn(PressTurn TurnFlag)
    {
        switch (TurnFlag)
        {
            case PressTurn.ReduceAllTurn:
                {
                    NumberOfWholeTurn = 0;
                    NumberOfHalfTurn = 0;
                    //페이즈 종료 및 상대 페이즈 전환 함수 기입
                    //턴 삭제 및 페이즈 전환 연출 함수 기입
                    //생각해보니 위엣것들 남은 턴이 전부 0 이하가 되면 페이즈 넘어가도록 액션 예약 걸어두면 되나?

                    UIScript.HideAllMark(IsPlayerPhase);        //모든 남은 턴 감추기

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
                        //온전한 턴 2턴 차감
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 2, IsPlayerPhase);       //절반 턴 2개 감추기
                        NumberOfHalfTurn -= 2;
                    }
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn >= 1)
                    {
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //절반 턴 1개 감추기
                        NumberOfHalfTurn -= 1;
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //온전한 턴 1개 감추기
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 2;
                    }
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn == 0)
                    {
                        //현재 행동 턴수가 절반 턴 1개만 남아있다면 2개 차감이 사실상 전부 차감과 마찬가지
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //절반 턴 1개 감추기
                        NumberOfHalfTurn -= 1;
                    }
                    else if (NumberOfWholeTurn == 1)
                    {
                        //현재 행동 턴수가 온전한 턴 1개만 남아있다면 2개 차감이 사실상 전부 차감과 마찬가지
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //온전한 턴 1개 감추기
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 2, IsPlayerPhase);        //온전한 턴 2개 감추기
                        NumberOfWholeTurn -= 2;
                        NumberOfUsedTurn += 2;
                    }
                }
                break;
            case PressTurn.ReduceHalfTurn:
                {
                    if (NumberOfHalfTurn >= 1)
                    {
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //절반 턴 1개 감추기
                        NumberOfHalfTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        UIScript.HideWholeTurnMark(NumberOfWholeTurn, 1, IsPlayerPhase);        //온전한 턴 1개 감추기
                        NumberOfWholeTurn -= 1;
                        NumberOfUsedTurn += 1;
                    }
                }
                break;
            case PressTurn.ReduceHalfTurn_Party:
                {
                    if (NumberOfHalfTurn >= 1)
                    {
                        UIScript.HideHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, 1, IsPlayerPhase);      //절반 턴 1개 감추기
                        NumberOfHalfTurn -= 1;      //남아있는 절반 턴 1개 차감
                        NumberOfUsedTurn += 1;
                    }
                    else
                    {
                        if (IsPlayerPhase)
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleParty);       //절반 턴 1개 추가
                        else
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleEnemy);       //절반 턴 1개 추가
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
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleParty);       //절반 턴 1개 추가
                        else
                            UIScript.PlaceHalfTurnMark(NumberOfWholeTurn, NumberOfHalfTurn, NumberOfUsedTurn, NumberOfOnBattleEnemy);       //절반 턴 1개 추가
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }
                }
                break;
        }

        if (NumberOfWholeTurn <= 0 && NumberOfHalfTurn <= 0)
        {
            IsPlayerPhase = !IsPlayerPhase;   //사용 가능 행동턴 수가 0이 되었을 경우 상대방 페이즈로 넘어간다
            //페이즈 전환 함수 호출
            NumberOfUsedTurn = 0;
        }
        else
        {
            if (IsPlayerPhase)
            {
                UIScript.DeactiveTurn(PartyTurnOrderIndexList[0]);      //행동턴을 마친 캐릭터의 턴 완료 표시
                PartyTurnOrderIndexList.RemoveAt(0);           //행동턴을 마친 캐릭터는 턴 대기열에서 빠진다
                SetPartyTurnOrder();
            }
            else
            {

            }
        }
    }

    /// <summary>
    /// 턴스킵 버튼
    /// </summary>
    public void PressTrunSkip()
    {
        ReduceTurn(PressTurn.ReduceHalfTurn_Party);
    }

    //아래로 턴 넘기기 동작 체크용 함수들 - 삭제 예정
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
 상태이상기

약점이면 앵간해선 걸린다 (거의 90퍼 쯤인 듯)
내성이면 상당히 경감?
없으면 몰?루
미스는 ㄱㅊ음 추가턴감소 없다
무효면 턴 2개 날림->물리/총 계열에 달린 추가 효과로는 반응하지 않는다
 */