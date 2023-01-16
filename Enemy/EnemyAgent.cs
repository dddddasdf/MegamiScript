using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAgent : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private float WanderSpeed; //��ȸ �ӵ�
    [SerializeField] private float RunSpeed;    //�޸��� �ӵ�

    private Animator AnimeState;
    private NavMeshAgent EnemyNav;
    private Transform EnemyTr;
        
    private float Magnitude;
    private Vector3 EndPosition;
    private int MoveProbability = 50;   //���� ���ִٰ� ������ Ȯ���� 50%
    private bool IsMoving = false;
    private float RandomDistanceMin = -3f;
    private float RandomDistanceMax = 3f;
    private float RotationSpeed = 10f;

    private float DistanceOffset = 0.2f;  //�̵��� �� �ٻ簪 ���ϱ��

    private Collider ChaseTarget;
    private Collider EnemyCollider;

    private Coroutine WanderWork;   //��ȸ �Ѱ� �ڷ�ƾ
    private Coroutine WanderMove;   //��ȸ �� �ִϸ��̼� �ڷ�ƾ
    private Coroutine ChaseMove;    //���� ��� �ڷ�ƾ

    private WaitForSeconds NewWanderPointDelay; //��ǥ ��µ� �ɸ��� ������ �ð� ĳ�̿�
    private WaitForSeconds MovingDelay; //�̵� ������ �ð� ĳ�̿�

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
        EnemyNav.autoBraking = false; //������ �Ÿ��� ���� �ӵ� ��ȭ �ɼ� ����
        EnemyNav.updateRotation = false;  //�ڵ� ȸ�� ����
    }


    #region Wander

    /// <summary>
    /// ������Ʈ���� ��ȸ ����
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

        AnimeState.SetFloat("Magnitude", 0f);    //�ȱ� �ִϸ��̼� �ӵ� ��ȭ
        
        if (WanderWork != null)
        {
            StopCoroutine(WanderWork);
            WanderWork = null;
        }
    }
    
    public IEnumerator WanderCoroutine()
    {
        yield return NewWanderPointDelay;    //�ٷ� ���ο� ��ǥ ��� �� �ƴ϶� 3�� ��� �� ���ο� ��ǥ ��� ����

        while (true)
        {
            if (!IsMoving)
                ChooseNewPosition();
        
            yield return MovingDelay;
        }
    }

    /// <summary>
    /// ��ȸ �� Ȯ���� ���� �����ϰ� ���� �̵��� �� ����
    /// </summary>
    private void ChooseNewPosition()
    {
        int RandomMove = Random.Range(0, 100);
        if (RandomMove >= MoveProbability)
            return; //30���� Ȯ���� ��÷�Ǹ� ������

        Vector3 NowPosition = transform.position;
        float MoveX = Random.Range(RandomDistanceMin, RandomDistanceMax);
        float MoveZ = Random.Range(RandomDistanceMin, RandomDistanceMax);
        EndPosition = new Vector3(NowPosition.x + MoveX, NowPosition.y, NowPosition.z + MoveZ);
        //���� �̵��� ���: X-���� X�� ���ο� ���� X�� ����, Y-Y�� �̵��� �����Ƿ� ���� ����, Z-���� Z ��ġ�� ���� ���� Z�� ����

        EnemyNav.SetDestination(EndPosition);
        
        WanderMove = StartCoroutine(WanderAnime());
    }

    private IEnumerator WanderAnime()
    {
        while (!(EnemyNav.remainingDistance == 0f))
        {
            if (EnemyNav.desiredVelocity != Vector3.zero)
            {
                AnimeState.SetFloat("Magnitude", 0.5f);    //�ȱ� �ִϸ��̼� �ӵ� ��ȭ
                Quaternion EndRotation = Quaternion.LookRotation(EnemyNav.desiredVelocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, EndRotation, Time.deltaTime * RotationSpeed);
            }
            yield return null;
        }

        AnimeState.SetFloat("Magnitude", 0f);    //�ȱ� �ִϸ��̼� �ӵ� ��ȭ
    }
    #endregion

    #region Chase

    /// <summary>
    /// ������Ʈ���� ���� �߰� ��� ����
    /// </summary>
    /// <param name="Target"> ������ ���� </param>
    public void SetChase(Collider Target)
    {
        if (WanderWork != null || WanderMove != null)
            StopWander();
        
        EnemyNav.speed = RunSpeed;    //�̵� �ӵ��� �޸��� �ӵ��� ����
        AnimeState.SetBool("IsFindPlayer", true);   //�޸��� ������� ����
        ChaseTarget = Target;
        ChaseMove = StartCoroutine(ChaseCoroutine());
    }

    public void StopChase()
    {
        AnimeState.SetBool("IsFindPlayer", false);   //�޸��� ������� ����
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
            EnemyNav.SetDestination(ChaseTarget.transform.position);   //������ ���� ��ǥ ����
            Quaternion EndRotation = Quaternion.LookRotation(EnemyNav.desiredVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, EndRotation, Time.deltaTime * RotationSpeed);
            
            yield return null;
        }
    }
    #endregion Chase

    #region Dummy
    ////���������� ��ȸ �� �����̴� �κ�<-���
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
    //            break;  //��ǥ �������� �ٻ��ϰ� ���������� �̵��� ����

    //        Magnitude = Mathf.Lerp(Magnitude, 0.5f, 10f * Time.deltaTime);
    //        AnimeState.SetFloat("Magnitude", Magnitude);    //�ȱ� �ִϸ��̼� �ӵ� ��ȭ
    //        transform.position = Vector3.Lerp(NowPosition, EndPosition, MovingSpeed * Time.deltaTime);

    //        Debug.Log("��ġ: " + transform.position);

    //        yield return null;
    //    }

    //    Magnitude = 0f;
    //    AnimeState.SetFloat("Magnitude", Magnitude);    //�ȱ� �ִϸ��̼� �ӵ� ��ȭ
    //    IsMoving = false;


    //}
    #endregion
}
