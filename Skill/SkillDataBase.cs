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
    public string Description;  //��ų ����
}

public class AttackSkillClass : Skill
{
    public AttackSkillType thisSkillType;
    public int Power;     //����
}

public class RecoverSkillClass : Skill
{
    public int Power;     //����
}

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
        
    }
}
