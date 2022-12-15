using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyRespawn : MonoBehaviour
{
    private int MaxMonsterCount = 5;    //�ʵ忡 ���ÿ� ������ �� �ִ� �ִ� ���� ������
    private int NowRandomEnemyCount;
    [SerializeField] private GameObject EnemyPrefab;
    private Coroutine RespawnCoroutineVa;
    
    private void Awake()
    {
        NowRandomEnemyCount = 0;
    }

    #region Random
    
    public void SetRespawn()
    {
        NowRandomEnemyCount = 0;    //�ʵ�� ���� �ִ� ���� ī��Ʈ 0���� �ʱ�ȭ
        RespawnCoroutineVa = StartCoroutine(RespawnCoroutine());
    }

    public void StopRespawn()
    {
        if (RespawnCoroutineVa != null)
            StopCoroutine(RespawnCoroutineVa);
    }

    /// <summary>
    /// ���� ��ġ�� ���� ����
    /// </summary>
    private IEnumerator RespawnCoroutine()
    {
        while (!(NowRandomEnemyCount == MaxMonsterCount))
        {
            GameObject NewEnemy = Instantiate(EnemyPrefab); //�� ���� ��ü ����

            //�ش� ���� �ʵ� ��ü�� ���� �� ���� �ε��� ����Ʈ
            List<int> MonsterIndexList = new List<int>();
            SetRandomIndex(MonsterIndexList);
            NewEnemy.GetComponent<EnemyObject>().SetEnemyIndexList(MonsterIndexList);

            //���� ������ ��ġ
            NewEnemy.transform.position = SetRandomPosition();

            NowRandomEnemyCount++;  //���� �ʵ� �� ���� �ִ� ���� ī��Ʈ ����
            yield return null;
        }
    }

    /// <summary>
    /// ���� �ε��� ���� ���� �� ����Ʈ�� �߰�
    /// </summary>
    /// <param name="MonsterIndexList"></param>
    private void SetRandomIndex(List<int> MonsterIndexList)
    {
        int MonsterCount = Random.Range(1, MaxMonsterCount);    //1�������� �ִ� ���������� ���� �� �� �� �������� ���� ����

        for (int i = 0; i < MonsterCount; i++)
        {
            int NewMonsterIndex = Random.Range(1, (int)MonsterIndex.ExtraMobCount); //������ ���� �ε��� ����
            MonsterIndexList.Add(NewMonsterIndex);  //������ �ε����� �ӽ� ����Ʈ�� �߰�
        }
    } 

    /// <summary>
    /// ���Ͱ� ������ ���� ��ǥ
    /// </summary>
    private Vector3 SetRandomPosition()
    {
        Vector3 VectorTmp = Random.insideUnitSphere * 50;
        NavMeshHit NavHitTmp;
        NavMesh.SamplePosition(VectorTmp, out NavHitTmp, 50, NavMesh.AllAreas);

        return new Vector3(NavHitTmp.position.x, 0f, NavHitTmp.position.z);
    }

    #endregion



    #region Fixed
    //���� ��ġ���� ������ ������ �����Ǵ� ���͵�
    //�� ������Ʈ�� �浹ü�� ���� �̸� �־�ΰ� ������ �ش� ��ġ�� ������ ��� �����Ǵ� ������ ¥�� ����
    //�ϴ� ���� �������� �ϰ� ����


    #endregion

    #region Dummy

    /// <summary>
    /// ���� �ε��� �ӽ� �迭 �ʱ�ȭ �� �迭�� ���� �ε��� �߰�
    /// </summary>
    /// <param name="MonsterIndexArray"></param>
    //private void SetRandomIndexArray(out int[] MonsterIndexArray)
    //{
    //    int MonsterCount = Random.Range(1, MaxMonsterCount);    //1�������� �ִ� ���������� ���� �� �� �� �������� ���� ����
    //    MonsterIndexArray = new int[MonsterCount];

    //    for (int i = 0; i < MonsterCount; i++)
    //    {
    //        int NewMonsterIndex = Random.Range(1, (int)MonsterIndex.ExtraMobCount); //������ ���� �ε��� ����
    //        MonsterIndexArray[i] = NewMonsterIndex;  //������ �ε����� �ӽ� �迭�� �߰�
    //    }
    //}

    #endregion
}
