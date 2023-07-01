using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public partial class BattleUIManager : MonoBehaviour
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

    private Button NowSelectedButton;   //활성화 상태 버튼 저장용

    private Color SelectedActButtonColor;    //선택된 행동 버튼 색상
    private Color UnselectedActButtonColor;  //선택되지 않은 행동 버튼 색상

    private int ShowNumberofSkill;  //캐릭터가 습득하고 있는 스킬수-해당 수에 맞춰서 슬롯 출력

    /// <summary>
    /// 버튼 더블클릭 감지용-배틀매니저와 연계가 필요할 때만 사용/배틀매니저가 사용하고 나면 반드시 리셋 함수 호출
    /// </summary>
    private bool IsButtonDoubleclicked = false;
    private NowOnMenu NowSelectedMenu;

    private Stack<NowOnMenu> OnMenuStack = new();
    private NowOnMenu PopMenu;
    private bool IsGetKeyOK = true;         //화면 전환 애니메이션 중에는 X키 입력 감지 불가능 용으로: 개발 단계 에서는 상시 true로 해두고 애니메이션이 도입되면 온오프 할 수 있게 한다

    private Queue<Action> JobQueue = new Queue<Action>();      //순차적으로 진행시킬 작업 목록
    public void AddJobQueueMethod(Action Method)
    {
        JobQueue.Enqueue(Method);
    }

    /// <summary>
    /// 작업큐에 들어와 있던 예약 작업들 처리하기
    /// </summary>
    private void DoJobQueue()
    {
        while (JobQueue.Count > 0)
        {
            Action action = JobQueue.Dequeue();
            action?.Invoke();
        }
        JobQueue.Clear();       //작업큐 비워주기
    }

    #endregion

    private void Awake()
    {
        //초기화
        SelectedActButtonColor = new Color(107/255f, 222/255f, 21/255f);
        UnselectedActButtonColor = new Color(0/255f, 0/255f, 0/255f);

        NowSelectedActIndex = 1;

        //IsCanInput = true;

        InitSkillUI();

        InitTurnUI();

        InsertItemData();
    }

    private void Start()
    {
        SetSkillScroll();
        SetItemScroll();
    }

    private void OnEnable()
    {
        SetDefaultSelectedActMenuButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && IsGetKeyOK)
        {
            //뒤로 가기 버튼
            GoBackPreviousMenu();
        }
    }

    /// <summary>
    /// 뒤로가기 버튼을 누를 경우 이전 단계 메뉴로 돌아간다
    /// </summary>
    private void GoBackPreviousMenu()
    {
        if (OnMenuStack.Count == 0)
        {
            //최상위 메뉴인 행동 단계 메뉴 상태인지 체크(스택이 비어 있다면 행동 단계 메뉴)
            return;
        }

        PopMenu = OnMenuStack.Pop();    //펼쳐진 메뉴 스택에서 현재 펼쳐진 메뉴를 pop

        //pop된 메뉴에 따라 분기
        switch (PopMenu)
        {
            case NowOnMenu.SkillMenu:
                HideSkillMenu();
                HideAffinityMarkAll();      //상성 표시 마크 감추기
                SelectActMenuCanvas.enabled = true;
                break;
            case NowOnMenu.SkillSelected:
                HideEnemyTargetSelectUI();
                ShowAffinityMarkAll(NowSelectedSkillData.ReturnSkillDataRec());      //다시 모든 적에게 상성 표시
                ShowSkillMenu();
                break;
            case NowOnMenu.ItemMenu:
                HideItemMenu();
                SelectActMenuCanvas.enabled = true;
                break;
        }
    }

    /// <summary>
    /// 메뉴를 처음 열면 가장 기본적으로 선택되는 버튼은 스킬/그 외에 선택 버튼 초기화시 스킬로 고정
    /// </summary>
    private void SetDefaultSelectedActMenuButton()
    {
        NowSelectedButton = SkillMenuButton;
        NowSelectedButton.image.color = SelectedActButtonColor;
    }

    
    public bool ReturnIsButtonDoubleClicked()
    {
        return IsButtonDoubleclicked;
    }

    public void ResetIsButtonChanged()
    {
        IsButtonDoubleclicked = false;
    }

    public NowOnMenu ReturnNowChangedMenu()
    {
        return NowSelectedMenu;
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
    /// 어떤 버튼이 더블 클릭 되었는지 대조
    /// </summary>
    /// <param name="Tmp">지금 눌린 버튼</param>
    private void CompareWhichActButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
        Debug.Log(TmpButtonName);


        switch (TmpButtonName)
        {
            case "SkillMenuButton":
                ShowSkillMenu();        //스킬 선택 메뉴 진입
                ShowAffinityMarkAll(NowSelectedSkillData.ReturnSkillDataRec());
                OnMenuStack.Push(NowOnMenu.SkillMenu);      //열린 메뉴 스택에 스킬 메뉴 push
                SelectActMenuCanvas.enabled = false;        //행동 메뉴 감추기
                break;
            case "ItemMenuButton":
                ShowItemMenu();
                OnMenuStack.Push(NowOnMenu.ItemMenu);      //열린 메뉴 스택에 아이템 메뉴 push
                SelectActMenuCanvas.enabled = false;        //행동 메뉴 감추기
                break;
            case "TalkMenuButton":
                SelectActMenuCanvas.enabled = false;        //행동 메뉴 감추기
                break;
            case "ChangeMenuButton":
                SelectActMenuCanvas.enabled = false;        //행동 메뉴 감추기
                break;
            case "EscapeMenuButton":
                break;
            case "PassMenuButton":
                IsButtonDoubleclicked = true;
                SetDefaultSelectedActMenuButton();
                PassMenuButton.image.color = UnselectedActButtonColor;
                break;
            default:
                Debug.Log("오류: 버튼명 인식되지 않음");
                break;
        }
    }

    #endregion

    /// <summary>
    /// 행동 단계 메뉴 보이기
    /// </summary>
    public void ShowActMenu()
    {
        SelectActMenuCanvas.enabled = true;
    }
    
    /// <summary>
    /// 행동 단계 메뉴 감추기
    /// </summary>
    public void HideActMenu()
    {
        SelectActMenuCanvas.enabled = false;
    }
    
    
    /// <summary>
    /// 스킬창에서 뒤로가기 버튼
    /// </summary>
    public void BackFromSkillToActMenu()
    {
        HideSkillMenu();
        SelectActMenuCanvas.enabled = true;
    }

    public void BackFromItemToActMenu()
    {
        HideItemMenu();
        SelectActMenuCanvas.enabled = true;
    }


    
}
