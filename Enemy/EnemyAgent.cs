using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private float WanderSpeed; //배회 속도
    [SerializeField] private float RunSpeed;    //달리는 속도

    private Animator AnimeState;
    private NavMeshAgent EnemyNav;
    private Transform EnemyTr;
        
    private float Magnitude;
    private Vector3 EndPosition;
    private int MoveProbability = 50;   //적이 서있다가 움직일 확률은 50%
    private bool IsMoving = false;
    private float RandomDistanceMin = -3f;
    private float RandomDistanceMax = 3f;
    private float RotationSpeed = 10f;

    private float DistanceOffset = 0.2f;  //이동할 때 근사값 구하기용

    private Collider ChaseTarget;
    private Collider EnemyCollider;

    private Coroutine WanderWork;   //배회 총괄 코루틴
    private Coroutine WanderMove;   //배회 중 애니메이션 코루틴
    private Coroutine ChaseMove;    //추적 담당 코루틴

    private WaitForSeconds NewWanderPointDelay; //좌표 찍는데 걸리는 딜레이 시간 캐싱용
    private WaitForSeconds MovingDelay; //이동 딜레이 시간 캐싱용

    #endregion

    private void Awake()
    {
        NewWanderPointDelay = new WaitForSeconds(3.0f);
        MovingDelay = new WaitForSeconds(7.0f);
    }

    private void OnEnable()
    {
        AnimeState = GetComponent<Animator>();
        EnemyNav = GetComponent<NavMeshAgent>();
        EnemyTr = GetComponent<Transform>();
        EnemyCollider = GetComponent<Collider>();
        EnemyNav.autoBraking = false; //목적지 거리에 따른 속도 변화 옵션 끄기
        EnemyNav.updateRotation = false;  //자동 회전 끄기
    }


    #region Wander

    /// <summary>
    /// 에이전트에게 배회 지시
    /// </summary>
    public void SetWander()
    {
        if (null == WanderWork)
        {
            EnemyNav.isStopped = true;
            EnemyNav.ResetPath();
            EnemyNav.speed = WanderSpeed;
            EnemyNav.isStopped = false;
            AnimeState.SetBool("IsFindPlayer", false);

            WanderWork = StartCoroutine(WanderCoroutine());
        }
    }

    public void StopWander()
    {
        if (WanderMove != null)
        {
            StopCoroutine(WanderMove);
            WanderMove = null;
        }

        AnimeState.SetFloat("Magnitude", 0f);    //걷기 애니메이션 속도 변화
        
        if (WanderWork != null)
        {
            StopCoroutine(WanderWork);
            WanderWork = null;
        }
    }
    
    public IEnumerator WanderCoroutine()
    {
        yield return NewWanderPointDelay;    //바로 새로운 좌표 찍는 게 아니라 3초 대기 후 새로운 좌표 찍기 시작

        while (true)
        {
            if (!IsMoving)
                ChooseNewPosition();
        
            yield return MovingDelay;
        }
    }

    /// <summary>
    /// 배회 중 확률에 따라 랜덤하게 새로 이동할 곳 지정
    /// </summary>
    private void ChooseNewPosition()
    {
        int RandomMove = Random.Range(0, 100);
        if (RandomMove >= MoveProbability)
            return; //30퍼의 확률에 당첨되면 움직임

        Vector3 NowPosition = transform.position;
        float MoveX = Random.Range(RandomDistanceMin, RandomDistanceMax);
        float MoveZ = Random.Range(RandomDistanceMin, RandomDistanceMax);
        EndPosition = new Vector3(NowPosition.x + MoveX, NowPosition.y, NowPosition.z + MoveZ);
        //새로 이동할 장소: X-기존 X에 새로운 랜덤 X를 더함, Y-Y축 이동은 없으므로 현상 유지, Z-기존 Z 위치에 새로 랜덤 Z를 더함

        EnemyNav.SetDestination(EndPosition);
        
        WanderMove = StartCoroutine(WanderAnime());
    }

    private IEnumerator WanderAnime()
    {
        while (!(EnemyNav.remainingDistance == 0f))
        {
            if (EnemyNav.desiredVelocity != Vector3.zero)
            {
                AnimeState.SetFloat("Magnitude", 0.5f);    //걷기 애니메이션 속도 변화
                Quaternion EndRotation = Quaternion.LookRotation(EnemyNav.desiredVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, EndRotation, Time.deltaTime * RotationSpeed);
            }
            yield return null;
        }

        AnimeState.SetFloat("Magnitude", 0f);    //걷기 애니메이션 속도 변화
    }
    #endregion

    #region Chase

    /// <summary>
    /// 에이전트에게 유저 추격 명령 지시
    /// </summary>
    /// <param name="Target"> 감지된 유저 </param>
    public void SetChase(Collider Target)
    {
        if (WanderWork != null || WanderMove != null)
            StopWander();
        
        EnemyNav.speed = RunSpeed;    //이동 속도를 달리기 속도로 맞춤
        AnimeState.SetBool("IsFindPlayer", true);   //달리기 모션으로 변경
        ChaseTarget = Target;
        ChaseMove = StartCoroutine(ChaseCoroutine());
    }

    public void StopChase()
    {
        AnimeState.SetBool("IsFindPlayer", false);   //달리기 모션으로 변경
        EnemyNav.isStopped = true;
        EnemyNav.ResetPath();
        EnemyNav.isStopped = false;
        ChaseTarget = null;

        if (ChaseMove != null)
        {
            StopCoroutine(ChaseMove);
            ChaseMove = null;
        }
    }

    public IEnumerator ChaseCoroutine()
    {
        while (true)
        {
            EnemyNav.SetDestination(ChaseTarget.transform.position);   //유저를 향해 목표 설정
            Quaternion EndRotation = Quaternion.LookRotation(EnemyNav.desiredVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, EndRotation, Time.deltaTime * RotationSpeed);
            
            yield return null;
        }
    }
    #endregion Chase

    #region Dummy
    ////실질적으로 배회 중 움직이는 부분<-폐기
    //private IEnumerator WanderMoving(float Angle)
    //{
    //float Angle = Mathf.Atan2(Mathf.Abs(EndPosition.x - NowPosition.x), Mathf.Abs(EndPosition.z - NowPosition.z)) * Mathf.Rad2Deg;
    //    IsMoving = true;

    //    //transform.rotation = Quaternion.Lerp(transform.rotation, );

    //    while (true)
    //    {
    //        Vector3 NowPosition = transform.position;
    //        Vector3 Tmp = EndPosition - NowPosition;
    //        float SqrLength = Tmp.sqrMagnitude;

    //        if (SqrLength < DistanceOffset * DistanceOffset)
    //            break;  //목표 지점까지 근사하게 도달했으면 이동을 종료

    //        Magnitude = Mathf.Lerp(Magnitude, 0.5f, 10f * Time.deltaTime);
    //        AnimeState.SetFloat("Magnitude", Magnitude);    //걷기 애니메이션 속도 변화
    //        transform.position = Vector3.Lerp(NowPosition, EndPosition, MovingSpeed * Time.deltaTime);

    //        Debug.Log("위치: " + transform.position);

    //        yield return null;
    //    }

    //    Magnitude = 0f;
    //    AnimeState.SetFloat("Magnitude", Magnitude);    //걷기 애니메이션 속도 변화
    //    IsMoving = false;


    //}
    #endregion
}
