using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

public class EnemyData
{
    private static List<MonsterData> MonsterDataList;   //정적 몬스터 데이터 리스트, 게임 매니저가 게임이 켜질 때 최초로 한 번 초기화 시켜주고 이후로 이 리스트는 정적으로 사용된다

    private static List<int> BattleEnemyListTmp;   //전투 돌입하기 전 부딪친 몹 객체로부터 임시로 몹 인덱스 리스트를 받아 저장하는 용도

    /// <summary>
    /// 호출용
    /// </summary>
    public EnemyData()
    {
        
    }

    /// <summary>
    /// 몬스터 데이터 json 파일 임포트
    /// </summary>
    public static void InitEnemyData()
    {
        MonsterDataList = new List<MonsterData>();

        AsyncOperationHandle<TextAsset> LoadHandle = Addressables.LoadAssetAsync<TextAsset>("MonsterData");

#if UNITY_EDITOR
        if (LoadHandle.Status == AsyncOperationStatus.Failed)
        {
            Debug.Log("몬스터 데이터 파일 로드 실패");
        }

#endif
        LoadHandle.Completed += Handle =>
        {
            //몬스터 데이터 파일을 전부 읽어들였으면 역직렬화 하여 리스트에 저장
            TextAsset DataStack = LoadHandle.Result;
            MonsterDataList = JsonConvert.DeserializeObject<List<MonsterData>>(DataStack.text);
            Addressables.Release(LoadHandle);   //리스트에 데이터 저장 완료했으므로 어드레서블 해제
        };


        BattleEnemyListTmp = new List<int>(); //
    }

    /// <summary>
    /// 몬스터의 인덱스를 받아 일치하는 몬스터의 데이터를 돌려주는 함수
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="Name"></param>
    /// <param name="HP"></param>
    /// <param name="Power"></param>
    public void ReturnMonsterData(int Index, out string Name, out int HP, out int Power)
    {
        MonsterData DataTmp = MonsterDataList.Find(Data => Data.Index == Index);

#if UNITY_EDITOR
        if (DataTmp.Name == null)
            Debug.Log("데이터 오류 확인 요망");
#endif

        Name = DataTmp.Name;
        HP = DataTmp.HP;
        Power = DataTmp.Power;
    }

    /// <summary>
    /// 몬스터의 인덱스를 받아 일치하는 몬스터의 데이터를 돌려주는 함수
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public MonsterData ReturnEnemyData(int Index)
    {
        MonsterData DataTmp = MonsterDataList.Find(Data => Data.Index == Index);

        return DataTmp;
    }

    public static void SetIndexList(List<int> EnemyIndexList)
    {
        BattleEnemyListTmp = EnemyIndexList;
    }

    /// <summary>
    /// 임시용 인덱스 리스트 초기화
    /// </summary>
    public void ClearList()
    {
        BattleEnemyListTmp.Clear();
    }

    /// <summary>
    /// 시용 인덱스 리스트 반환
    /// </summary>
    /// <returns></returns>
    public List<int> ReturnList()
    {
        return BattleEnemyListTmp;
    }
}
