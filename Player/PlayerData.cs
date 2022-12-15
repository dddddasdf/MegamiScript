using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 파티 멤버 정보를 저장할 구조체
/// </summary>
public struct PartyMember
{
    public bool IsEmptyMember;  //해당 번호의 멤버가 공란인지 확인
    public string Name; //이름
    public int Level;   //현재 레벨
    public int EXP;
    public int MaxHP;   //최대 체력
    public int RemainHP;    //현재 체력
    public int MaxMP;   //최대 마나
    public int RemainMP;    //현재 마나
    public Status PlayerStatus;

    //여기 스킬 리스트 들어가야 됨
}

public class PlayerData
{
    private PartyMember PlayerStruct;   //파티에 플레이어 캐릭터는 첫번째 멤버로 고정
    private PartyMember[] PartyMemberArray; //파티 멤버 배열

    /// <summary>
    /// 임시용 유저 상태 강제로 세팅
    /// </summary>
    public void SetPlayerTmp()
    {
        PartyMemberArray = new PartyMember[4];
        
        PartyMemberArray[0].IsEmptyMember = false;
        PartyMemberArray[0].Name = "홍길동";
        PartyMemberArray[0].Level = 10;
        PartyMemberArray[0].EXP = 50;
        PartyMemberArray[0].MaxHP = 100;
        PartyMemberArray[0].RemainHP = 70;
        PartyMemberArray[0].MaxMP = 70;
        PartyMemberArray[0].RemainMP = 70;

        PartyMemberArray[1].IsEmptyMember = false;
        PartyMemberArray[1].Name = "잭 프로스트";
        PartyMemberArray[1].Level = 10;
        PartyMemberArray[1].EXP = 50;
        PartyMemberArray[1].MaxHP = 80;
        PartyMemberArray[1].RemainHP = 80;
        PartyMemberArray[1].MaxMP = 50;
        PartyMemberArray[1].RemainMP = 27;

        PartyMemberArray[2].IsEmptyMember = false;
        PartyMemberArray[2].Name = "락슈미";
        PartyMemberArray[2].Level = 10;
        PartyMemberArray[2].EXP = 50;
        PartyMemberArray[2].MaxHP = 93;
        PartyMemberArray[2].RemainHP = 58;
        PartyMemberArray[2].MaxMP = 128;
        PartyMemberArray[2].RemainMP = 99;

        PartyMemberArray[3].IsEmptyMember = true;
        PartyMemberArray[3].Name = "";
        PartyMemberArray[3].Level = 0;
        PartyMemberArray[3].EXP = 0;
        PartyMemberArray[3].MaxHP = 0;
        PartyMemberArray[3].RemainHP = 0;
        PartyMemberArray[3].MaxMP = 0;
        PartyMemberArray[3].RemainMP = 0;
    }

    public PartyMember ReturnPlayerData()
    {
        return PlayerStruct;
    }

    /// <summary>
    /// 전투를 시작할 때 파티 정보 반환
    /// </summary>
    /// <param name="MemberNumber"></param>
    /// <returns></returns>
    public PartyMember ReturnPartyData(int MemberNumber)
    {
        return PartyMemberArray[MemberNumber];
    }
}
