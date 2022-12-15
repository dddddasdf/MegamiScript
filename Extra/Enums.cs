using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneIndex
{
    BattleScene = 1,
    ManagerScene = 2
}

/// <summary>
/// 몬스터 인덱스
/// </summary>
public enum MonsterIndex
{
    ExtraMobCount = 10, //잡몹 총 개수
    //아래로는 보스몬스터들 인덱스
}

/// <summary>
/// 몬스터 구조체 - 임시용으로 만든 거임,,,
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
    //여기 스킬 리스트 들어가야 됨
}

/// <summary>
/// 스탯 구조체
/// </summary>
public struct Status
{
    public int Power;
    public int Technic;
    public int Magic;
    public int Speed;
    public int Luck;
}