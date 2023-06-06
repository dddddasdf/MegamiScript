using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

[System.Serializable]
[JsonObject(MemberSerialization.OptIn)]
public record EnemyDataRec
{
    #region Field

    [JsonProperty] private int ID { get; init; }
    [JsonProperty] private string Name { get; init; }
    [JsonProperty] private string Tribe { get; init; }
    [JsonProperty] private int Level { get; init; }
    [JsonProperty] private int HP { get; init; }
    [JsonProperty] private int MP { get; init; }
    [JsonProperty] private int St { get; init; }
    [JsonProperty] private int Dx { get; init; }
    [JsonProperty] private int Ma { get; init; }
    [JsonProperty] private int Ag { get; init; }
    [JsonProperty] private int Lu { get; init; }
    [JsonProperty] private SkillAffinities? AffinityPhysical { get; init; }
    [JsonProperty] private SkillAffinities? AffinityGun { get; init; }
    [JsonProperty] private SkillAffinities? AffinityFire { get; init; }
    [JsonProperty] private SkillAffinities? AffinityIce { get; init; }
    [JsonProperty] private SkillAffinities? AffinityElectric { get; init; }
    [JsonProperty] private SkillAffinities? AffinityForce { get; init; }
    [JsonProperty] private SkillAffinities? AffinityLight { get; init; }
    [JsonProperty] private SkillAffinities? AffinityDark { get; init; }
    [JsonProperty] private int? IDofSkill1 { get; init; }
    [JsonProperty] private int? LearnLevel1 { get; init; }
    [JsonProperty] private int? IDofSkill2 { get; init; }
    [JsonProperty] private int? LearnLevel2 { get; init; }
    [JsonProperty] private int? IDofSkill3 { get; init; }
    [JsonProperty] private int? LearnLevel3 { get; init; }
    [JsonProperty] private int? IDofSkill4 { get; init; }
    [JsonProperty] private int? LearnLevel4 { get; init; }
    [JsonProperty] private int? IDofSkill5 { get; init; }
    [JsonProperty] private int? LearnLevel5 { get; init; }
    [JsonProperty] private int? IDofSkill6 { get; init; }
    [JsonProperty] private int? LearnLevel6 { get; init; }
    [JsonProperty] private int? IDofSkill7 { get; init; }
    [JsonProperty] private int? LearnLevel7 { get; init; }
    [JsonProperty] private bool IsNormalGun { get; init; }        //일반 공격이 총속성일 경우 true, 그 외에는 false
    [JsonProperty] private int NormalMinHit { get; init; }        //일반 공격의 최소 타수: 일부 악마를 제외하면 통상적으로는 1회
    [JsonProperty] private int NormalMaxHit { get; init; }        //일반 공격의 최대 타수: 일부 악마를 제외하면 통상적으로는 1회

    //추후 상태이상 관련 약점도 추가

    #endregion

    #region ReturnMethod
    public int ReturnID()
    {
        return ID;
    }

    public string ReturnName()
    {
        return Name;
    }

    public string ReturnTrbie()
    {
        return Tribe;
    }

    public int ReturnLevel()
    {
        return Level;
    }

    public int ReturnHP()
    {
        return HP;
    }

    public int ReturnMP()
    {
        return HP;
    }

    public int ReturnSt()
    {
        return St;
    }

    public int ReturnDx()
    {
        return Dx;
    }

    public int ReturnMa()
    {
        return Ma;
    }

    public int ReturnAg()
    {
        return Ag;
    }

    public int ReturnLu()
    {
        return Lu;
    }

    /// <summary>
    /// 각 공격 스킬 속성에 대한 약점~강점 반환
    /// </summary>
    /// <param name="AttackSkill"></param>
    /// <returns></returns>
    public SkillAffinities? ReturnAffinity(SkillTypeSort AttackSkill)
    {
        switch (AttackSkill)
        {
            case SkillTypeSort.Physical:
                return AffinityPhysical;
            case SkillTypeSort.Gun:
                return AffinityGun;
            case SkillTypeSort.Fire:
                return AffinityFire;
            case SkillTypeSort.Ice:
                return AffinityIce;
            case SkillTypeSort.Electric:
                return AffinityElectric;
            case SkillTypeSort.Force:
                return AffinityForce;
            case SkillTypeSort.Light:
                return AffinityLight;
            case SkillTypeSort.Dark:
                return AffinityDark;
            default:
                return null;
        }
    }

    public int ReturnLearnSkillLevel(int Number)
    { 
        switch (Number)
        {
            case 1:
                return LearnLevel1 ?? -1;   //null일 경우 -1 반환, 그렇지 않으면 본래값 반환
                //if (LearnLevel1.HasValue)
                //    return LearnLevel1.Value;
                //else
                //    return -1;
            case 2:
                if (LearnLevel2.HasValue)
                    return LearnLevel1.Value;
                else
                    return -1;
            case 3:
                if (LearnLevel3.HasValue)
                    return LearnLevel1.Value;
                else
                    return -1;
            case 4:
                if (LearnLevel4.HasValue)
                    return LearnLevel1.Value;
                else
                    return -1;
            case 5:
                if (LearnLevel5.HasValue)
                    return LearnLevel1.Value;
                else
                    return -1;
            case 6:
                if (LearnLevel6.HasValue)
                    return LearnLevel1.Value;
                else
                    return -1;
            case 7:
                if (LearnLevel7.HasValue)
                    return LearnLevel1.Value;
                else
                    return -1;
            default:
                return -1;
        }
    }

    public int ReturnLearnSkiLLID(int Number)
    {
        switch (Number)
        {
            case 1:
                if (IDofSkill1.HasValue)
                    return IDofSkill1.Value;
                else
                    return -1;
            case 2:
                if (IDofSkill2.HasValue)
                    return IDofSkill2.Value;
                else
                    return -1;
            case 3:
                if (IDofSkill3.HasValue)
                    return IDofSkill3.Value;
                else
                    return -1;
            case 4:
                if (IDofSkill4.HasValue)
                    return IDofSkill4.Value;
                else
                    return -1;
            case 5:
                if (IDofSkill5.HasValue)
                    return IDofSkill5.Value;
                else
                    return -1;
            case 6:
                if (IDofSkill6.HasValue)
                    return IDofSkill6.Value;
                else
                    return -1;
            case 7:
                if (IDofSkill7.HasValue)
                    return IDofSkill7.Value;
                else
                    return -1;
            default:
                return -1;
        }
    }

    public bool ReturnIsNormalGun()
    {
        return IsNormalGun;
    }

    public int ReturnNormalMinHit()
    {
        return NormalMinHit;
    }

    public int ReturnNormalMaxHit()
    {
        return NormalMaxHit;
    }

    #endregion
}

public class EnemyDatabaseManager
{
    private List<EnemyDataRec> EnemyRecList = new List<EnemyDataRec>();

    private Queue<Action> AfterInitJobQueue = new Queue<Action>();      //초기화 작업 후 순차적으로 진행시킬 작업 목록
    public void AddJobQueueMethod(Action Method)
    {
        AfterInitJobQueue.Enqueue(Method);
    }

    private static List<int> BattleEnemyListTmp;   //전투 돌입하기 전 부딪친 몹 객체로부터 임시로 몹 인덱스 리스트를 받아 저장하는 용도

    public void InitEnemyDatabaseManager()
    {
        LoadEnemyRecDatabase();
    }

    private void LoadEnemyRecDatabase()
    {
        AsyncOperationHandle<TextAsset> TextAssetHandle;
        TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("EnemyData");

        TextAssetHandle.Completed += Handle =>
        {
            if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
            {

            }
            EnemyRecList = JsonConvert.DeserializeObject<List<EnemyDataRec>>(TextAssetHandle.Result.text);

            while (AfterInitJobQueue.Count > 0)
            {
                Action action = AfterInitJobQueue.Dequeue();
                action?.Invoke();
            }
            AfterInitJobQueue = null;       //작업큐 비워주기
        };
    }

    /// <summary>
    /// 몬스터의 인덱스를 받아 일치하는 몬스터의 데이터를 돌려주는 함수
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="Name"></param>
    /// <param name="HP"></param>
    /// <param name="Power"></param>
//    public void ReturnEnemyData(int Index, out string Name, out int HP, out int Power)
//    {
//        EnemyDataRec DataTmp = null;    //null로 초기화
//        DataTmp = EnemyRecList.Find(Data => Data.ReturnID() == Index);     //ID가 일치하는 데이터 찾기



//#if UNITY_EDITOR
//        if (DataTmp == null)
//            Debug.Log("데이터 오류 확인 요망");
//#endif

//        //Name = DataTmp.Name;
//        //HP = DataTmp.HP;
//        //Power = DataTmp.Power;

//        //조합인가, 아니면 일일이 변수 지정인가?
//    }

    /// <summary>
    /// 몬스터의 인덱스를 받아 일치하는 몬스터의 데이터를 돌려주는 함수
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public EnemyDataRec ReturnEnemyData(int Index)
    {
        EnemyDataRec DataTmp = null;    //null로 초기화
        DataTmp = EnemyRecList.Find(Data => Data.ReturnID() == Index);     //ID가 일치하는 데이터 찾기

#if UNITY_EDITOR
        if (DataTmp == null)
            Debug.Log("데이터 오류 확인 요망");
#endif

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
