using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

//���� ������ ��ų ȿ������ ���ϵ� json�� �Ľ��� ������ �����ϴ� Ŭ����

/// <summary>
/// ��ų ���� ��з�
/// </summary>
public enum SkillSort
{
    Attack,     //����
    Recover,    //ȸ��
    Support,    //��Ÿ ����&�������
    Passive     //�нú��
}


/// <summary>
/// ���� ��ų ����
/// </summary>
public enum AttackSkillType
{
    Physical,       //����
    Gun,            //��
    Fire,           //�Ʊ�
    Ice,            //�ν�
    Electric,       //����
    Force,          //��
    Light,          //�ϸ�
    Dark,           //����
    Almighty        //�ޱ⵵(����)
}

/// <summary>
/// ��ų�� �����ϴ� ��ü�� ��
/// </summary>
public enum TargetNumber
{
    Single,     //����
    Multi,      //�ټ�
    All         //��ü
}

/// <summary>
/// ���� ��ų�� �ΰ� ȿ��
/// </summary>
public enum AddtionalEffect
{
    None,           //ȿ�� ����
    Critical,       //ũ��Ƽ�� Ȯ�� ����
    DrainHP,        //HP ���
    DrainMP,        //MP ���
    DrainHPMP,      //HP, MP ���
    
}


/// <summary>
/// ����, ȸ��, ������ ���������� ������ �κ�
/// </summary>
public class Skill
{
    public string Name;         //��ų �̸�
    public int ID;              //�ĺ� ID
    public int UseMP;           //�Ҹ� ����
    public TargetNumber thisTargetNumber;   //��ų ��� ��ü ��
    public int MinHits;         //�ּ� Ƚ��
    public int MaxHits;         //�ִ� Ƚ��
    public int Rank;            //��ų ��ũ
    public int Accuracy;        //���߷�
    public string Description;  //��ų ����
}

/// <summary>
/// ���� ��ų Ŭ����
/// </summary>
[SerializeField]
public class AttackSkillClass : Skill
{
    public AttackSkillType thisSkillType;
    public int Power;     //����
    public AddtionalEffect AddEffect;       //�ΰ� ȿ��
    public int? Value;                      //�ΰ� ȿ���� ������ ��� ���� ��ġ-�ΰ� ȿ���� ���� ��� null
}

/// <summary>
/// ȸ�� ��ų Ŭ����
/// </summary>
[SerializeField]
public class RecoverSkillClass : Skill
{
    public int Power;     //����
}

/// <summary>
/// ���� ��ų Ŭ����
/// </summary>
[SerializeField]
public class SupportSkillClass : Skill
{
    public string Target;       //��ų ����� �Ʊ����� ������
}

public class SkillDataBase
{
    //private static List<SkillDataBase> SkillDataBaseList;
    private List<AttackSkillClass> AttackSkillList = new List<AttackSkillClass>();


    public SkillDataBase()
    {
        LoadAttackSkillDatabase();
    }

    /// <summary>
    /// ��ų DB �ҷ����̱�
    /// </summary>
    private void LoadAttackSkillDatabase()
    {
        AsyncOperationHandle<TextAsset> TextAssetHandle;
        TextAssetHandle = Addressables.LoadAssetAsync<TextAsset>("AttackSkillData");

        TextAssetHandle.Completed += Handle =>
        {
            if (TextAssetHandle.Status == AsyncOperationStatus.Failed)
            {
                
            }

            AttackSkillList = JsonConvert.DeserializeObject<List<AttackSkillClass>>(TextAssetHandle.Result.text);
        };
    }

    public void TestScript()
    {
        Debug.Log(AttackSkillList[0].Name);
    }
}
