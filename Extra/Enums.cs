using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneIndex
{
    BattleScene = 1,
    ManagerScene = 2
}

/// <summary>
/// ���� �ε���
/// </summary>
public enum MonsterIndex
{
    ExtraMobCount = 10, //��� �� ����
    //�Ʒ��δ� �������͵� �ε���
}

/// <summary>
/// ���� ����ü - �ӽÿ����� ���� ����,,,
/// </summary>
[System.Serializable]
public struct MonsterData
{
    public int Index;
    public string Name;
    public int HP;
    public int Power;
}

[System.Serializable] 
public struct EnemyDataStruct
{
    public Status EnemyStatus;
    //���� ��ų ����Ʈ ���� ��
}


/// <summary>
/// �ʵ� ���� �ɺ� ����
/// </summary>
public enum EnemySymbolType
{
    Humanoid,   //�ΰ���
    Bird,   //��
    Wild    //�߼�
}

/// <summary>
/// ���� ����ü
/// </summary>
public struct Status
{
    public int Power;
    public int Technic;
    public int Magic;
    public int Speed;
    public int Luck;
}





//��ų ���� �ܶ�
#region Skill

/// <summary>
/// ���� ��ų ���� ���
/// </summary>
public enum AttackType
{
    Physical,   //����
    Gun,    //��
    Fire,   //ȭ��
    Ice,    //����
    Electric,   //����
    Force,  //���
    Light,  //�ĸ�
    Dark,   //�ֻ�
    Almighty    //����
}

/// <summary>
/// ���� ġ�� �� ��ų�� ���� ����
/// </summary>
[System.Serializable] 
public struct AttackSkill
{
    public AttackType TypeName; //���� ��ų ����
    public string Name; //��ų��
    public int Power;   //����
    public int Accuracy;    //���߷�
    public int UseMP;   //�Ҹ� ������

    public bool IsAdditionalEffect;    //�ΰ�ȿ�� ����

}

public struct AttackAddtional
{


}

#endregion

#region Help
/// <summary>
/// ���� ġ�� �� ��ų, ������, ��ȭ �õ� ���� ��ư�� ���� ���� ����ü
/// </summary>
[System.Serializable]
public struct HelpText
{
    public string Code;
    public string Text;
}

#endregion

#region BattleMenu

/// <summary>
/// ���� ��ư���� �ִ� �޴� ���
/// </summary>
[System.Serializable]
enum BattleMenuName
{
    SkillMenuButton = 0,
    ItemMenuButton = 1,
    TalkMenuButton = 2,
    ChangeMenuButton = 3,
    EscapeMenuButton = 4,
    PassMenuButton = 5
}

#endregion