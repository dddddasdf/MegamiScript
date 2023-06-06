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
    [JsonProperty] private bool IsNormalGun { get; init; }        //�Ϲ� ������ �ѼӼ��� ��� true, �� �ܿ��� false
    [JsonProperty] private int NormalMinHit { get; init; }        //�Ϲ� ������ �ּ� Ÿ��: �Ϻ� �Ǹ��� �����ϸ� ��������δ� 1ȸ
    [JsonProperty] private int NormalMaxHit { get; init; }        //�Ϲ� ������ �ִ� Ÿ��: �Ϻ� �Ǹ��� �����ϸ� ��������δ� 1ȸ

    //���� �����̻� ���� ������ �߰�

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
    /// �� ���� ��ų �Ӽ��� ���� ����~���� ��ȯ
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
                return LearnLevel1 ?? -1;   //null�� ��� -1 ��ȯ, �׷��� ������ ������ ��ȯ
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

    private Queue<Action> AfterInitJobQueue = new Queue<Action>();      //�ʱ�ȭ �۾� �� ���������� �����ų �۾� ���
    public void AddJobQueueMethod(Action Method)
    {
        AfterInitJobQueue.Enqueue(Method);
    }

    private static List<int> BattleEnemyListTmp;   //���� �����ϱ� �� �ε�ģ �� ��ü�κ��� �ӽ÷� �� �ε��� ����Ʈ�� �޾� �����ϴ� �뵵

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
            AfterInitJobQueue = null;       //�۾�ť ����ֱ�
        };
    }

    /// <summary>
    /// ������ �ε����� �޾� ��ġ�ϴ� ������ �����͸� �����ִ� �Լ�
    /// </summary>
    /// <param name="Index"></param>
    /// <param name="Name"></param>
    /// <param name="HP"></param>
    /// <param name="Power"></param>
//    public void ReturnEnemyData(int Index, out string Name, out int HP, out int Power)
//    {
//        EnemyDataRec DataTmp = null;    //null�� �ʱ�ȭ
//        DataTmp = EnemyRecList.Find(Data => Data.ReturnID() == Index);     //ID�� ��ġ�ϴ� ������ ã��



//#if UNITY_EDITOR
//        if (DataTmp == null)
//            Debug.Log("������ ���� Ȯ�� ���");
//#endif

//        //Name = DataTmp.Name;
//        //HP = DataTmp.HP;
//        //Power = DataTmp.Power;

//        //�����ΰ�, �ƴϸ� ������ ���� �����ΰ�?
//    }

    /// <summary>
    /// ������ �ε����� �޾� ��ġ�ϴ� ������ �����͸� �����ִ� �Լ�
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public EnemyDataRec ReturnEnemyData(int Index)
    {
        EnemyDataRec DataTmp = null;    //null�� �ʱ�ȭ
        DataTmp = EnemyRecList.Find(Data => Data.ReturnID() == Index);     //ID�� ��ġ�ϴ� ������ ã��

#if UNITY_EDITOR
        if (DataTmp == null)
            Debug.Log("������ ���� Ȯ�� ���");
#endif

        return DataTmp;
    }

    public static void SetIndexList(List<int> EnemyIndexList)
    {
        BattleEnemyListTmp = EnemyIndexList;
    }

    /// <summary>
    /// �ӽÿ� �ε��� ����Ʈ �ʱ�ȭ
    /// </summary>
    public void ClearList()
    {
        BattleEnemyListTmp.Clear();
    }

    /// <summary>
    /// �ÿ� �ε��� ����Ʈ ��ȯ
    /// </summary>
    /// <returns></returns>
    public List<int> ReturnList()
    {
        return BattleEnemyListTmp;
    }
}
