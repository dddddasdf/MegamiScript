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