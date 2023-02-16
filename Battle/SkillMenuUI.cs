using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillMenuUI : MonoBehaviour
{
    #region SetVariables
 
    [SerializeField] private Canvas SelectSkillMenuCanvas;
    [SerializeField] private List<Button> SkillButton;
    private int NowSelectedSkillIndex;  //현재 선택 중인 스킬 버튼 인덱스: 키보드 입력용 및 저장용

    //선택된 버튼 관련 변수
    private Button NowSelectedButton;   //선택된 버튼 저장용
    private Color SelectedSkillButtonColor; //선택된 스킬 버튼 색상+투명도
    private Color UnselectedSkillButtonColor;   //선택되지 않은 스킬 버튼 색상+투명도

    private int ShowNumberofSkill;  //캐릭터가 습득하고 있는 스킬수-해당 수에 맞춰서 슬롯 출력


    #endregion

    private void Awake()
    {
        SelectedSkillButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
        UnselectedSkillButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100 / 255f);

        NowSelectedSkillIndex = 0;
        SetDefaultSelectedSkillButton();
    }

    /// <summary>
    /// 스킬 메뉴창 보이기
    /// </summary>
    public void ShowSkillMenu()
    {
        SelectSkillMenuCanvas.enabled = true;
    }

    /// <summary>
    /// 스킬 메뉴창 숨기기
    /// </summary>
    public void HideSkillMenu()
    {
        SelectSkillMenuCanvas.enabled = false;
    }



    /// <summary>
    /// 스킬창 들어가면 기본적으로 활성화되는 스킬 버튼은 첫번째 스킬
    /// </summary>
    private void SetDefaultSelectedSkillButton()
    {
        NowSelectedButton = SkillButton[NowSelectedSkillIndex];
        NowSelectedButton.image.color = SelectedSkillButtonColor;
    }

    public void ClickSkillButton()
    {
        Button Tmp = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();  //클릭된 버튼 임시로 가져오기

        if (NowSelectedButton == Tmp)
        {
            //현재 활성화된 버튼과 유저가 클릭한 버튼이 일치시 취할 행동
            CompareWhichSkillButtonClicked(Tmp);
        }
        else
        {
            //일치하지 않으면 활성화된 버튼을 스왑함
            NowSelectedButton.image.color = UnselectedSkillButtonColor;
            NowSelectedButton = Tmp;
            Tmp.image.color = SelectedSkillButtonColor;
            NowSelectedSkillIndex = int.Parse(Regex.Replace(Tmp.name, @"[^0-9]", ""));
            Debug.Log(NowSelectedSkillIndex);
        }
    }

    private void CompareWhichSkillButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
        Debug.Log(TmpButtonName);
    }
}
