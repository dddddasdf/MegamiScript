using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private Image[] PlayerTurnMark;    //플레이어 턴 마크
    [SerializeField] private Image[] EnemyTurnMark;     //적 턴 마크
    [SerializeField] private TextMeshProUGUI TurnText;      //누구의 턴인지 표기하는 텍스트


    private void Awake()
    {
        //배틀씬이 켜질 때 턴 마크가 켜져있는 걸 막기 위하여 한 번 더 렌더러를 꺼주는 작업
        for (int i = 0; i < PlayerTurnMark.Length; i++)
            PlayerTurnMark[i].enabled = false;
        for (int i = 0; i < EnemyTurnMark.Length; i++)
            EnemyTurnMark[i].enabled = false;
    }

    /// <summary>
    /// 플레이어의 턴임을 표시
    /// </summary>
    /// <param name="ActivePartyMember">현재 활동 가능한 파티 멤버의 수</param>
    public void PlacePlayerTurn(int ActivePartyMember)
    {
        for (int i = 0; i < ActivePartyMember; i++)
            PlayerTurnMark[i].enabled = true;   //현재 활동 가능한 파티 멤버수 만큼 마크 렌더러 켜줌

        TurnText.text = "<i><color=#3AEC37>Player</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// 플레이어 턴을 소모했으면 마크를 꺼줌
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void HidePlayerMark(int TurnNumber)
    {
        PlayerTurnMark[TurnNumber - 1].enabled = false;
    }

    /// <summary>
    /// 적의 턴임을 표시
    /// </summary>
    /// <param name="ActiveEnemyMember">현재 활동 가능한 적의 수</param>
    public void PlaceEnemyTurn(int ActiveEnemyMember)
    {
        for (int i = 0; i < ActiveEnemyMember; i++)
            EnemyTurnMark[i].enabled = true;   //현재 활동 가능한 적의 수 만큼 마크 렌더러 켜줌

        TurnText.text = "<i><color=#FB2F2F>Enemy</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// 적의 턴을 소모했으면 마크를 꺼줌
    /// </summary>
    /// <param name="TurnNumber"></param>
    public void HideEnemyMark(int TurnNumber)
    {
        EnemyTurnMark[TurnNumber - 1].enabled = false;
    }
}
