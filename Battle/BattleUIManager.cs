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
    //��ü �޴� ���� ����
    [SerializeField] private Canvas SelectActMenuCanvas;    //��ü �ൿ �޴� ĵ����
    [SerializeField] private List<Button> ActButton;
    [SerializeField] private Button SkillMenuButton;
    [SerializeField] private Button ItemMenuButton;
    [SerializeField] private Button TalkMenuButton;
    [SerializeField] private Button ChangeMenuButton;
    [SerializeField] private Button EscapeMenuButton;
    [SerializeField] private Button PassMenuButton;
    private int NowSelectedActIndex;    //���� ���� ���� �ൿ ��ư �ε���: Ű���� �Է¿� �� �����

    private Button NowSelectedButton;   //Ȱ��ȭ ���� ��ư �����

    private Color SelectedActButtonColor;    //���õ� �ൿ ��ư ����
    private Color UnselectedActButtonColor;  //���õ��� ���� �ൿ ��ư ����

    private int ShowNumberofSkill;  //ĳ���Ͱ� �����ϰ� �ִ� ��ų��-�ش� ���� ���缭 ���� ���

    /// <summary>
    /// ��ư ����Ŭ�� ������
    /// </summary>
    public bool IsSelectedChanged = false;
    private NowOnMenu NowSelectedMenu;

    private Stack<NowOnMenu> OnMenuStack = new();
    private NowOnMenu PopMenu;

    private Queue<Action> JobQueue = new Queue<Action>();      //���������� �����ų �۾� ���
    public void AddJobQueueMethod(Action Method)
    {
        JobQueue.Enqueue(Method);
    }

    /// <summary>
    /// �۾�ť�� ���� �ִ� ���� �۾��� ó���ϱ�
    /// </summary>
    private void DoJobQueue()
    {
        while (JobQueue.Count > 0)
        {
            Action action = JobQueue.Dequeue();
            action?.Invoke();
        }
        JobQueue.Clear();       //�۾�ť ����ֱ�
    }

    #endregion

    private void Awake()
    {
        //�ʱ�ȭ
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
        if (Input.GetKey(KeyCode.X))
        {
            //�ڷ� ���� ��ư
            CheckOnMenu();
        }
    }

    /// <summary>
    /// �ڷΰ��� ��ư�� ���� ��� ���� �ܰ� �޴��� ���ư���
    /// </summary>
    private void CheckOnMenu()
    {
        if (OnMenuStack.Count == 0)
        {
            //�ֻ��� �޴��� �ൿ �ܰ� �޴� �������� üũ(������ ��� �ִٸ� �ൿ �ܰ� �޴�)
            return;
        }

        PopMenu = OnMenuStack.Pop();    //������ �޴� ���ÿ��� ���� ������ �޴��� pop

        //pop�� �޴��� ���� �б�
        switch (PopMenu)
        {
            case NowOnMenu.SkillMenu:
                HideSkillMenu();
                HideAffinityMarkAll();      //�� ǥ�� ��ũ ���߱�
                SelectActMenuCanvas.enabled = true;
                break;
            case NowOnMenu.SkillSelected:
                break;
            case NowOnMenu.ItemMenu:
                HideItemMenu();
                SelectActMenuCanvas.enabled = true;
                break;
        }
    }

    /// <summary>
    /// �޴��� ó�� ���� ���� �⺻������ ���õǴ� ��ư�� ��ų
    /// </summary>
    private void SetDefaultSelectedActMenuButton()
    {
        NowSelectedButton = SkillMenuButton;
        NowSelectedButton.image.color = SelectedActButtonColor;
    }

    
    public bool ReturnIsButtonChanged()
    {
        return IsSelectedChanged;
    }

    public NowOnMenu ReturnNowChangedMenu()
    {
        return NowSelectedMenu;
    }

    #region SelectActMenuButton

    /// <summary>
    /// ��ư Ŭ���� Ȱ��ȭ �� ��ư�� �� �� �� ������ ������, �ٸ� ��ư�� �������� �Ǻ��ϴ� �Լ�
    /// </summary>
    public void ClickActButton()
    {
        Button Tmp = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();  //Ŭ���� ��ư �ӽ÷� ��������

        if (NowSelectedButton == Tmp)
        {
            //���� Ȱ��ȭ�� ��ư�� ������ Ŭ���� ��ư�� ��ġ�� ���� �ൿ
            CompareWhichActButtonClicked(Tmp);
            IsSelectedChanged = true;
        }
        else
        {
            //��ġ���� ������ Ȱ��ȭ�� ��ư�� ������
            NowSelectedButton.image.color = UnselectedActButtonColor;
            NowSelectedButton = Tmp;
            Tmp.image.color = SelectedActButtonColor;
            Debug.Log("Ȱ��ȭ �� ��ư ��ü");
            IsSelectedChanged = false;
        }
    }

    /// <summary>
    /// � ��ư�� ���� Ŭ�� �Ǿ����� ����
    /// </summary>
    /// <param name="Tmp">���� ���� ��ư</param>
    private void CompareWhichActButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
        Debug.Log(TmpButtonName);

        SelectActMenuCanvas.enabled = false;

        switch (TmpButtonName)
        {
            case "SkillMenuButton":
                ShowSkillMenu();        //��ų ���� �޴� ����
                NowSelectedMenu = NowOnMenu.SkillMenu;
                ShowAffinityMarkAll(NowSelectedSkillData.ReturnSkillDataRec());
                OnMenuStack.Push(NowOnMenu.SkillMenu);      //���� �޴� ���ÿ� ��ų �޴� push
                break;
            case "ItemMenuButton":
                ShowItemMenu();
                OnMenuStack.Push(NowOnMenu.ItemMenu);      //���� �޴� ���ÿ� ������ �޴� push
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
                Debug.Log("����: ��ư�� �νĵ��� ����");
                break;
        }
    }

    #endregion

    /// <summary>
    /// �ൿ �ܰ� �޴� ���̱�
    /// </summary>
    public void ShowActMenu()
    {
        SelectActMenuCanvas.enabled = true;
    }
    
    /// <summary>
    /// �ൿ �ܰ� �޴� ���߱�
    /// </summary>
    public void HideActMenu()
    {
        SelectActMenuCanvas.enabled = false;
    }
    
    
    /// <summary>
    /// ��ųâ���� �ڷΰ��� ��ư
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
