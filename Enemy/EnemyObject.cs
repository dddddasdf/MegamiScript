using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private List<int> EnemyIndexList;



    #endregion

    private void Awake()
    {
        EnemyState = State.Wander;
        Controller = GetComponent<EnemyAgent>();
        EnemyIndexList = new List<int>();
    }
    private void Start()
    {
        Action();
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
    /// �÷��̾� ĳ���Ϳ� �浹 ����
    /// </summary>
    /// <param name="Collision"></param>
    private void OnCollisionEnter(Collision Collision)
    {
        if (Collision.collider == PlayerTarget[0])
        {
            EnemyData.SetIndexList(EnemyIndexList); //���� ������ ��ũ��Ʈ���� �ӽ������� ������ �ִ� �ε��� ����� �Ѱ���
            GameManager.Instance.CallBattle();
            Debug.Log("�浹!");
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
