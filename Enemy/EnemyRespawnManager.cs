using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyRespawnManager : MonoBehaviour
{
    #region SetVariables
    private int MaxMonsterCount = 7;    //�ʵ忡 ���ÿ� ������ �� �ִ� �ִ� ���� ������->�ʺ��� �ٸ��� �� ���� ��� ���ε� �ϴ��� �׽�Ʈ���̹Ƿ� ���Ƿ� ����
    private int NowRandomEnemyCount;
    [SerializeField] private GameObject EnemyPrefab;

    /// <summary>
    /// ������ �ڷ�ƾ ����� ����
    /// </summary>
    private Coroutine SpawnCoroutineVa;
    private bool IsSpawnCoroutineWork;  //�ڷ�ƾ ���� Ȯ�ο� ����

    /// <summary>
    /// yield WaitForSeconds ĳ�̿�, �ɺ��� �ִ� ���� ���� ���ɼ� ���� ������ 2�ʸ��� �����ȴ�
    /// </summary>
    private WaitForSeconds SpawnDelayTimerCaching;
    private WaitForSeconds AppearTimerCaching;

    /// <summary>
    /// ���� ���� ��ġ ������ ������ �ִ� ���� ������Ʈ
    /// </summary>
    private GameObject RandomSpawnPointGameObj;
    private GameObject CertainSpawnPointObj;

    /// <summary>
    /// Ǯ���� ������Ʈ ����Ʈ
    /// </summary>
    [SerializeField] private List<GameObject> PullingObjects;

    private int NowPulledObjectNumber;  //���� Ǯ���� ������Ʈ ���� ��Ͽ�

    //��ǥ ���� ����
    /// <summary>
    /// �ɺ� ���� ��ǥ ������ ����� Ŭ����
    /// </summary>
    class SpawnPointNode
    {
        public SpawnPointNode(Vector3 SpawnPoint)
        {
            this.PointData = SpawnPoint;
        }
        public Vector3 PointData
        {
            get; private set;
        }
    }

    /// <summary>
    /// ��ǥ ����� ����Ʈ
    /// </summary>
    private List<SpawnPointNode> RandomSpawnPointList;

    /// <summary>
    /// �� ����Ʈ�� "��ȣ"�� ������ ����Ʈ
    /// </summary>
    private List<int> RandomSpawnPointIndexList;

    /// <summary>
    /// ������ �� �ε��� "��ȣ" ����� ť
    /// </summary>
    private Queue<int> SpawnedPointIndexQueue;
    
    //��ǥ ���� ���� ����

    #endregion


    private void Awake()
    {
        RandomSpawnPointList = new List<SpawnPointNode>();  //��ǥ ����Ʈ �ʱ�ȭ
        RandomSpawnPointIndexList = new List<int>();    //��ǥ �ε��� ����Ʈ �ʱ�ȭ
        SpawnedPointIndexQueue = new Queue<int>();    //������ �� ť �ʱ�ȭ
        SpawnDelayTimerCaching = new WaitForSeconds(2.0f); //���� ������ Ÿ�̸� ĳ�̿� ���� �ʱ�ȭ
        AppearTimerCaching = new WaitForSeconds(15.0f); //���� �ð� Ÿ�̸� ĳ�̿� ���� �ʱ�ȭ
        
        NowRandomEnemyCount = 0;    //���� �ʵ忡 �ִ� ���� ���� 0���� �ʱ�ȭ
        ClearSpawnVirables();
        
        NowPulledObjectNumber = 0;
        IsSpawnCoroutineWork = false;
    }

    private void OnEnable()
    {
        StartSpawn();
    }

    /// <summary>
    /// �� ����� ���� ���� ��ǥ ������ ������ �ִ� ������Ʈ ã�� ���� ��������
    /// </summary>
    public void FindSpawnPosition()
    {
        RandomSpawnPointGameObj = GameObject.FindWithTag("RandomSpawnPoint");
        GetPointData();
        CertainSpawnPointObj = GameObject.FindWithTag("CertainSpawnPoint");
    }

    /// <summary>
    /// �� �̵��� �ʱ�ȭ �ؾ� �� ������ �ʱ�ȭ-���� �̵��ÿ��� X ������ �ٸ� ������ �̵��� ���� ȣ��
    /// </summary>
    public void ClearSpawnVirables()
    {
        RandomSpawnPointGameObj = null;
        CertainSpawnPointObj = null;
    }

    /// <summary>
    /// ���� ���� ��ǥ ����Ʈ Ŭ���� / ���� ������ ������ �и���
    /// </summary>
    public void ClearSpawnPointList()
    {
        RandomSpawnPointList.Clear();
    }

    /// <summary>
    /// �ε��� ť Ŭ����
    /// </summary>
    public void ClearSpawnedQueue()
    {
        SpawnedPointIndexQueue.Clear();
    }

    /// <summary>
    /// ���� ���� ��ǥ �Ѱ� ���� ������Ʈ�� �̿��ؼ� �ڽ� ���� ������Ʈ�� ��ȸ�ϸ� ��ǥ ����
    /// </summary>
    private void GetPointData()
    {
        int i = 0;

        foreach (Transform child in RandomSpawnPointGameObj.transform)
        {
            Vector3 PositionVec = new Vector3(child.position.x, child.position.y, child.position.z);
            RandomSpawnPointList.Add(new SpawnPointNode(PositionVec));
            RandomSpawnPointIndexList.Add(i);
            i++;
        }
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void StartSpawn()
    {
        SpawnCoroutineVa = StartCoroutine(RandomSpawnCoroutine());
    }

    /// <summary>
    /// ������ �� �̵� �Ǵ� ���� ���Խ� �ɺ� ���� ���� �� ��� �ɺ��� ������
    /// </summary>
    public void StopSpawn()
    {
        if (IsSpawnCoroutineWork)
        {
            StopCoroutine(SpawnCoroutineVa);
        }

        //�ʵ�� Ǯ���� ��� ������Ʈ ȸ�� �� �ʱ�ȭ
        for (int i = 0; i < PullingObjects.Count; i++)
        {
            if (PullingObjects[i].activeSelf)
            {
                PullingObjects[i].SetActive(false);
                PullingObjects[i].transform.position = new Vector3(1000, 1000, 1000);
            }
        }

        //ť�� �� �ִ� ��� ���� ��ǥ �ε����� �ε��� ����Ʈ�� ��ȯ
        while(SpawnedPointIndexQueue.Count != 0)
        {
            int IndexNumber = SpawnedPointIndexQueue.Dequeue(); //ť���� �ε��� ��ť
            RandomSpawnPointIndexList.Add(IndexNumber);
        }

        NowRandomEnemyCount = 0;    //���� �ʵ忡 �ִ� ���� ���� 0���� �ʱ�ȭ
        NowPulledObjectNumber = 0;
        IsSpawnCoroutineWork = false;
    }



    #region RandomSpawn
    /// <summary>
    /// 2�ʸ��� �ִ� ���� ���� ��ü���� �����ߴ°� Ȯ���ϰ� �������� �ʾҴٸ� ���ο� �ɺ��� �����. �� �� �ִ� ���� ���� ���ɼ��� �����ߴٸ� �ڷ�ƾ�� �����Ѵ�.
    /// </summary>
    /// <returns>2��</returns>
    private IEnumerator RandomSpawnCoroutine()
    {
        IsSpawnCoroutineWork = true;    //�۵� �����ϸ鼭 �ڷ�ƾ ���� Ȯ�� ���� true
        while (!(NowRandomEnemyCount >= MaxMonsterCount))
        {
            yield return SpawnDelayTimerCaching;    //2�ʸ��� �����Ѵ�

            if (NowRandomEnemyCount < MaxMonsterCount)
            {
                RandomSpawn();

                NowRandomEnemyCount++;
            }
        }
        IsSpawnCoroutineWork = false;   //�Ϸ� �� �ڷ�ƾ ���� Ȯ�� ���� false
    }

    /// <summary>
    /// ���� ��ǥ ���� Ǯ���ϵ��� �ϴ� �Լ�/�ִ� ���� ���� ��ü���� �����ϸ� �ڷ�ƾ�� Ż���ϰ� �� �Լ��� ȣ���ؼ� ������Ʈ ȸ�� �� ��Ǯ��
    /// </summary>
    private void RandomSpawn()
    {
        //���� ��ǥ �ε��� ���� �κ�
        int PickedIndex;    //������ ��ǥ �ε����� �޾ƿ� ����
        PickedIndex = PickRandomSpawnPoint();

        //�Ǹ� ���� ���� �κ�
        List<int> DemonDataIndexList = new List<int>(); //�ش� ������Ʈ�� ������ �� �Ǹ� ����
        SetRandomIndex(DemonDataIndexList);

        PullingSymbolObject(PickedIndex, DemonDataIndexList);
    }

    /// <summary>
    /// ������ ��ǥ ����
    /// </summary>
    /// <returns>����� �ε���</returns>
    private int PickRandomSpawnPoint()
    {
        int RandomNumber = Random.Range(0, RandomSpawnPointIndexList.Count);  //���� ���� ����
        int RandomIndexNumber = RandomSpawnPointIndexList[RandomNumber]; //���� ������ �ε����� ��ġ�ϴ� ����Ʈ �ε��� ����
        RandomSpawnPointIndexList.RemoveAt(RandomNumber);  //�ε��� ����Ʈ���� �ش� ��ġ�ϴ� �ε��� ����
        SpawnedPointIndexQueue.Enqueue(RandomIndexNumber);    //�ε��� ť�� �ش� ��ȣ �߰�

        return RandomIndexNumber;
    }

    /// <summary>
    /// ��ǥ�� ����Ʈ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    private void FetchRandomSpawnPoint()
    {
        int IndexNumber = SpawnedPointIndexQueue.Dequeue(); //ť���� �ε��� ��ť
        RandomSpawnPointIndexList.Add(IndexNumber);
    }

    /// <summary>
    /// Ǯ���� ������Ʈ�� �ʻ� ������
    /// </summary>
    /// <param name="PickedIndex"> ��ǥ ��ġ </param>
    /// <param name="DemonDataIndexList"></param>
    private void PullingSymbolObject(int PickedIndex, List<int> DemonDataIndexList)
    {
        GameObject NowPullObject = PullingObjects[NowPulledObjectNumber];
        EnemyObject NowPullObjectData = NowPullObject.GetComponent<EnemyObject>();

        NowPullObject.transform.position = RandomSpawnPointList[PickedIndex].PointData; //Ǯ���� ������Ʈ�� ��ġ�� ���� �ε����� ��ġ�� ��ǥ�� ����
        NowPullObjectData.SetEnemyIndexList(DemonDataIndexList);    //Ǯ���� ������Ʈ�� �Ǹ� ����Ʈ �־��ֱ�

        //�Լ� ȣ�� ���� �κ�
        NowPullObjectData.GetAppearTimeEvent(() => FetchSymbolObject(NowPullObject));  //15�ʰ� �Ǹ� ȸ�� �Լ� ȣ���ϵ��� ����

        NowPullObject.SetActive(true);  //�Ű��� ������Ʈ�� Ȱ��ȭ
        NowPullObjectData.SetTimer(AppearTimerCaching); //Ÿ�̸� ����

        NowPulledObjectNumber++;
        if (NowPulledObjectNumber == PullingObjects.Count)
            NowPulledObjectNumber = 0;  //Ǯ���� ������Ʈ �ε����� ������ �� �������� 0���� ���� �ٽ� ���
    }

    /// <summary>
    /// �ʻ� Ǯ���� ������Ʈ ȸ��
    /// </summary>
    private void FetchSymbolObject(GameObject PulledObject)
    {
        PulledObject.SetActive(false);  //ȸ���� ������Ʈ ��Ȱ��ȭ
        PulledObject.transform.position = new Vector3(1000, 1000, 1000); //ȸ���� ������Ʈ�� ��ġ�� �������� ������ �ű��
        RandomSpawn();
        FetchRandomSpawnPoint();
        //���� �ڸ����� �ٷ� �罺�� ���� �ʴ� ������ ���� �ٸ� ������ ���� �� �ε����� ť���� ȸ���Ѵ�
    }

    /// <summary>
    /// ���� �ε��� ���� ���� �� ����Ʈ�� �߰�
    /// </summary>
    /// <param name="MonsterIndexList"></param>
    private void SetRandomIndex(List<int> MonsterIndexList)
    {
        int MonsterCount = Random.Range(2, 4);    //���� �� ���� ���� �� �ִ� ��� �Ǹ��� ��: �ּ� 2, �ִ� 4

        for (int i = 0; i < MonsterCount; i++)
        {
            int NewMonsterIndex = Random.Range(1, (int)MonsterIndex.ExtraMobCount); //������ ���� �ε��� ����
            MonsterIndexList.Add(NewMonsterIndex);  //������ �ε����� �ӽ� ����Ʈ�� �߰�
        }
    }
  
    
    /*
     ������ ���� �ΰ������� ���Ͻ��ѳ����� ���� �� �����ϴ� �͵� �ٲ�� ��
    �ʵ�󿡼� ���̴� �𵨸��� ���� ������ ���͵��� �����Ǿ��ִµ� ������ �� � ������ ���� �������� ���� ����->���� ������ �����ϰ�->�˸��� ������ ������Ʈ�� ���� ���� ���� Ǯ���ϵ��� ���� �ʿ�
     �������� ���� ����X ������ �̰͵� �����ؾ�
     */

    #endregion



    #region FixedSpawn
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

    /// <summary>
    /// ���Ͱ� ������ ���� ��ǥ
    /// </summary>
    //private Vector3 SetRandomPosition()
    //{
    //    Vector3 VectorTmp = Random.insideUnitSphere * 50;
    //    NavMeshHit NavHitTmp;
    //    NavMesh.SamplePosition(VectorTmp, out NavHitTmp, 50, NavMesh.AllAreas);
    //
    //    return new Vector3(NavHitTmp.position.x, 0f, NavHitTmp.position.z);
    //}

    #endregion
}
