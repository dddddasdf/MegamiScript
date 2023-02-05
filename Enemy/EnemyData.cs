using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

public class EnemyData
{
    private static List<MonsterData> MonsterDataList;   //���� ���� ������ ����Ʈ, ���� �Ŵ����� ������ ���� �� ���ʷ� �� �� �ʱ�ȭ �����ְ� ���ķ� �� ����Ʈ�� �������� ���ȴ�

    private static List<int> BattleEnemyListTmp;   //���� �����ϱ� �� �ε�ģ �� ��ü�κ��� �ӽ÷� �� �ε��� ����Ʈ�� �޾� �����ϴ� �뵵

    /// <summary>
    /// ȣ���
    /// </summary>
    public EnemyData()
    {
        
    }

    /// <summary>
    /// ���� ������ json ���� ����Ʈ
    /// </summary>
    public static void InitEnemyData()
    {
        MonsterDataList = new List<MonsterData>();

        AsyncOperationHandle<TextAsset> LoadHandle = Addressables.LoadAssetAsync<TextAsset>("MonsterData");

#if UNITY_EDITOR
        if (LoadHandle.Status == AsyncOperationStatus.Failed)
        {
            Debug.Log("���� ������ ���� �ε� ����");
        }

#endif
        LoadHandle.Completed += Handle =>
        {
            //���� ������ ������ ���� �о�鿴���� ������ȭ �Ͽ� ����Ʈ�� ����
            TextAsset DataStack = LoadHandle.Result;
            MonsterDataList = JsonConvert.DeserializeObject<List<MonsterData>>(DataStack.text);
            Addressables.Release(LoadHandle);   //����Ʈ�� ������ ���� �Ϸ������Ƿ� ��巹���� ����
        };


        BattleEnemyListTmp = new List<int>(); //
    }

    /// <summary>
    /// ������ �ε����� �޾� ��ġ�ϴ� ������ �����͸� �����ִ� �Լ�
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
            Debug.Log("������ ���� Ȯ�� ���");
#endif

        Name = DataTmp.Name;
        HP = DataTmp.HP;
        Power = DataTmp.Power;
    }

    /// <summary>
    /// ������ �ε����� �޾� ��ġ�ϴ� ������ �����͸� �����ִ� �Լ�
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
