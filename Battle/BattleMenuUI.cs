using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleMenuUI : MonoBehaviour
{
    #region SetVariables
    //전체 메뉴 관련 변수
    [SerializeField] private Canvas SelectActMenuCanvas;    //전체 행동 메뉴 캔버스
    [SerializeField] private List<Button> ActButton;
    [SerializeField] private Button SkillMenuButton;
    [SerializeField] private Button ItemMenuButton;
    [SerializeField] private Button TalkMenuButton;
    [SerializeField] private Button ChangeMenuButton;
    [SerializeField] private Button EscapeMenuButton;
    [SerializeField] private Button PassMenuButton;
    private int NowSelectedActIndex;    //현재 선택 중인 행동 버튼 인덱스: 키보드 입력용 및 저장용


    [SerializeField] private Canvas SelectSkillMenuCanvas;  //스킬 메뉴 캔버스
    [SerializeField] private List<Button> SkillButton;
    private int NowSelectedSkillIndex;  //현재 선택 중인 스킬 버튼 인덱스: 키보드 입력용 및 저장용


    private Button NowSelectedButton;   //활성화 상태 버튼 저장용

    private Color SelectedActButtonColor;    //선택된 행동 버튼 색상
    private Color UnselectedActButtonColor;  //선택되지 않은 행동 버튼 색상

    private Color SelectedSkillButtonColor; //선택된 스킬 버튼 색상+투명도
    private Color UnselectedSkillButtonColor;   //선택되지 않은 스킬 버튼 색상+투명도

    private int ShowNumberofSkill;  //캐릭터가 습득하고 있는 스킬수-해당 수에 맞춰서 슬롯 출력

    
    //키입력 관련 변수
    private bool IsCanInput;    //유저의 키입력을 현재 받을지 말지 결정-연출 동안 유저의 키 입력을 아예 받을지 말지 일단 확인용... 쓸데없는 오작동은 막는 게 좋다
    private Coroutine GetInputCoroutine;


    #endregion

    private void Awake()
    {
        //초기화
        SelectedActButtonColor = new Color(107/255f, 222/255f, 21/255f);
        UnselectedActButtonColor = new Color(0/255f, 0/255f, 0/255f);

        SelectedSkillButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255/255f);
        UnselectedSkillButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100/255f);

        NowSelectedActIndex = 1;

        NowSelectedSkillIndex = 1;

        IsCanInput = true;
        GetInputCoroutine =  StartCoroutine(GetPlayerKeyboardInput());
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
        NowSelectedButton.image.color = SelectedActButtonColor;
    }


    public void SwapMenuCanvas(int ToSwapMenuIndex)
    {

    }

    #region SelectActMenuButton

    /// <summary>
    /// 버튼 클릭시 활성화 된 버튼을 한 번 더 누르는 것인지, 다른 버튼을 눌렀는지 판별하는 함수
    /// </summary>
    public void ClickActButton()
    {
        Button Tmp = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();  //클릭된 버튼 임시로 가져오기

        if (NowSelectedButton == Tmp)
        {
            //현재 활성화된 버튼과 유저가 클릭한 버튼이 일치시 취할 행동
            CompareWhichActButtonClicked(Tmp);
        }
        else
        {
            //일치하지 않으면 활성화된 버튼을 스왑함
            NowSelectedButton.image.color = UnselectedActButtonColor;
            NowSelectedButton = Tmp;
            Tmp.image.color = SelectedActButtonColor;
            Debug.Log("활성화 된 버튼 교체");
        }
    }

    /// <summary>
    /// 어떤 버튼이 클릭 되었는지 대조
    /// </summary>
    /// <param name="Tmp">지금 눌린 버튼</param>
    private void CompareWhichActButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
        Debug.Log(TmpButtonName);
        
        switch (TmpButtonName)
        {
            case "SkillMenuButton":
                SelectActMenuCanvas.enabled = false;
                SetDefaultSelectedSkillButton();
                SelectSkillMenuCanvas.enabled = true;
                break;
            case "ItemMenuButton":
                break;
            case "TalkMenuButton":
                break;
            case "ChangeMenuButton":
                break;
            case "EscapeMenuButton":
                break;
            case "PassMenuButton":
                break;
            default:
                Debug.Log("오류: 버튼명 인식되지 않음");
                break;
        }
    }

    #endregion

    #region SelectSkillMenuButton

    /// <summary>
    /// 스킬창 들어가면 기본적으로 활성화되는 스킬 버튼은 첫번째 스킬
    /// </summary>
    private void SetDefaultSelectedSkillButton()
    {
        NowSelectedButton = SkillButton[0]; //첫번째 스킬이 활성화된 버튼으로
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
            Debug.Log("활성화 된 버튼 교체");
            Debug.Log(NowSelectedSkillIndex);
        }
    }

    private void CompareWhichSkillButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
        Debug.Log(TmpButtonName);

        //switch (TmpButtonName)
        //{
        //    case "SkillMenuButton":
        //        SelectActMenuCanvas.enabled = false;
        //        SetDefaultSelectedSkillButton();
        //        SelectSkillMenuCanvas.enabled = true;
        //        break;
        //    case "ItemMenuButton":
        //        break;
        //    case "TalkMenuButton":
        //        break;
        //    case "ChangeMenuButton":
        //        break;
        //    case "EscapeMenuButton":
        //        break;
        //    case "PassMenuButton":
        //        break;
        //    default:
        //        Debug.Log("오류: 버튼명 인식되지 않음");
        //        break;
        //}
    }



    #endregion


    #region GetInput
    /// <summary>
    /// 유저 키보드 조작 키입력 받는 곳
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetPlayerKeyboardInput()
    {
        

        while (true)
        {
            //위쪽 버튼
            if (Input.GetKeyDown(KeyCode.W))
            {
                //키입력 받음
                if (NowSelectedSkillIndex != 1)
                {
                    NowSelectedButton.image.color = UnselectedSkillButtonColor;
                    NowSelectedSkillIndex--;
                    NowSelectedButton = SkillButton[NowSelectedSkillIndex - 1];
                    NowSelectedButton.image.color = SelectedSkillButtonColor;
                    Debug.Log(NowSelectedSkillIndex);
                }

            }

            //아래쪽 버튼
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (NowSelectedSkillIndex != 8)
                {
                    NowSelectedButton.image.color = UnselectedSkillButtonColor;
                    NowSelectedSkillIndex++;
                    NowSelectedButton = SkillButton[NowSelectedSkillIndex - 1];
                    NowSelectedButton.image.color = SelectedSkillButtonColor;
                    Debug.Log(NowSelectedSkillIndex);
                }
            }

            yield return null;
        }
        

        

        //결정 버튼
        //if (Input.GetKey(KeyCode.F))
            ;
    }

    #endregion
}
