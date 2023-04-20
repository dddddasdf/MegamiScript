using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

public class BattleManager : MonoBehaviour, IPartyObserver
{
    #region SetVariables

    /// <summary>
    /// 테스트용 임시 변수
    /// </summary>
    private PartyMemberManager TestPMM = new PartyMemberManager();

    [SerializeField] private GameObject BattleCanvasObject;
    private List<MonsterData> BattleEnemyDataList;

    [SerializeField] private BattleUI BattleUIScript;
    [SerializeField] private PartyUI PartyUIScript;
    [SerializeField] private TurnUI TurnUIScript;
    [SerializeField] private BattleSkillMenuUI SkillUIScript;

    private PartyMember[] PartyMemberArray; //파티 정보를 받아올 배열
    private int PartyMemberNumber = 0;  //현재 전투에서 활동 가능한 아군 인원 수

    private int NumberOfPartyWholeTurn;     //파티 턴에서 남아있는 온전한 한 턴 수
    private int NumberOfPartyHalfTurn;      //파티 턴에서 남아있는 절반 턴 수
    private int NumberOfEnemyWholeTurn;     //적 턴에서 남아있는 온전한 한 턴 수
    private int NumberOfEnemyHalfTurn;      //적 턴에서 남아있는 절반 턴 수

    private int NumberOfWholeTurn;          //온전한 한 턴 수
    private int NumberOfHalfTurn;           //절반 턴 수

    private bool IsPlayerPhase;              //플레이어 페이즈 확인용: true == 플레이어 페이즈/false == 적 페이즈
    private OnBattleObject NowTurnCharacterOfParty;   //플레이어 페이즈 중 현재 행동 중인 아군

#nullable enable
    private List<OnBattleObject?> NowOnBattleEntryList = new List<OnBattleObject?>(); //현재 전투에 참전 중인 엔트리 리스트
#nullable disable
    private List<OnBattleObject> TurnOrderList = new List<OnBattleObject>();     //행동턴 배정용 리스트
    private int MaxNumberOfEntry = 4;       //한 번에 엔트리에 들어오는 멤버 수는 넷
    /*
    아군 행동순서 배정: 엔트리의 상단(좌측)에 있을수록 먼저 행동한다 (진여5의 시스템과 동일)
    
    적 행동순서 배정: 적의 속도에 따라 순서 배정(속도가 같을 경우 랜덤) 
    */

    private OnBattleObject NowSelectedSwapFrom;     //악마를 교체할 때 엔트리에서 교체 대상 아군 데이터를 담는 변수
    private PartyDemonData NowSelectedSwapTo;       //악마를 교체할 때 스톡에서 교체할 아군 데이터를 담는 변수


    #endregion

    private void Awake()
    {
        //BattleEnemyDataList = new List<MonsterData>();

        //PartyMemberArray = new PartyMember[4];  //파티 정보 배열 초기화
        NowOnBattleEntryList.Capacity = MaxNumberOfEntry;       //불필요한 재할당이 일어나지 않도록 미리 크기 설정
        TurnOrderList.Capacity = MaxNumberOfEntry;     //불필요한 재할당이 일어나지 않도록 미리 크기 설정

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
        //TestScript.Instance.ReturnClass().AddJobQueueMethod(() => TestPMM.SaveTMP());
    }


    private void OnDisable()
    {
        //GameManager.Instance.ReturnPartyManager().RemoveObserver(this);     //파티매니저의 옵저버 리스트에서 해제
    }

    /// <summary>
    /// 파티 매니저에게서 갱신 안내 받음
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
            BattleEnemyDataList.Add(EnemyDataBase.ReturnEnemyData(EnemyIndexList[i]));  //인덱스를 통해 순차적으로 전투에 나오는 적들의 데이터를 가져옴
        }
        EnemyDataBase.ClearList();  //임시용 인덱스 리스트 클리어

        BattleUIScript.PrintEnemyData(BattleEnemyDataList);
    }

    /// <summary>
    /// 전투 시작시 현재 파티 정보 세팅
    /// </summary>
    private void SetOnBattlePartyData()
    {
        //플레이어 데이터를 전투 참여 엔트리의 첫번째 멤버로 넣기
        OnBattleObject PlayerCharacterBattleData = new OnBattleObject();

        //PlayerCharacterBattleData.SetMemberData(GameManager.Instance.ReturnPartyManager().ReturnPlayerCharacterData());
        //PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        //NowOnBattleEntryList.Add(PlayerCharacterBattleData);

        //테스트용 빌드. 아래 지울 것
        PlayerCharacterBattleData.SetMemberData(TestPMM.ReturnPlayerCharacterData());
        PlayerCharacterBattleData.SetIsPlayerCharacter(true);
        NowOnBattleEntryList.Add(PlayerCharacterBattleData);

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

        for (int i = 0; i < 3; i++)
        {
            if (TestPMM.ReturnEntryDemonData(i) != null)
            {
                OnBattleObject PartyDemonBattleData = new OnBattleObject();
                PartyDemonBattleData.SetMemberData(TestPMM.ReturnEntryDemonData(i));
                PartyDemonBattleData.SetIsPlayerCharacter(false);
                NowOnBattleEntryList.Add(PartyDemonBattleData);
            }
            else
            {
                NowOnBattleEntryList.Add(null);
            }
        }


        //BattlePlayerData = GameManager.Instance.GetPlayerData();
        //아래로는 안 쓴다 구조 변경 확정나면 지울 것.
        //for (int i = 0; i < 4; i++)
        //{
        //    PartyMemberArray[i] = GameManager.Instance.GetPartyData(i); //전투 정보 저장용 배열에 파티의 정보를 받아옴
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
        //        PartyMemberNumber++;    //활동 가능한 아군 인원 수 증가
        //    }
        //}
    }
    #endregion

    #region Skill

    /// <summary>
    /// 행동턴 교체시 현재 행동턴인 캐릭터의 스킬 데이터 전달
    /// </summary>
    public void CallSetSkillList()
    {
        SkillUIScript.SetSkillList(NowTurnCharacterOfParty);
    }


    /// <summary>
    /// 스킬 선택창에서 스킬을 골랐을 때 어떤 스킬인지 확인하고 어떤 UI를 뿌려줄지 결정하는 곳
    /// </summary>
    public void CheckSelectedSkill()
    {
        
    }



    #endregion


    /// <summary>
    /// 동료 악마가 사망할 경우 엔트리에서 제거되고 스톡으로 돌아간다
    /// </summary>
    public void RemoveInEntry(OnBattleObject Target)
    {
        Target.ReturnMemberData();
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
        TurnUIScript.PlacePlayerTurn(PartyMemberNumber);
        IsPlayerPhase = true;

        if (TurnOrderList.Count != 0)
            TurnOrderList.Clear();          //만약 행동순서 리스트가 비어있지 않다면 한 번 비워준다

        for (int i = 0; i < NowOnBattleEntryList.Count; i++)
        {
            if (NowOnBattleEntryList[i] != null)
                TurnOrderList.Add(NowOnBattleEntryList[i]);     //행동순서 리스트에 현재 엔트리 된 멤버를 Add로 넣어준다
        }

        SortPartyTurn();        //속도에 따른 행동순서 배정을 한다
    }

    /// <summary>
    /// 페이즈 전환시 행동턴수를 알맞게 채워주는 함수
    /// </summary>
    private void FillNumberOfTurn()
    {
        if (IsPlayerPhase)
        {
            //플레이어의 파티에서 현재 생존 중인 멤버수만큼 온전한 한 턴 수를 채워준다
            //그 숫자만큼 배틀 UI 관리자에게 턴 아이콘을 채우라고 전달한다
        }
        else
        {
            //적의 파티에서 현재 생존 중인 멤버수만큼 온전한 한 턴 수를 채워준다
            //그 숫자만큼 배틀 UI 관리자에게 턴 아이콘을 채우라고 전달한다
            //특정 보스전 같은 예외가 있지만 지금은 그것에 대해 신경쓰지 않는다
        }

    }

    
    /// <summary>
    /// 속도에 따라 파티 행동순서를 정렬하는 함수
    /// </summary>
    private void SortPartyTurn()
    {
        TurnOrderList.Sort((Character1, Character2) =>Character1.ReturnMemberData().ReturnAg().CompareTo(Character1.ReturnMemberData().ReturnAg()));
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
                NumberOfWholeTurn = 0;
                NumberOfHalfTurn = 0;
                //페이즈 종료 및 상대 페이즈 전환 함수 기입
                //턴 삭제 및 페이즈 전환 연출 함수 기입
                //생각해보니 위엣것들 남은 턴이 전부 0 이하가 되면 페이즈 넘어가도록 액션 예약 걸어두면 되나?
                break;
            case PressTurn.ReduceTwoTurn:
                {
                    if (NumberOfHalfTurn >= 2)
                        NumberOfHalfTurn -= 2;
                    else if (NumberOfHalfTurn == 1 && NumberOfWholeTurn >= 1)
                    {
                        NumberOfHalfTurn -= 1;
                        NumberOfWholeTurn -= 1;
                    }
                    else
                        NumberOfWholeTurn -= 2;
                }
                break;
            case PressTurn.ReduceHalfTurn:
                {
                    if (NumberOfHalfTurn >= 1)
                        NumberOfHalfTurn -= 1;
                    else
                        NumberOfWholeTurn -= 1;
                }
                break;
            case PressTurn.ReduceHalfTurn_Party:
                {
                    if (NumberOfHalfTurn >= 1)
                        NumberOfHalfTurn -= 1;
                    else
                    {
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }    
                }
                break;
            case PressTurn.ChangeHalfTurn:
                {
                    if (NumberOfWholeTurn >= 1)
                    {
                        NumberOfWholeTurn -= 1;
                        NumberOfHalfTurn += 1;
                    }
                    else
                        NumberOfHalfTurn -= 1;
                }
                break;
        }

        if (NumberOfWholeTurn == 0 && NumberOfHalfTurn == 0)
            IsPlayerPhase = !IsPlayerPhase;   //사용 가능 행동턴 수가 0이 되었을 경우 상대방 페이즈로 넘어간다
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