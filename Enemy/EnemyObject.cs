using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyObject : MonoBehaviour
{
    #region SetVariables

    private enum State
    {
        Wander, //배회
        Chase   //유저 추격
    }

    private State EnemyState;
    private EnemyAgent Controller;
    [SerializeField] private LayerMask TargetLayer;
    [SerializeField] private float SearchRange = 7; //몹의 유저 감지 범위
    private float ChaseRange = 8;  //몹의 유저 추적 범위
    private Collider[] PlayerTarget;    //유저 추적시 사용
    private bool OnPatrol = false;  //코루틴 오작동 방지용
    private bool OnChase = false;   //코루틴 오작동 방지용
    private Coroutine CheckingCoroutine;    //유저 어그로 체크용 코루틴

    private List<int> EnemyIndexList;

    private Coroutine TimerCoroutine;   //타이머용 코루틴

    private event Action AppearTimerEvent;   //스폰된지 15초가 지나면 스폰 관리자에게 신호를 준다

    /// <summary>
    /// 몹 심볼이 가지고 있어야 할 정보
    /// </summary>
    private struct SymbolObjectData
    {
        Vector2 SymbolPosition; //심볼의 2차원 좌표
        public EnemySymbolType ThisEnemyType;  //필드상에서 표시될 모델링 유형
    }
    #endregion

    private void Awake()
    {
        Controller = GetComponent<EnemyAgent>();
        EnemyState = State.Wander;
        EnemyIndexList = new List<int>();
        SymbolObjectData NewSymbolData;
        NewSymbolData.ThisEnemyType = EnemySymbolType.Humanoid; //아직은 임시 단계이므로 인간형으로 통일
    }

    private void OnEnable()
    {
        Action();
    }

    private void OnDisable()
    {
        EnemyIndexList.Clear(); //악마 리스트 클리어
        AppearTimerEvent = null;    //함수 예약 리스트 클리어
    }

    /// <summary>
    /// 리스폰 담당자가 만들어준 랜덤 몬스터 인덱스를 받아옴
    /// </summary>
    /// <param name="EnemyIndexListTmp"> 임시 몬스터 인덱스 리스트 </param>
    public void SetEnemyIndexList(List<int> EnemyIndexListTmp)
    {
        EnemyIndexList = EnemyIndexListTmp;
    }

    /// <summary>
    /// 리스폰 매니저에게서 캐싱용 변수 받아옴
    /// </summary>
    /// <param name="AppearTimer">캐싱된 15초</param>
    public void SetTimer(WaitForSeconds AppearTimer)
    {
        TimerCoroutine = StartCoroutine(AppearTimeCheck(AppearTimer));
    }

    /// <summary>
    /// 출현한 시간 세기용
    /// </summary>
    /// <param name="AppearTimer">출현 시간은 15초</param>
    /// <returns></returns>
    private IEnumerator AppearTimeCheck(WaitForSeconds AppearTimer)
    {
        yield return AppearTimer;   //15초가 지나면 스폰 매니저에게 회수 요청 전달
        AppearTimerEvent?.Invoke();
        AppearTimerEvent = null;
    }

    /// <summary>
    /// 이벤트 리스트에 함수 추가
    /// </summary>
    /// <param name="fetchSymbolObject">예약 목록에 추가할 함수</param>
    public void GetAppearTimeEvent(Action fetchSymbolObject)
    {
        AppearTimerEvent += () =>
        {
            fetchSymbolObject();    //15초가 되면 회수 함수 호출하도록 예약
        };
    }

    /// <summary>
    /// 플레이어 캐릭터와 충돌 감지
    /// </summary>
    /// <param name="Collision"></param>
    private void OnCollisionEnter(Collision Collision)
    {
        if (Collision.collider == PlayerTarget[0])
        {
            EnemyDatabaseManager.SetIndexList(EnemyIndexList); //악마 데이터 담당 스크립트에게 가지고 있는 인덱스 목록을 넘겨줌
            GameManager.Instance.CallBattle();  //게임 매니저에게 배틀 호출 요청
        }
    }

    #region AI
    /// <summary>
    /// 몬스터가 현재 취할 상태 설정
    /// </summary>
    private void Action()
    {
        switch (EnemyState)
        {
            case State.Wander:
                StartCoroutine(Patrol());
                break;
            case State.Chase:
                StartCoroutine(CheckIsPlayerIn());
                break;
        }
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            PlayerTarget = Physics.OverlapSphere(transform.position, SearchRange, TargetLayer);    //감시 영역 내에 플레이어 있는지 확인

            if (PlayerTarget != null && 0 < PlayerTarget.Length)
            {
                EnemyState = State.Chase;   //플레이어 발견시 배회 모드 종료 및 추적 모드 활성화
                Controller.StopWander();
                OnPatrol = false;
                Action();

                break;
            }

            if (OnPatrol == false)
            {
                //리스폰 후 주변을 스캔하여 유저가 없는 것이 판명되면 에이전트에게 배회 명령 지시
                //순찰 모드 전환 후 1회만 작동
                Controller.SetWander();
                OnPatrol = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 추격 하면서 인식 범위 내에 유저가 있는지 확인함
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckIsPlayerIn()
    {
        while (true)
        {
            PlayerTarget = Physics.OverlapSphere(transform.position, ChaseRange, TargetLayer);    //감시 영역 내에 플레이어 있는지 확인

            if (PlayerTarget != null && PlayerTarget.Length == 0)
            {
                EnemyState = State.Wander;
                Controller.StopChase();
                OnChase = false;
                Action();

                break;
            }

            if (OnChase == false)
            {
                //에이전트에게 유저 추적 명령 지시
                //추적 모드 전환 후 1회만 작동
                Controller.SetChase(PlayerTarget[0]);
                OnChase = true;
            }
            yield return null;
        }
    }
    #endregion
}
