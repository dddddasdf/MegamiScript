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
/// 필드 몬스터 심볼 유형
/// </summary>
public enum EnemySymbolType
{
    Humanoid,   //인간형
    Bird,   //새
    Wild    //야수
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

/// <summary>
/// 전투 치를 때 스킬, 아이템, 대화 시도 등의 버튼에 대한 설명 구조체
/// </summary>
[System.Serializable]
public struct HelpText
{
    public string Code;
    public string Text;
}

/// <summary>
/// 전투 치를 때 스킬에 대한 설명
/// </summary>
[System.Serializable] 
public struct SkillInfo
{
    public string Type; //공격 유형
    public string Name; //스킬명
    public int Power;   //위력

}