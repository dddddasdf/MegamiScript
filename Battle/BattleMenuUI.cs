using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas SelectActMenuCanvas;
    
    [SerializeField] private Button SkillMenuButton;
    [SerializeField] private Button ItemMenuButton;
    [SerializeField] private Button TalkMenuButton;
    [SerializeField] private Button ChangeMenuButton;
    [SerializeField] private Button EscapeMenuButton;
    [SerializeField] private Button PassMenuButton;

    [SerializeField] private Canvas SelectSkillMenuCanvas;



    private Button NowSelectedButton;   //활성화 상태 버튼 저장용

    private Color SelectedColor;    //선택된 버튼 색상
    private Color UnselectedColor;  //선택되지 않은 버튼 색상

    private void Awake()
    {
        SelectedColor = new Color(107/255f, 222/255f, 21/255f);
        UnselectedColor = new Color(0/255f, 0/255f, 0/255f);
    }

    private void OnEnable()
    {
        SetDefaultSelectedActMenuButton();
    }

    /// <summary>
    /// 메뉴를 처음 열면 가장 기본적으로 선택되는 버튼은 스킬
    /// </summary>
    private void SetDefaultSelectedActMenuButton()
    {
        NowSelectedButton = SkillMenuButton;
        NowSelectedButton.image.color = SelectedColor;
    }

    /// <summary>
    /// 스킬창에 들어가면 가장 기본적으로 선택되는 버튼은 최상위에 배치된 스킬
    /// </summary>
    private void SetDefaultSelectedSkilButton()
    {

    }

    public void SwapMenuCanvas(int ToSwapMenuIndex)
    {

    }

    /// <summary>
    /// 버튼 클릭시 활성화 된 버튼을 한 번 더 누르는 것인지, 다른 버튼을 눌렀는지 판별하는 함수
    /// </summary>
    public void ClickButton()
    {
        Button Tmp = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();  //클릭된 버튼 임시로 가져오기

        if (NowSelectedButton == Tmp)
        {
            //현재 활성화된 버튼과 유저가 클릭한 버튼이 일치시 취할 행동
            Debug.Log("버튼 일치");
        }
        else
        {
            //일치하지 않으면 활성화된 버튼을 스왑함
            NowSelectedButton.image.color = UnselectedColor;
            NowSelectedButton = Tmp;
            Tmp.image.color = SelectedColor;
            Debug.Log("활성화 된 버튼 교체");
        }
    }
}
