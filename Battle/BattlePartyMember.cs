using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlePartyMember : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject MemberInfo;  //빈 멤버가 아닐 경우 출력하는 정보값들의 집합
    [SerializeField] private Image Frame;  //파티 프레임
    [SerializeField] private Slider HPBar;  //체력 바 
    [SerializeField] private Slider MPBar;  //마나 바
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI MPText;
    [SerializeField] private GameObject EmptyText; //빈 멤버일 경우 비어있음 표기

    #endregion

    /// <summary>
    /// 비어있는 멤버인지 확인
    /// </summary>
    /// <param name="IsEmpty"> 비어있으면 true</param>
    public void SetEmptyText()
    {
        MemberInfo.SetActive(false);
        EmptyText.SetActive(true);
    }

    /// <summary>
    /// 표기될 이름 문자열 받아옴
    /// </summary>
    /// <param name="Name"></param>
    public void GetName(string Name)
    {
        NameTMP.text = Name;
    }

    /// <summary>
    /// 해당 번호 멤버의 최대 체력 및 마나 값 설정
    /// </summary>
    /// <param name="thisMaxHP"></param>
    /// <param name="thisMaxMP"></param>
    public void GetMaxHPMP(int MaxHP, int MaxMP)
    {
        HPBar.maxValue = MaxHP;
        MPBar.maxValue = MaxMP;
    }

    /// <summary>
    /// 체력 바 게이지 설정
    /// </summary>
    /// <param name="RemainHP"></param>
    public void GetHPValue(int RemainHP)
    {
        HPBar.value = RemainHP;
        HPText.text = RemainHP.ToString();
    }
    
    /// <summary>
    /// 마나 바 게이지 설정
    /// </summary>
    /// <param name="RemainMP"></param>
    public void GetMPValue(int RemainMP)
    {
        MPBar.value = RemainMP;
        MPText.text = RemainMP.ToString();
    }
    
    /// <summary>
    /// 현재 조작턴일 경우 프레임을 녹색으로 변경
    /// </summary>
    public void NowTurn()
    {
        Frame.color = Color.green;
    }

    /// <summary>
    /// 조작턴이 종료되면 프레임을 흰색으로 변경
    /// </summary>
    public void EndTurn()
    {
        Frame.color = Color.white;
    }
}
