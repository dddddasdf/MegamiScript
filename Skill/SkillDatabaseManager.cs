using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

/// <summary>
/// init ��� ����� �����ϰ� ���ִ� �ܶ�. ������ �� ��
/// </summary>
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

//���� ������ ��ų ȿ������ ���ϵ� json�� �Ľ��� ������ �����ϴ� Ŭ����

#region Enums

public enum SkillTypeSort
{
    Physical,       //����
    Gun,            //��
    Fire,           //�Ʊ�-ȭ��
    Ice,            //����-����
    Electric,       //����-����
    Force,          //��-���
    Light,          //�ϸ�-��
    Dark,           //����-�ֻ�
    Almighty,       //�ޱ⵵-����
    //������� ���� �迭

    Recover,    //ȸ��
    Support,    //��Ÿ ����&�������
    Passive,     //�нú��
    Ailment      //�����̻��
}

/// <summary>
/// ��ų�� �����ϴ� ��ü�� ��
/// </summary>
public enum NumberOfTarget
{
    Single,     //����
    Multi,      //�ټ�
    All         //��ü
}

/// <summary>
/// ��ų ���
/// </summary>
public enum WhichTarget
{
    Foe,    //��
    Party,  //�Ʊ�
    Self    //�ڽŸ�
}

/// <summary>
/// �ΰ� ȿ�� �� �ο� �����̻� ����
/// </summary>
public enum EffectType
{
    Critical,       //ũ��Ƽ�� Ȯ�� ����
    DrainHP,        //HP ���
    DrainMP,        //MP ���
    DrainHPMP,      //HP, MP ���
    Drain,
    InstantKill,    //���
    //������� ���� �迭

    Buff,           //������
    Debuff,         //�������
    //������� ���� �迭

    RecoverAllHP,
    RecoverAil1,
    RecoverAil2,
    RecoverAllAil, 
    RecoverAllHPAil,
    RecoverHP,
    Recover,
    //������� ȸ�� �迭

    InflictSleep,
    InflictPoison,
    InflictSick,
    InflictPanic,
    InflictBind,
    Inflict,
    //������� �����̻� �迭 - ���� ��ų �� �����̻��� �ο��ϴ� ��ų�� �ִٸ� ���� ��� ����
}

/// <summary>
/// ����/������⿡�� ���� �� ���Ұ� ����Ǵ� ����
/// </summary>
[Flags]
public enum TargetStats
{
    None = 0,           //���� ��� ���� ����
    Atk = 1 << 0,    //���ݷ�
    Def = 1 << 1,   //����
    Acc = 1 << 2,  //���߷�
    Avd = 1 << 3      //ȸ����
}

#endregion


[System.Serializable][JsonObject(MemberSerialization.OptIn)]
public record SkillDataRec
{
    /// <summary>
    /// Init���� ���� �����ڸ� �������� ������ȭ�� �����ϴ�
    /// </summary>
    /// <param name="skillType"></param>
    /// <param name="iD"></param>
    /// <param name="rank"></param>
    /// <param name="name"></param>
    /// <param name="useMP"></param>
    /// <param name="target"></param>
    /// <param name="targetNuber"></param>
    /// <param name="minHits"></param>
    /// <param name="maxHits"></param>
    /// <param name="power"></param>
    /// <param name="accuracy"></param>
    /// <param name="description"></param>
    /// <param name="addEffect"></param>
    /// <param name="effectValue"></param>
    //public SkillDataRec(SkillTypeSort skillType, int iD, int rank, string name, int useMP, WhichTarget target, NumberOfTarget? targetNuber, int minHits, int maxHits, int? power,
    //    int accuracy, string description, EffectType? addEffect, int? effectValue)
    //{
    //    SkillType = skillType;
    //    ID = iD;
    //    Rank = rank;
    //    Name = name;
    //    UseMP = useMP;
    //    Target = target;
    //    thisTargetNumber = targetNuber;
    //    MinHits = minHits;
    //    MaxHits = maxHits;
    //    Power = power;
    //    Accuracy = accuracy;
    //    Description = description;
    //    AddEffect = addEffect;
    //    EffectValue = effectValue;
    //}

    
    [JsonProperty] private SkillTypeSort SkillType { get; init; } //��ų ����
    [JsonProperty] private int ID { get; init; }              //�ĺ� ID
    [JsonProperty] private int Rank { get; init; }            //��ų ��ũ
    [JsonProperty] private string Name { get; init; }         //��ų �̸�
    [JsonProperty] private int UseMP { get; init; }           //�Ҹ� ����
    [JsonProperty] private WhichTarget Target { get; init; }  //���� ���
    [JsonProperty] private NumberOfTarget thisTargetNumber { get; init; }   //��ų ��� ��ü ��
    [JsonProperty] private int MinHits { get; init; }         //�ּ� Ƚ��
    [JsonProperty] private int MaxHits { get; init; }         //�ִ� Ƚ��
    [JsonProperty] private int? Power { get; init; }          //����: ������ �ʿ� ���� ���� ��ų�� ��� null
    [JsonProperty] private int Accuracy { get; init; }        //���߷�
    [JsonProperty] private string Description { get; init; }  //��ų ����
    [JsonProperty] private EffectType? AddEffect { get; init; }        //�ΰ� ȿ��: ���� ��� null
    [JsonProperty] private int? EffectValue { get; init; }                    //�ΰ� ȿ���� ��ġ: ���� ��� null
    /*
    �ΰ� ȿ���� ��ġ�� ���� ���� ����
    -���� ��ų �� ũ��Ƽ���� ������ ���: ũ��Ƽ������ �����ϴ� ���� ��ġ
    -���� ��ų�� ���: ����� �ۼ�Ƽ��
    -�����̻� ��ų�� ���: �ش� �����̻��� �ο��� Ȯ��
    */
    [JsonProperty] private TargetStats? EffectStats { get; init; }        //�߰� ȿ��(���, ȸ��, ����, �������)���� ����� �Ǵ� ����

    /// <summary>
    /// ��ų�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public string ReturnName()
    {
        return Name;
    }

    /// <summary>
    /// ID�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnID()
    {
        return ID;
    }

    /// <summary>
    /// �Ҹ� ���� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnUseMP()
    {
        return UseMP;
    }

    /// <summary>
    /// ���� �� ��ȯ
    /// </summary>
    /// <returns>��ų�� ����</returns>
    public int? ReturnPower()
    {
        return Power;
    }

    /// <summary>
    /// �ּ� ��Ʈ �� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnMinHit()
    {
        return MinHits;
    }

    /// <summary>
    /// �ִ� ��Ʈ �� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int ReturnMaxHit()
    {
        return MaxHits;
    }

    /// <summary>
    /// ��ų Ÿ�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public SkillTypeSort ReturnSkillType()
    {
        return SkillType;
    }

    public NumberOfTarget ReturnNumberOfTarget()
    {
        return thisTargetNumber;
    }
}

/// <summary>
/// ������ ���ʹ� ������ �ʿ��� �ּ����� ������ ������ �ְ� �Ѵ�
/// </summary>
[System.Serializable]
[JsonObject(MemberSerialization.OptIn)]
public record SimplifySkillDataRec
{
    private int ID { get; init; }
    private string Name { get; init; }

    /// <summary>
    /// �Ǹ� �������� ����� �ʿ��� ��(���ΰ��� null�� ����) �����ϱ� ���� �����ؾ� �ϴ� ����
    /// </summary>
    private int? RequireLevel { get; init; }
}


public class SkillDatabaseManager
{
    //private static List<SkillDataBase> SkillDataBaseList;
    //private List<AttackSkillClass> AttackSkillList = new List<AttackSkillClass>();
    //private List<RecoverSkillClass> RecoverSkillList = new List<RecoverSkillClass>();
    //private List<SkillDataClass> SkillList = new List<SkillDataClass>();

    private List<SkillDataRec> SkillRecList = new List<SkillDataRec>();
    private Queue<Action> AfterInitJobQueue = new Queue<Action>();      //�ʱ�ȭ �۾� �� ���������� �����ų �۾� ���

    /// <summary>
    /// �ӽÿ�
    /// </summary>
    public void InitSkillDatabaseManager()
    {
        LoadSkillRecDatabasse();
    }


    private void LoadSkillRecDatabasse()
    {
        AsyncOperationHandle<TextAsset> TextAssetHandle;
        TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("SkillData");

        TextAssetHandle.Completed += Handle =>
        {
            if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
            {

            }
            SkillRecList = JsonConvert.DeserializeObject<List<SkillDataRec>>(TextAssetHandle.Result.text);

            while (AfterInitJobQueue.Count > 0)
            {
                Action action = AfterInitJobQueue.Dequeue();
                action?.Invoke();
            }
            AfterInitJobQueue = null;       //�۾�ť ����ֱ�
        };
    }

    public void AddJobQueueMethod(Action Method)
    {
        AfterInitJobQueue.Enqueue(Method);
    }

    /// <summary>
    /// ID���� ��ġ�ϴ� ��ų ��ȯ
    /// </summary>
    /// <param name="SkilliD"></param>
    /// <returns></returns>
    public SkillDataRec ReturnSkillData(int SkilliD)
    {
        return SkillRecList.Find(x => (x.ReturnID()) == SkilliD);
    }


    /*
    ���� �����ؾ� �ϴ� �޼ҵ�
    1. ID���� �Ķ���ͷ� ���� �޾Ƽ� ID���� ��ġ�ϴ� ���ڵ带 Ž�� �� �ش� ���ڵ� ��ȯ(���� ���簡 �ʿ����� ���� ���簡 �ʿ����� �� �� ��� ��)
    2. ��ų ������ �߻��ϸ� rank���� �Ķ���ͷ� ���� �޾Ƽ� �ٻ��� rank������ ���� ���ڵ���� �߸� �� rand�� ���� �ϳ� ���� �� ��ȯ
    
    */


    #region NotUse
    //
    ///// <summary>
    ///// ��ų ���� ��з�
    ///// </summary>
    //public enum SkillSort
    //{
    //    Attack,     //����
    //    Recover,    //ȸ��
    //    Support,    //��Ÿ ����&�������
    //    Passive,     //�нú��
    //    Ailment      //�����̻��
    //}


    ///// <summary>
    ///// ���� ��ų ����
    ///// </summary>
    //public enum AttackSkillType
    //{
    //    Physical,       //����
    //    Gun,            //��
    //    Fire,           //�Ʊ�
    //    Ice,            //�ν�
    //    Electric,       //����
    //    Force,          //��
    //    Light,          //�ϸ�
    //    Dark,           //����
    //    Almighty        //�ޱ⵵(����)
    //}

    ///// <summary>
    ///// ȸ�� ��ų ����
    ///// </summary>
    //public enum RecoverSkillyType
    //{
    //    RecoverHP
    //}



    ///// <summary>
    ///// ���� ��ų�� �ΰ� ȿ��
    ///// </summary>
    //public enum AddtionalEffect
    //{
    //    None,           //ȿ�� ����
    //    Critical,       //ũ��Ƽ�� Ȯ�� ����
    //    DrainHP,        //HP ���
    //    DrainMP,        //MP ���
    //    DrainHPMP,      //HP, MP ���

    //}


    ///// <summary>
    ///// ���� ��ų�� ȿ��
    ///// </summary>
    //public enum SupportEffect
    //{

    //}

    ///// <summary>
    ///// ���� ��ų ���
    ///// </summary>
    //public enum SupportTarget
    //{
    //    Self,       //�ڱ� �ڽ�
    //    Party,      //�Ʊ� ����
    //    Foe         //�� ����
    //}

    ///// <summary>
    ///// ����, ȸ��, ������ ���������� ������ �κ�
    ///// </summary>
    //public class Skill
    //{
    //    public string Name;         //��ų �̸�
    //    public int ID;              //�ĺ� ID
    //    public int UseMP;           //�Ҹ� ����
    //    public NumberOfTarget thisTargetNumber;   //��ų ��� ��ü ��
    //    public int MinHits;         //�ּ� Ƚ��
    //    public int MaxHits;         //�ִ� Ƚ��
    //    public int Rank;            //��ų ��ũ
    //    public int Accuracy;        //���߷�
    //    public string Description;  //��ų ����
    //}

    ///// <summary>
    ///// ���� ��ų Ŭ����
    ///// </summary>
    //[SerializeField]
    //public class AttackSkillClass : Skill
    //{
    //    public AttackSkillType thisSkillType;
    //    public int Power;     //����
    //    public AddtionalEffect AddEffect;       //�ΰ� ȿ��
    //    public int? Value;                      //�ΰ� ȿ���� ������ ��� ���� ��ġ-�ΰ� ȿ���� ���� ��� null
    //}

    ///// <summary>
    ///// ȸ�� ��ų Ŭ����
    ///// </summary>
    //[SerializeField]
    //public class RecoverSkillClass : Skill
    //{
    //    public RecoverSkillyType thisSkillType;
    //    public int Power;     //����
    //}

    ///// <summary>
    ///// ���� ��ų Ŭ����
    ///// </summary>
    //[SerializeField]
    //public class SupportSkillClass : Skill
    //{
    //    public SupportTarget Target;       //��ų ����� �Ʊ����� ������
    //    public SupportEffect Effect;    //���� ȿ��
    //    int? Value;                     //���� ȿ���� ��ġ
    //}
    //[System.Serializable]
    //public class SkillDataClass
    //{
    //    public SkillTypeSort SkillType; //��ų ����
    //    public int ID;              //�ĺ� ID
    //    public int Rank;            //��ų ��ũ
    //    public string Name;         //��ų �̸�
    //    public int UseMP;           //�Ҹ� ����
    //    public WhichTarget Target;  //���� ���
    //    public NumberOfTarget? thisTargetNumber;   //��ų ��� ��ü ��
    //    public int MinHits;         //�ּ� Ƚ��
    //    public int MaxHits;         //�ִ� Ƚ��
    //    public int? Power;          //����: ������ �ʿ� ���� ���� ��ų�� ��� null
    //    public int Accuracy;        //���߷�
    //    public string Description;  //��ų ����
    //    public EffectType? AddEffect;        //�ΰ� ȿ��: ���� ��� null
    //    public int? EffectValue;                    //�ΰ� ȿ���� ��ġ: ���� ��� null
    //    /*
    //    �ΰ� ȿ���� ��ġ�� ���� ���� ����
    //    -���� ��ų �� ũ��Ƽ���� ������ ���: ũ��Ƽ������ �����ϴ� ���� ��ġ
    //    -���� ��ų�� ���: ����� �ۼ�Ƽ��
    //    -�����̻� ��ų�� ���: �ش� �����̻��� �ο��� Ȯ��
    //    */
    //    public TargetStats? EffectStats;        //���� �Ǵ� ������⿡�� ����� �Ǵ� ����
    //}

    ////public string GetSkillInfo(SkillSort GetSkillType, int SkilliD)
    ////{
    ////    switch(GetSkillType)
    ////    {
    ////        case SkillSort.Attack:
    ////            return AttackSkillList.Find(x => x.ID == SkilliD).Description;
    ////        //case SkillSort.Recover:
    ////        //    break;
    ////        //case SkillSort.Support:
    ////        //    break;
    ////        //case SkillSort.Ailment:
    ////        //    break;
    ////        //case SkillSort.Passive:
    ////            //break;
    ////        default:
    ////            return "������ ����";
    ////    }
    ////}

    /////// <summary>
    /////// ��ų DB �ҷ����̱�
    /////// </summary>
    ////private void LoadSkillDatabasse()
    ////{
    ////    AsyncOperationHandle<TextAsset> TextAssetHandle;
    ////    TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("SkillData");

    ////    TextAssetHandle.Completed += Handle =>
    ////    {
    ////        if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
    ////        {

    ////        }
    ////        SkillList = JsonConvert.DeserializeObject<List<SkillDataClass>>(TextAssetHandle.Result.text);
    ////    };
    ////}

    ///// <summary>
    ///// �Ǹ� DB���� ������ ������ ������ Ŭ����: �̰� ���� �����ϴ� ��ų �����͸� ã�´�
    ///// </summary>
    //public record SkillCell
    //{
    //    public string Name;
    //    public SkillTypeSort thisSkillType;
    //    public int SkilliD;
    //}
    
    #endregion

}
