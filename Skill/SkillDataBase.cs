using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

//게임 구동시 스킬 효과들이 수록된 json을 파싱해 내용을 저장하는 클래스

/// <summary>
/// 스킬 종류 대분류
/// </summary>
public enum SkillSort
{
    Attack,     //공격
    Recover,    //회복
    Support,    //기타 버프&디버프기
    Passive     //패시브기
}


/// <summary>
/// 공격 스킬 유형
/// </summary>
public enum AttackSkillType
{
    Physical,       //물리
    Gun,            //총
    Fire,           //아기
    Ice,            //부스
    Electric,       //지오
    Force,          //잔
    Light,          //하마
    Dark,           //무드
    Almighty        //메기도(만능)
}

/// <summary>
/// 스킬이 적중하는 개체의 수
/// </summary>
public enum TargetNumber
{
    Single,     //단일
    Multi,      //다수
    All         //전체
}


/// <summary>
/// 공격, 회복, 버프가 공통적으로 가지는 부분
/// </summary>
public class Skill
{
    public string Name;         //스킬 이름
    public int ID;              //식별 ID
    public int UseMP;           //소모 마나
    public TargetNumber thisTargetNumber;   //스킬 대상 개체 수
    public int MinHits;         //최소 횟수
    public int MaxHits;         //최다 횟수
    public int Rank;            //스킬 랭크
    public string Description;  //스킬 설명
}

public class AttackSkillClass : Skill
{
    public AttackSkillType thisSkillType;
    public int Power;     //위력
}

public class RecoverSkillClass : Skill
{
    public int Power;     //위력
}

public class SupportSkillClass : Skill
{
    public string Target;       //스킬 대상이 아군인지 적인지
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
    /// 스킬 DB 불러들이기
    /// </summary>
    private void LoadAttackSkillDatabase()
    {
        AsyncOperationHandle<TextAsset> TextAssetHandle;
        
    }
}
