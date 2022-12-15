using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyRespawn : MonoBehaviour
{
    private int MaxMonsterCount = 5;    //필드에 동시에 존재할 수 있는 최대 몬스터 마리수
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
        NowRandomEnemyCount = 0;    //필드상에 나와 있는 몬스터 카운트 0으로 초기화
        RespawnCoroutineVa = StartCoroutine(RespawnCoroutine());
    }

    public void StopRespawn()
    {
        if (RespawnCoroutineVa != null)
            StopCoroutine(RespawnCoroutineVa);
    }

    /// <summary>
    /// 랜덤 위치에 몬스터 생성
    /// </summary>
    private IEnumerator RespawnCoroutine()
    {
        while (!(NowRandomEnemyCount == MaxMonsterCount))
        {
            GameObject NewEnemy = Instantiate(EnemyPrefab); //새 몬스터 개체 생성

            //해당 몬스터 필드 개체가 갖게 될 몬스터 인덱스 리스트
            List<int> MonsterIndexList = new List<int>();
            SetRandomIndex(MonsterIndexList);
            NewEnemy.GetComponent<EnemyObject>().SetEnemyIndexList(MonsterIndexList);

            //몬스터 리스폰 위치
            NewEnemy.transform.position = SetRandomPosition();

            NowRandomEnemyCount++;  //현재 필드 상에 나와 있는 몬스터 카운트 증가
            yield return null;
        }
    }

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

    /// <summary>
    /// 몬스터가 생성될 랜덤 좌표
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

    #endregion
}
