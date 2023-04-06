using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

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


[System.Serializable]
public record SkillDataRec
{
    private SkillTypeSort SkillType { get; init; } //��ų ����
    private int ID { get; init; }              //�ĺ� ID
    private int Rank { get; init; }            //��ų ��ũ
    private string Name { get; init; }         //��ų �̸�
    private int UseMP { get; init; }           //�Ҹ� ����
    private WhichTarget Target { get; init; }  //���� ���
    private NumberOfTarget? thisTargetNumber { get; init; }   //��ų ��� ��ü ��
    private int MinHits { get; init; }         //�ּ� Ƚ��
    private int MaxHits { get; init; }         //�ִ� Ƚ��
    private int? Power { get; init; }          //����: ������ �ʿ� ���� ���� ��ų�� ��� null
    private int Accuracy { get; init; }        //���߷�
    private string Description { get; init; }  //��ų ����
    private EffectType? AddEffect { get; init; }        //�ΰ� ȿ��: ���� ��� null
    private int? EffectValue { get; init; }                    //�ΰ� ȿ���� ��ġ: ���� ��� null
    /*
    �ΰ� ȿ���� ��ġ�� ���� ���� ����
    -���� ��ų �� ũ��Ƽ���� ������ ���: ũ��Ƽ������ �����ϴ� ���� ��ġ
    -���� ��ų�� ���: ����� �ۼ�Ƽ��
    -�����̻� ��ų�� ���: �ش� �����̻��� �ο��� Ȯ��
    */
    private TargetStats? EffectStats { get; init; }        //�߰� ȿ��(���, ȸ��, ����, �������)���� ����� �Ǵ� ����

    /// <summary>
    /// ��ų�� ��ȯ
    /// </summary>
    /// <returns></returns>
    public string ReturnName()
    {
        return Name;
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
}


public class SkillDatabaseManager
{
    //private static List<SkillDataBase> SkillDataBaseList;
    //private List<AttackSkillClass> AttackSkillList = new List<AttackSkillClass>();
    //private List<RecoverSkillClass> RecoverSkillList = new List<RecoverSkillClass>();
    //private List<SkillDataClass> SkillList = new List<SkillDataClass>();

    private List<SkillDataRec> SkillRecList = new List<SkillDataRec>();


    public SkillDatabaseManager()
    {
        //LoadAttackSkillDatabase();
        //LoadSkillDatabasse();
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
        };
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
