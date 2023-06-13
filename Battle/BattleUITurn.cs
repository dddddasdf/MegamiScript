using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class BattleUIManager : MonoBehaviour
{
    [SerializeField] private Image[] PlayerWholeTurnMark;    //플레이어 온전한 턴 마크
    [SerializeField] private Image[] EnemyWholeTurnMark;     //적 온전한 턴 마크
    [SerializeField] private Image[] HalfTurnMark;    //플레이어 절반 턴 마크
    [SerializeField] private TextMeshProUGUI TurnText;      //누구의 턴인지 표기하는 텍스트


    
    private void InitTurnUI()
    {
        //배틀씬이 켜질 때 턴 마크가 켜져있는 걸 막기 위하여 한 번 더 렌더러를 꺼주는 작업
        for (int i = 0; i < PlayerWholeTurnMark.Length; i++)
            PlayerWholeTurnMark[i].enabled = false;
        for (int i = 0; i < EnemyWholeTurnMark.Length; i++)
            EnemyWholeTurnMark[i].enabled = false;
    }

    /// <summary>
    /// 페이즈 전환시 참전 중 인원에 맞춰서 온전한 턴 배치
    /// </summary>
    /// <param name="NumberOfActiveMember">참전 중인 인원</param>
    /// <param name="IsPlayerTurn">플레이어의 턴인가?</param>
    public void PlaceTurn(int NumberOfActiveMember, bool IsPlayerTurn)
    {
        if (IsPlayerTurn)
        {
            for (int i = 0; i < NumberOfActiveMember; i++)
                PlayerWholeTurnMark[i].enabled = true;   //현재 활동 가능한 파티 멤버수 만큼 마크 렌더러 켜줌

            TurnText.text = "<i><color=#3AEC37>Player</color> <color=#FFFFFF>Turn</color></i>";
        }
        else
        {

        }
    }

    /// <summary>
    /// 모든 턴 삭제
    /// </summary>
    /// <param name="IsPlayerTurn">플레이어의 턴인가?</param>
    public void HideAllMark(bool IsPlayerTurn)
    {
        if (IsPlayerTurn)
        {
            foreach (Image TurnMark in PlayerWholeTurnMark)
            {
                TurnMark.enabled = false;
            }
        }
        else
        {
            foreach (Image TurnMark in EnemyWholeTurnMark)
            {
                TurnMark.enabled = false;
            }
        }
        HideAllHalfTurn();
    }

    /// <summary>
    /// 소모한 온전한 턴수만큼 마크 감추기
    /// </summary>
    /// <param name="NumberOfRemainTurn">남아있는 온전한 턴수</param>
    /// <param name="NumberOfUsedTurn">소모한 온전한 턴수</param>
    /// <param name="IsPlayerTurn">플레이어의 턴인가?</param>
    public void HideWholeTurnMark(int NumberOfRemainTurn, int NumberOfUsedTurn, bool IsPlayerTurn)
    {
        if (IsPlayerTurn)
        {
            for (int i = 1; i <= NumberOfUsedTurn; i++)
                PlayerWholeTurnMark[NumberOfRemainTurn - i].enabled = false;
        }
        else
        {
            for (int i = 1; i <= NumberOfUsedTurn; i++)
                EnemyWholeTurnMark[NumberOfRemainTurn - i].enabled = false;
        }
    }

    /// <summary>
    /// 절반 턴 생성->절반 턴 마크 표시하기
    /// </summary>
    /// <param name="NumberOfRemainTurn">남아있는 온전한 턴수</param>
    public void PlaceHalfTurnMark(int NumberOfRemainTurn, int NumberOfRemainHalfTurn, int NumberOfUsedTurn, int NumberOfOnBattle)
    {
        if (NumberOfRemainHalfTurn == 0)
        {
            HalfTurnMark[NumberOfRemainTurn - 1].enabled = true;
            //남아있는 온전한 턴수의 인덱스를 절반 턴수의 인덱스로 치환한다고 생각하면... 편하다...
        }
        else
        {
            for (int i = (NumberOfOnBattle - NumberOfUsedTurn - 1); i >= 0; i--)
            {
                if (HalfTurnMark[i].enabled == false)
                {
                    HalfTurnMark[i].enabled = true;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 절반 턴 소모->절반 턴 감추기
    /// </summary>
    /// <param name="NumberOfRemainTurn">남아있는 온전한 턴수</param>
    /// <param name="NumberOfHalfTurn">남아있는 절반 턴수</param>
    /// <param name="NumberOfUsedHalfTurn">소모한 절반 턴수</param>
    public void HideHalfTurnMark(int NumberOfRemainTurn, int NumberOfHalfTurn, int NumberOfUsedHalfTurn, bool IsPlayerTurn)
    {
        if (NumberOfUsedHalfTurn == 1)
        {
            HalfTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;

            if (IsPlayerTurn)
            {
                PlayerWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
            }
            else
            {
                EnemyWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
            }
        }
        else if (NumberOfUsedHalfTurn == 2)
        {
            HalfTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
            HalfTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 2].enabled = false;

            if (IsPlayerTurn)
            {
                PlayerWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
                PlayerWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 2].enabled = false;
            }
            else
            {
                EnemyWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 1].enabled = false;
                EnemyWholeTurnMark[NumberOfRemainTurn + NumberOfHalfTurn - 2].enabled = false;
            }
        }

        //남아있는 온전한 턴수에 현재 절반 턴수를 더하고 1 빼면 감춰야 할 이미지 인덱스
        //절반 턴 표시가 사라지는 건 곧 온전한 턴 표시 또한 같이 사라지는 것이므로 동시에 처리해준다
        //뭐 굳이 반복문 돌려야 할 필요가 있을까 싶어서... 절반 턴수는 한 번에 최대 2까지만 소모하므로
    }

    /// <summary>
    /// 플레이어의 턴임을 표시
    /// </summary>
    /// <param name="ActivePartyMember">현재 활동 가능한 파티 멤버의 수</param>
    public void PlacePlayerTurn(int ActivePartyMember)
    {
        for (int i = 0; i < ActivePartyMember; i++)
            PlayerWholeTurnMark[i].enabled = true;   //현재 활동 가능한 파티 멤버수 만큼 마크 렌더러 켜줌

        TurnText.text = "<i><color=#3AEC37>Player</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// 플레이어 턴을 소모했으면 마크를 꺼줌
    /// </summary>
    /// <param name="MemberNumber"></param>
    public void HidePlayerMark(int TurnNumber)
    {
        PlayerWholeTurnMark[TurnNumber - 1].enabled = false;
    }

    /// <summary>
    /// 모든 턴 삭제시 남아있는 모든 턴의 렌더러를 끈다-플레이어
    /// </summary>
    public void HideAllPlayerMark()
    {
        foreach (Image TurnMark in PlayerWholeTurnMark)
        {
            TurnMark.enabled = false;
        }
        HideAllHalfTurn();
    }

    /// <summary>
    /// 적의 턴임을 표시
    /// </summary>
    /// <param name="ActiveEnemyMember">현재 활동 가능한 적의 수</param>
    public void PlaceEnemyTurn(int ActiveEnemyMember)
    {
        for (int i = 0; i < ActiveEnemyMember; i++)
            EnemyWholeTurnMark[i].enabled = true;   //현재 활동 가능한 적의 수 만큼 마크 렌더러 켜줌

        TurnText.text = "<i><color=#FB2F2F>Enemy</color> <color=#FFFFFF>Turn</color></i>";
    }

    /// <summary>
    /// 적의 턴을 소모했으면 마크를 꺼줌
    /// </summary>
    /// <param name="TurnNumber"></param>
    public void HideEnemyMark(int TurnNumber)
    {
        EnemyWholeTurnMark[TurnNumber - 1].enabled = false;
    }

    /// <summary>
    /// 모든 턴 삭제시 남아있는 모든 턴의 렌더러를 끈다-적
    /// </summary>
    public void HideAllEnemyMark()
    {
        foreach (Image TurnMark in EnemyWholeTurnMark)
        {
            TurnMark.enabled = false;
        }
        HideAllHalfTurn();
    }

    /// <summary>
    /// 절반턴 표시
    /// </summary>
    public void PlaceHalfTurn(int RemainWholeTurn, int HalfTurn)
    {

    }

    /// <summary>
    /// 모든 절반 턴 렌더러 끄기
    /// </summary>
    public void HideAllHalfTurn()
    {
        foreach (Image TurnMark in HalfTurnMark)
        {
            TurnMark.enabled = false;
        }
    }
}
