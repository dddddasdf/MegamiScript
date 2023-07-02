using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyObject : MonoBehaviour
{
    #region SetVariables

    private enum State
    {
        Wander, //��ȸ
        Chase   //���� �߰�
    }

    private State EnemyState;
    private EnemyAgent Controller;
    [SerializeField] private LayerMask TargetLayer;
    [SerializeField] private float SearchRange = 7; //���� ���� ���� ����
    private float ChaseRange = 8;  //���� ���� ���� ����
    private Collider[] PlayerTarget;    //���� ������ ���
    private bool OnPatrol = false;  //�ڷ�ƾ ���۵� ������
    private bool OnChase = false;   //�ڷ�ƾ ���۵� ������
    private Coroutine CheckingCoroutine;    //���� ��׷� üũ�� �ڷ�ƾ

    private List<int> EnemyIndexList;

    private Coroutine TimerCoroutine;   //Ÿ�̸ӿ� �ڷ�ƾ

    private event Action AppearTimerEvent;   //�������� 15�ʰ� ������ ���� �����ڿ��� ��ȣ�� �ش�

    /// <summary>
    /// �� �ɺ��� ������ �־�� �� ����
    /// </summary>
    private struct SymbolObjectData
    {
        Vector2 SymbolPosition; //�ɺ��� 2���� ��ǥ
        public EnemySymbolType ThisEnemyType;  //�ʵ�󿡼� ǥ�õ� �𵨸� ����
    }
    #endregion

    private void Awake()
    {
        Controller = GetComponent<EnemyAgent>();
        EnemyState = State.Wander;
        EnemyIndexList = new List<int>();
        SymbolObjectData NewSymbolData;
        NewSymbolData.ThisEnemyType = EnemySymbolType.Humanoid; //������ �ӽ� �ܰ��̹Ƿ� �ΰ������� ����
    }

    private void OnEnable()
    {
        Action();
    }

    private void OnDisable()
    {
        EnemyIndexList.Clear(); //�Ǹ� ����Ʈ Ŭ����
        AppearTimerEvent = null;    //�Լ� ���� ����Ʈ Ŭ����
    }

    /// <summary>
    /// ������ ����ڰ� ������� ���� ���� �ε����� �޾ƿ�
    /// </summary>
    /// <param name="EnemyIndexListTmp"> �ӽ� ���� �ε��� ����Ʈ </param>
    public void SetEnemyIndexList(List<int> EnemyIndexListTmp)
    {
        EnemyIndexList = EnemyIndexListTmp;
    }

    /// <summary>
    /// ������ �Ŵ������Լ� ĳ�̿� ���� �޾ƿ�
    /// </summary>
    /// <param name="AppearTimer">ĳ�̵� 15��</param>
    public void SetTimer(WaitForSeconds AppearTimer)
    {
        TimerCoroutine = StartCoroutine(AppearTimeCheck(AppearTimer));
    }

    /// <summary>
    /// ������ �ð� �����
    /// </summary>
    /// <param name="AppearTimer">���� �ð��� 15��</param>
    /// <returns></returns>
    private IEnumerator AppearTimeCheck(WaitForSeconds AppearTimer)
    {
        yield return AppearTimer;   //15�ʰ� ������ ���� �Ŵ������� ȸ�� ��û ����
        AppearTimerEvent?.Invoke();
        AppearTimerEvent = null;
    }

    /// <summary>
    /// �̺�Ʈ ����Ʈ�� �Լ� �߰�
    /// </summary>
    /// <param name="fetchSymbolObject">���� ��Ͽ� �߰��� �Լ�</param>
    public void GetAppearTimeEvent(Action fetchSymbolObject)
    {
        AppearTimerEvent += () =>
        {
            fetchSymbolObject();    //15�ʰ� �Ǹ� ȸ�� �Լ� ȣ���ϵ��� ����
        };
    }

    /// <summary>
    /// �÷��̾� ĳ���Ϳ� �浹 ����
    /// </summary>
    /// <param name="Collision"></param>
    private void OnCollisionEnter(Collision Collision)
    {
        if (Collision.collider == PlayerTarget[0])
        {
            EnemyDatabaseManager.SetIndexList(EnemyIndexList); //�Ǹ� ������ ��� ��ũ��Ʈ���� ������ �ִ� �ε��� ����� �Ѱ���
            GameManager.Instance.CallBattle();  //���� �Ŵ������� ��Ʋ ȣ�� ��û
        }
    }

    #region AI
    /// <summary>
    /// ���Ͱ� ���� ���� ���� ����
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
            PlayerTarget = Physics.OverlapSphere(transform.position, SearchRange, TargetLayer);    //���� ���� ���� �÷��̾� �ִ��� Ȯ��

            if (PlayerTarget != null && 0 < PlayerTarget.Length)
            {
                EnemyState = State.Chase;   //�÷��̾� �߽߰� ��ȸ ��� ���� �� ���� ��� Ȱ��ȭ
                Controller.StopWander();
                OnPatrol = false;
                Action();

                break;
            }

            if (OnPatrol == false)
            {
                //������ �� �ֺ��� ��ĵ�Ͽ� ������ ���� ���� �Ǹ�Ǹ� ������Ʈ���� ��ȸ ��� ����
                //���� ��� ��ȯ �� 1ȸ�� �۵�
                Controller.SetWander();
                OnPatrol = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// �߰� �ϸ鼭 �ν� ���� ���� ������ �ִ��� Ȯ����
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckIsPlayerIn()
    {
        while (true)
        {
            PlayerTarget = Physics.OverlapSphere(transform.position, ChaseRange, TargetLayer);    //���� ���� ���� �÷��̾� �ִ��� Ȯ��

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
                //������Ʈ���� ���� ���� ��� ����
                //���� ��� ��ȯ �� 1ȸ�� �۵�
                Controller.SetChase(PlayerTarget[0]);
                OnChase = true;
            }
            yield return null;
        }
    }
    #endregion
}
