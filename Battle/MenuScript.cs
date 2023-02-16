using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private Canvas MenuCanvas;
    [SerializeField] private List<Button> ButtonList;
    private int NowSelectedButtonIndex;  //현재 선택 중인 스킬 버튼 인덱스: 키보드 입력용 및 저장용

    //선택된 버튼 관련 변수
    private Button NowSelectedButton;   //선택된 버튼 저장용
    private Color SelectedButtonColor; //선택된 스킬 버튼 색상+투명도
    private Color UnselectedButtonColor;   //선택되지 않은 스킬 버튼 색상+투명도

    #endregion

    public void ShowMenuCanvas()
    {
        MenuCanvas.enabled = true;
    }

    public void HideMenuCanvas()
    {
        MenuCanvas.enabled = false;
    }
    
}
