using System.Collections;
using System.Collections.Generic;
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

public class BattleManager : MonoBehaviour
{
    #region SetVariables

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


    #endregion

    private void Awake()
    {
        //BattleEnemyDataList = new List<MonsterData>();

        //PartyMemberArray = new PartyMember[4];  //파티 정보 배열 초기화
    }

    private void Start()
    {
        //GetEnemyData();
        //SetPartyData();
        //SetPlayerTurn();
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
    /// 전투 시작시 UI에 파티 정보 세팅
    /// </summary>
    private void SetPartyData()
    {
        //BattlePlayerData = GameManager.Instance.GetPlayerData();

        for (int i = 0; i < 4; i++)
        {
            PartyMemberArray[i] = GameManager.Instance.GetPartyData(i); //전투 정보 저장용 배열에 파티의 정보를 받아옴

            if (PartyMemberArray[i].IsEmptyMember)
            {
                PartyUIScript.SetEmptyMember(i);
            }
            else
            {
                PartyUIScript.SetUIText(i, PartyMemberArray[i].Name, PartyMemberArray[i].MaxHP, PartyMemberArray[i].MaxMP);
                PartyUIScript.SetHPValue(i, PartyMemberArray[i].RemainHP);
                PartyUIScript.SetMPValue(i, PartyMemberArray[i].RemainMP);

                PartyMemberNumber++;    //활동 가능한 아군 인원 수 증가
            }
        }
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


    #region CheckTurns

    //임시용
    private void SetPlayerTurn()
    {
        TurnUIScript.PlacePlayerTurn(PartyMemberNumber);
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
        }

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