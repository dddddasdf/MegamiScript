using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyRespawnManager : MonoBehaviour
{
    #region SetVariables
    private int MaxMonsterCount = 7;    //필드에 동시에 존재할 수 있는 최대 몬스터 마리수->맵별로 다르게 할 건지 고민 중인데 일단은 테스트용이므로 임의로 지정
    private int NowRandomEnemyCount;
    [SerializeField] private GameObject EnemyPrefab;

    /// <summary>
    /// 리스폰 코루틴 저장용 변수
    /// </summary>
    private Coroutine SpawnCoroutineVa;
    private bool IsSpawnCoroutineWork;  //코루틴 동작 확인용 변수

    /// <summary>
    /// yield WaitForSeconds 캐싱용, 심볼은 최대 동시 존재 가능수 도달 전까지 2초마다 생성된다
    /// </summary>
    private WaitForSeconds SpawnDelayTimerCaching;
    private WaitForSeconds AppearTimerCaching;

    /// <summary>
    /// 랜덤 스폰 위치 정보를 가지고 있는 게임 오브젝트
    /// </summary>
    private GameObject RandomSpawnPointGameObj;
    private GameObject CertainSpawnPointObj;

    /// <summary>
    /// 풀링용 오브젝트 리스트
    /// </summary>
    [SerializeField] private List<GameObject> PullingObjects;

    private int NowPulledObjectNumber;  //현재 풀링된 오브젝트 숫자 기록용

    //좌표 관련 변수
    /// <summary>
    /// 심볼 생성 좌표 데이터 저장용 클래스
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
    /// 좌표 저장용 리스트
    /// </summary>
    private List<SpawnPointNode> RandomSpawnPointList;

    /// <summary>
    /// 위 리스트의 "번호"를 저장할 리스트
    /// </summary>
    private List<int> RandomSpawnPointIndexList;

    /// <summary>
    /// 스폰된 몹 인덱스 "번호" 저장용 큐
    /// </summary>
    private Queue<int> SpawnedPointIndexQueue;

    #endregion


    private void Awake()
    {
        NowRandomEnemyCount = 0;    //현재 필드에 있는 몬스터 숫자 0으로 초기화
        ClearSpawnVirables();
        RandomSpawnPointList = new List<SpawnPointNode>();  //좌표 리스트 초기화
        RandomSpawnPointIndexList = new List<int>();    //좌표 인덱스 리스트 초기화
        SpawnedPointIndexQueue = new Queue<int>();    //스폰된 몹 큐 초기화
        SpawnDelayTimerCaching = new WaitForSeconds(2.0f); //스폰 딜레이 타이머 캐싱용 변수 초기화
        AppearTimerCaching = new WaitForSeconds(15.0f); //등장 시간 타이머 캐싱용 변수 초기화
        
        NowPulledObjectNumber = 0;
        IsSpawnCoroutineWork = false;
    }

    private void OnEnable()
    {
        SpawnCoroutineVa = StartCoroutine(RandomSpawnCoroutine());
    }

    /// <summary>
    /// 맵 입장시 랜덤 스폰 좌표 정보를 가지고 있는 오브젝트 찾고 정보 가져오기
    /// </summary>
    public void FindSpawnPosition()
    {
        RandomSpawnPointGameObj = GameObject.FindWithTag("RandomSpawnPoint");
        GetPointData();
        CertainSpawnPointObj = GameObject.FindWithTag("CertainSpawnPoint");
    }

    /// <summary>
    /// 맵 이동시 초기화 해야 할 변수들 초기화
    /// </summary>
    public void ClearSpawnVirables()
    {
        RandomSpawnPointGameObj = null;
        CertainSpawnPointObj = null;
    }

    /// <summary>
    /// 랜덤 스폰 좌표 리스트 클리어 / 오류 때문에 별도로 분리함
    /// </summary>
    public void ClearSpawnPointList()
    {
        RandomSpawnPointList.Clear();
    }

    /// <summary>
    /// 인덱스 큐 클리어
    /// </summary>
    public void ClearSpawnedQueue()
    {
        SpawnedPointIndexQueue.Clear();
    }

    /// <summary>
    /// 랜덤 스폰 좌표 총괄 게임 오브젝트를 이용해서 자식 게임 오브젝트들 순회하며 좌표 저장
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
    /// 유저가 맵 이동 또는 전투 돌입시 심볼 스폰 중지 및 모든 심볼이 삭제됨
    /// </summary>
    public void StopSpawn()
    {
        if (IsSpawnCoroutineWork)
        {
            StopCoroutine(SpawnCoroutineVa);
        }
    }




    #region RandomSpawn
    /// <summary>
    /// 2초마다 최대 동시 존재 개체수에 도달했는가 확인하고 도달하지 않았다면 새로운 심볼을 만든다. 한 번 최대 동시 존재 가능수에 도달했다면 코루틴을 종료한다.
    /// </summary>
    /// <returns>2초</returns>
    private IEnumerator RandomSpawnCoroutine()
    {
        IsSpawnCoroutineWork = true;    //작동 시작하면서 코루틴 동작 확인 변수 true
        while (!(NowRandomEnemyCount >= MaxMonsterCount))
        {
            yield return SpawnDelayTimerCaching;    //2초마다 스폰한다

            if (NowRandomEnemyCount < MaxMonsterCount)
            {
                RandomSpawn();

                NowRandomEnemyCount++;
            }
        }
        IsSpawnCoroutineWork = false;   //완료 후 코루틴 동작 확인 변수 false
    }

    /// <summary>
    /// 최대 동시 존재 개체수에 도달하면 이후로 이 함수를 호출해서 오브젝트 회수 및 재풀링
    /// </summary>
    private void RandomSpawn()
    {
        int PickedIndex;
        PickedIndex = PickRandomSpawnPoint();
        PullingSymbolObject(PickedIndex);
    }

    /// <summary>
    /// 랜덤한 좌표 선택
    /// </summary>
    /// <returns>추출된 인덱스</returns>
    private int PickRandomSpawnPoint()
    {
        int RandomNumber = Random.Range(0, RandomSpawnPointIndexList.Count);  //랜덤 숫자 추출
        int RandomIndexNumber = RandomSpawnPointIndexList[RandomNumber]; //랜덤 숫자의 인덱스에 위치하던 포인트 인덱스 추출
        RandomSpawnPointIndexList.RemoveAt(RandomNumber);  //인덱스 리스트에서 해당 일치하는 인덱스 제거
        SpawnedPointIndexQueue.Enqueue(RandomIndexNumber);    //인덱스 큐에 해당 번호 추가

        return RandomIndexNumber;
    }

    /// <summary>
    /// 좌표를 리스트로 반환
    /// </summary>
    /// <returns></returns>
    private void FetchRandomSpawnPoint()
    {
        int IndexNumber = SpawnedPointIndexQueue.Dequeue(); //큐에서 인덱스 디큐
        RandomSpawnPointIndexList.Add(IndexNumber);
    }

    /// <summary>
    /// 풀링용 오브젝트를 맵상에 릴리즈
    /// </summary>
    /// <param name="PickedIndex"> 좌표 위치 </param>
    private void PullingSymbolObject(int PickedIndex)
    {
        GameObject NowPullObject = PullingObjects[NowPulledObjectNumber];
        NowPullObject.transform.position = RandomSpawnPointList[PickedIndex].PointData; //풀링할 오브젝트의 위치를 뽑힌 인덱스에 위치한 좌표로 갱신
        NowPullObject.GetComponent<EnemyObject>().AppearTimerEvent += () =>
        {
            FetchSymbolObject(NowPullObject);    //15초가 되면 회수 함수 호출하도록 예약
        };
        NowPullObject.SetActive(true);  //옮겨진 오브젝트를 활성화
        NowPullObject.GetComponent<EnemyObject>().SetTimer(AppearTimerCaching); //타이머 설정

        NowPulledObjectNumber++;
        if (NowPulledObjectNumber == PullingObjects.Count)
            NowPulledObjectNumber = 0;  //풀링용 오브젝트 인덱스를 끝까지 다 돌렸으면 0으로 가서 다시 대기
    }

    /// <summary>
    /// 맵상에 풀링된 오브젝트 회수
    /// </summary>
    private void FetchSymbolObject(GameObject PulledObject)
    {
        Debug.Log("회수");
        PulledObject.SetActive(false);  //회수한 오브젝트 비활성화
        PulledObject.transform.position = new Vector3(1000, 1000, 1000); //회수한 오브젝트의 위치를 동떨어진 곳으로 옮기기
        RandomSpawn();
        FetchRandomSpawnPoint();
        //같은 자리에서 바로 재스폰 하지 않는 구조라 먼저 다른 지점에 생성 후 인덱스를 큐에서 회수한다
    }




    //아래에 있는 코드들은 전부 무시하라 처음부터 다시 짜야한다


    /// <summary>
    /// 몬스터 인덱스 랜덤 생성 후 리스트에 추가
    /// </summary>
    /// <param name="MonsterIndexList"></param>
    private void SetRandomIndex(List<int> MonsterIndexList)
    {
        int MonsterCount = Random.Range(1, MaxMonsterCount);    //1마리부터 최대 마릿수까지 전투 한 번 당 적마릿수 랜덤 생성

        for (int i = 0; i < MonsterCount; i++)
        {
            int NewMonsterIndex = Random.Range(1, (int)MonsterIndex.ExtraMobCount); //랜덤한 몬스터 인덱스 생성
            MonsterIndexList.Add(NewMonsterIndex);  //생성한 인덱스를 임시 리스트에 추가
        }
    }



    #endregion



    #region FixedSpawn
    //고정 위치에서 고정된 값으로 스폰되는 몬스터들
    //빈 오브젝트에 충돌체와 값을 미리 넣어두고 유저가 해당 위치를 지나갈 경우 스폰되는 식으로 짜둘 예정
    //일단 랜럼 생성부터 하고 보자


    #endregion

    #region Dummy

    /// <summary>
    /// 몬스터 인덱스 임시 배열 초기화 및 배열에 랜덤 인덱스 추가
    /// </summary>
    /// <param name="MonsterIndexArray"></param>
    //private void SetRandomIndexArray(out int[] MonsterIndexArray)
    //{
    //    int MonsterCount = Random.Range(1, MaxMonsterCount);    //1마리부터 최대 마릿수까지 전투 한 번 당 적마릿수 랜덤 생성
    //    MonsterIndexArray = new int[MonsterCount];

    //    for (int i = 0; i < MonsterCount; i++)
    //    {
    //        int NewMonsterIndex = Random.Range(1, (int)MonsterIndex.ExtraMobCount); //랜덤한 몬스터 인덱스 생성
    //        MonsterIndexArray[i] = NewMonsterIndex;  //생성한 인덱스를 임시 배열에 추가
    //    }
    //}

    /// <summary>
    /// 몬스터가 생성될 랜덤 좌표
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
