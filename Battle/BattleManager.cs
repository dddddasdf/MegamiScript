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
[Flags]
public enum PressTurn
{
    None = 0, 
    ReduceAllTurn = 1 << 0,      //남아있는 모든 턴 소모
    /*
    1. 상대방에게 반사 또는 흡수인 속성으로 공격했을 경우
    2. 도주를 실패했을 경우
    3. 대화에서 상대방을 화나게 했을 경우

    남아있는 턴을 모두 소모하고 즉시 상대 페이즈로 교체
    */

    ReduceTwoTurn = 1 << 1,      //2턴 소모
    /*
    1. 상대방에게 무효인 속성으로 공격했을 경우
    2. 상대방이 공격을 회피했을 경우 (하마, 주살의 즉사가 실패한 미스는 제외)
    3. 적과 협상에 실패하고 적이 그냥 떠났을 경우
    
    다음과 같은 우선순위를 거쳐 2턴을 소모
    1. 2개의 절반 턴을 소모
    2. 절반 턴이 충분하지 않을 경우, 1개의 절반 턴과 1개의 온전한 한 턴을 소모
    3. 절반 턴이 존재하지 않을 경우 2개의 온전한 한 턴을 소모
    */

    ReduceHalfTurn = 1 << 2,     //절반 턴 소모
    /*
    1. 공격을 일반적으로 끝마쳤을 경우(적중, 내성)
    2. 적과 협상 타결
    
    다음과 같은 우선순위를 거쳐 절반턴을 소모
    1. 1개의 절반 턴을 소모
    2. 절반 턴이 존재하지 않을 경우, 1개의 온전한 한 턴을 소모
    */

    ReduceHalfTurn_Party = 1 << 3,   //절반 턴 소모(플레이어 페이즈에서 턴스킵 및 체인지 행동 전용)
    /*
    1. 다음 아군에게 차례를 넘겼을 경우
    2. 아군 소환, 복귀 또는 교체를 했을 경우

    다음과 같은 우선순위를 거쳐 절반턴을 소모
    1. 1개의 절반 턴을 소모
    2. 절반 턴이 존재하지 않을 경우, 1개의 온전한 한 턴을 절반 턴으로 변경
    */

    ChangeHalfTurn = 1 << 4,     //온전한 한 턴을 절반 턴으로 변경
    /*
    1. 크리티컬이 발생하거나 상대방의 약점인 속성으로 공격했을 경우
    2. 스카우트에서 크리티컬 스카우트가 발동할 경우(대상이 만족될 수 있도록 올바르게 골랐을 경우)

    다음과 같은 우선순위를 거쳐 턴 소모
    1. 1개의 온전한 한 턴을 절반 턴으로 변경
    2. 온전한 한 턴이 존재하지 않을 경우, 1개의 절반 턴 소모
    */
}



public class BattleManager : MonoBehaviour, IPartyObserver
{
    #region SetField

    /// <summary>
    /// 테스트용 임시 변수
    /// </summary>
    private PartyMemberManager TestPMM = new PartyMemberManager();
    private EnemyDatabaseManager TestEDM = new EnemyDatabaseManager();
    private SkillSystem SkillCalculator = new SkillSystem();


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
    private List<OnBattleEnemyObject?> NowOnBattleEnemyList = new List<OnBattleEnemyObject?>();     //전투 중인 적의 파티
#nullable disable
    private List<int> EnemyTurnOrderList = new List<int>();
    private int NumberOfOnBattleEnemy = 0;          //현재 생존해서 전투 중인 적의 수

    private const int MaxNumberOfEntry = 4;       //한 번에 엔트리에 들어오는 멤버 수는 넷-초기화할 때 크기 지정용
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

    public static SkillCellData SelectedSkillDataBuffer = new();    //선택된 스킬 정보 저장용
    private SkillTypeSort SelectedSkillTypeCaching;     //현재 선택된 스킬의 타입 캐싱용

    #endregion

    private void Awake()
    {
        //BattleEnemyDataList = new List<MonsterData>();

        //PartyMemberArray = new PartyMember[4];  //파티 정보 배열 초기화
        NowOnBattlePartyList.Capacity = MaxNumberOfEntry;       //불필요한 재할당이 일어나지 않도록 미리 크기 설정
        PartyTurnOrderList.Capacity = MaxNumberOfEntry;     //불필요한 재할당이 일어나지 않도록 미리 크기 설정
        SelectedSkillDataBuffer = null;     //스킬 데이터 버퍼용 정적 변수 비워주기

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
        SelectedSkillDataBuffer = null;     //씬이 교체될 때 버퍼용 정적 변수 초기화
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
    public void NowActivatedSkillMark()
    {
        if (!UIScript.ReturnIsButtonDoubleClicked())
        {
            return;         //같은 스킬 버튼이 더블클릭된 게 아니면 다음 단계로 넘어가지 않는다
        }

        SelectedSkillTypeCaching = SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnSkillType();

        if (SelectedSkillTypeCaching == SkillTypeSort.Recover || SelectedSkillTypeCaching == SkillTypeSort.Support)
        {
            //회복 또는 서포트 계열: 아군 타게팅 UI
        }
        else
        {
            //그 외 적 공격 계열: 적 타게팅 UI
            UIScript.ShowEnemyTargetSelectUI();     //고른 스킬이 무엇인지 띄워주는 화면 호출
        }

        
        UIScript.ResetIsButtonChanged();        //더블 클릭 감지용 변수 원상복구
    }

    /// <summary>
    /// 현재 고른 스킬 발동
    /// </summary>
    public void ClickSelectedSkill()
    {
        switch (SelectedSkillTypeCaching)
        {
            case SkillTypeSort.Physical:
            case SkillTypeSort.Gun:
                ActivateAttackSkil(new CalculateSkillDamage(SkillCalculator.CalculatePhysicGunDamage));     //델리게이트로 스킬 시스템의 물리/총 계산 메소드 전달
                break;
            case SkillTypeSort.Fire:
            case SkillTypeSort.Ice:
            case SkillTypeSort.Electric:
            case SkillTypeSort.Force:
            case SkillTypeSort.Light:
            case SkillTypeSort.Dark:
            case SkillTypeSort.Almighty:
                ActivateAttackSkil(new CalculateSkillDamage(SkillCalculator.CalculateMagicDamage));     //델리게이트로 스킬 시스템의 마법 계산 메소드 전달
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
    /// 스킬 시스템 공격 스킬 대미지 계산 메소드 전달용 델리게이트
    /// </summary>
    /// <param name="UsedSkill"></param>
    /// <param name="SkillUser"></param>
    /// <param name="Target"></param>
    /// <param name="SkillDamage"></param>
    private delegate void CalculateSkillDamage(SkillDataRec UsedSkill, IOnBattleObject SkillUser, IOnBattleObject Target, out int SkillDamage);

    private void ActivateAttackSkil(CalculateSkillDamage SkillSystemMethod)
    {
        int CalculatedDamage;       //받을 대미지
        int numberOfHit = SetNumberOfHit();       //받을 히트수를 정한다
        PressTurn reduceTurnFlag = PressTurn.None;       //공격을 끝마치면 턴 차감을 위해 사용하는 변수
        SkillDataRec NowUsingSkill = SelectedSkillDataBuffer.ReturnSkillDataRec();      //현재 사용된 스킬의 정보

        if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
        {
            //싱글 타겟 스킬: 현재 타겟된 적의 정보를 필요로 한다
            for (int i = 0; i < numberOfHit; i++)
            {
                //공격 횟수만큼 반복문
                SkillSystemMethod(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], UIScript.ReturnTargetedEnemyData(), out CalculatedDamage);
                SavePressTurnFlag(UIScript.ReturnTargetedEnemyData().ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
            }
        }
        else if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.All)
        {
            //전체 타겟 스킬: 전체 타겟 기술은 공격 횟수 1로 고정
            for (int i = 0; i < NowOnBattleEnemyList.Count; i++)
            {
                //현재 생존 중인 적 개체수만큼 공격 반복
                SkillSystemMethod(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[i], out CalculatedDamage);
                SavePressTurnFlag(NowOnBattleEnemyList[i].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
            }
        }
        else
        {
            //다단기(전체X 히트수와 대상 랜덤)
            int randomIndex;        //랜덤 대상 인덱스를 받을 변수

            //공격 횟수만큼 반복문
            for (int i = 0; i < numberOfHit; i++)
            {
                randomIndex = UnityEngine.Random.Range(0, NowOnBattleEnemyList.Count - 1);
                SkillSystemMethod(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[randomIndex], out CalculatedDamage);
                SavePressTurnFlag(NowOnBattleEnemyList[randomIndex].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
            }
        }
        EndAttackTurn(reduceTurnFlag);    //공격 프로세스를 마치고 해당 캐릭터의 턴 종료
    }

    /// <summary>
    /// 공격 스킬들의 랜덤 타수 결정
    /// </summary>
    /// <returns></returns>
    private int SetNumberOfHit()
    {
        int minHit = SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnMinHit();
        int maxHit = SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnMaxHit();
        if (minHit == maxHit)
        {
            //최대 히트수와 최소 히트수가 같다면 굳이 랜덤 굴릴 필요 없이 바로 반환
            return SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnMaxHit();
        }
        else
        {
            //아니라면 랜덤한 히트수를 골라서 반환
            return UnityEngine.Random.Range(minHit, maxHit);
        }
    }

    /// <summary>
    /// 사용한 스킬의 효과에 따라 플래그 축적
    /// </summary>
    /// <param name="skillAffinityOfUsedSkill"></param>
    /// <param name="reduceTurnFlag"></param>
    private void SavePressTurnFlag(SkillAffinities? skillAffinityOfUsedSkill, ref PressTurn reduceTurnFlag)
    {
        switch (skillAffinityOfUsedSkill)
        {
            case SkillAffinities.Weak:
                reduceTurnFlag |= PressTurn.ChangeHalfTurn;     //약점
                break;
            case SkillAffinities.Void:
                reduceTurnFlag |= PressTurn.ReduceTwoTurn;      //무효
                break;
            case SkillAffinities.Reflect:
            case SkillAffinities.Drain:
                reduceTurnFlag |= PressTurn.ReduceAllTurn;      //반사, 흡수
                break;
            case SkillAffinities.Resist:
            default:
                reduceTurnFlag |= PressTurn.ReduceHalfTurn;     //내성, 아무것도 없음
                break;
        }
    }

    /// <summary>
    /// 축적된 플래그를 바탕으로 공격 후 턴 차감 실행
    /// </summary>
    /// <param name="reduceTurnFlag"></param>
    private void EndAttackTurn(PressTurn reduceTurnFlag)
    {
        //공격을 끝마친 후 턴차감 플래그가 쌓인 걸 보고 우선순위순으로 if 조건 걸어둔다->우선순위가 높은 게 존재할 경우 해당 규칙에 따라 턴 차감
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

        UIScript.ResetIsButtonChanged();
        if (NowOnBattlePartyList[PartyTurnOrderIndexList[0]].ReturnIsPlayerCharacter())
            UIScript.EnableActButtonPlayerTurn();       //플레이어 턴이면 비활성화 된 버튼들 활성화
        else
            UIScript.DisableActButtonPartyTurn();       //그 외 파티원 턴이면 일부 버튼 비활성화

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
                UIScript.ReturnActMenu();
            }
            else
            {

            }
        }
    }


    /// <summary>
    /// 턴스킵 버튼을 누름
    /// </summary>
    public void PressTrunSkip()
    {
        if (!UIScript.ReturnIsButtonDoubleClicked())
        {
            return;     //같은 버튼이 다시 눌린 게 아니라면 다음 행동 수행할 필요X
        }

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

#region Dummy
/// <summary>
/// 사용한 공격스킬이 물리 스킬일 경우
/// </summary>
//private void ActivatePhysicSkill()
//{
//    int CalculatedDamage;       //받을 대미지
//    int numberOfHit = SetNumberOfHit();       //받을 히트수를 정한다
//    PressTurn reduceTurnFlag = PressTurn.None;       //공격을 끝마치면 턴 차감을 위해 사용하는 변수
//    SkillDataRec NowUsingSkill = SelectedSkillDataBuffer.ReturnSkillDataRec();      //현재 사용된 스킬의 정보

//    if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
//    {
//        //싱글 타겟 스킬: 현재 타겟된 적의 정보를 필요로 한다
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            //공격 횟수만큼 반복문
//            SkillCalculator.CalculatePhysicGunDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], UIScript.ReturnTargetedEnemyData(), out CalculatedDamage);
//            SavePressTurnFlag(UIScript.ReturnTargetedEnemyData().ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.All)
//    {
//        //전체 타겟 스킬: 전체 타겟 기술은 공격 횟수 1로 고정
//        for (int i = 0; i < NowOnBattleEnemyList.Count; i++)
//        {
//            //현재 생존 중인 적 개체수만큼 공격 반복
//            SkillCalculator.CalculatePhysicGunDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[i], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[i].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else
//    {
//        //다단기(전체X 히트수와 대상 랜덤)
//        int randomIndex;        //랜덤 대상 인덱스를 받을 변수

//        //공격 횟수만큼 반복문
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            randomIndex = UnityEngine.Random.Range(0, NowOnBattleEnemyList.Count - 1);
//            SkillCalculator.CalculatePhysicGunDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[randomIndex], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[randomIndex].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    EndAttackTurn(reduceTurnFlag);    //공격 프로세스를 마치고 해당 캐릭터의 턴 종료
//}


///// <summary>
///// 사용한 공격스킬이 마법 스킬일 경우
///// </summary>
//private void ActivateMagicSkill()
//{
//    int CalculatedDamage;       //받을 대미지
//    int numberOfHit = SetNumberOfHit();       //받을 히트수를 정한다
//    PressTurn reduceTurnFlag = PressTurn.None;       //공격을 끝마치면 턴 차감을 위해 사용하는 변수
//    SkillDataRec NowUsingSkill = SelectedSkillDataBuffer.ReturnSkillDataRec();      //현재 사용된 스킬의 정보

//    if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
//    {
//        //싱글 타겟 스킬: 현재 타겟된 적의 정보를 필요로 한다
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            //공격 횟수만큼 반복문
//            SkillCalculator.CalculateMagicDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], UIScript.ReturnTargetedEnemyData(), out CalculatedDamage);
//            SavePressTurnFlag(UIScript.ReturnTargetedEnemyData().ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else if (SelectedSkillDataBuffer.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.All)
//    {
//        //전체 타겟 스킬: 전체 타겟 기술은 공격 횟수 1로 고정
//        for (int i = 0; i < NowOnBattleEnemyList.Count; i++)
//        {
//            //현재 생존 중인 적 개체수만큼 공격 반복
//            SkillCalculator.CalculateMagicDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[i], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[i].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    else
//    {
//        //다단기(전체X 히트수와 대상 랜덤)
//        int randomIndex;        //랜덤 대상 인덱스를 받을 변수

//        //공격 횟수만큼 반복문
//        for (int i = 0; i < numberOfHit; i++)
//        {
//            randomIndex = UnityEngine.Random.Range(0, NowOnBattleEnemyList.Count - 1);
//            SkillCalculator.CalculateMagicDamage(NowUsingSkill, NowOnBattlePartyList[PartyTurnOrderIndexList[0]], NowOnBattleEnemyList[randomIndex], out CalculatedDamage);
//            SavePressTurnFlag(NowOnBattleEnemyList[randomIndex].ReturnAffinity(NowUsingSkill.ReturnSkillType()), ref reduceTurnFlag);
//        }
//    }
//    EndAttackTurn(reduceTurnFlag);    //공격 프로세스를 마치고 해당 캐릭터의 턴 종료
//}
#endregion
