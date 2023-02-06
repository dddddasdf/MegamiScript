using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleMenuUI : MonoBehaviour
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


    [SerializeField] private Canvas SelectSkillMenuCanvas;  //��ų �޴� ĵ����
    [SerializeField] private List<Button> SkillButton;
    private int NowSelectedSkillIndex;  //���� ���� ���� ��ų ��ư �ε���: Ű���� �Է¿� �� �����


    private Button NowSelectedButton;   //Ȱ��ȭ ���� ��ư �����

    private Color SelectedActButtonColor;    //���õ� �ൿ ��ư ����
    private Color UnselectedActButtonColor;  //���õ��� ���� �ൿ ��ư ����

    private Color SelectedSkillButtonColor; //���õ� ��ų ��ư ����+����
    private Color UnselectedSkillButtonColor;   //���õ��� ���� ��ų ��ư ����+����

    private int ShowNumberofSkill;  //ĳ���Ͱ� �����ϰ� �ִ� ��ų��-�ش� ���� ���缭 ���� ���

    
    //Ű�Է� ���� ����
    private bool IsCanInput;    //������ Ű�Է��� ���� ������ ���� ����-���� ���� ������ Ű �Է��� �ƿ� ������ ���� �ϴ� Ȯ�ο�... �������� ���۵��� ���� �� ����
    private Coroutine GetInputCoroutine;


    #endregion

    private void Awake()
    {
        //�ʱ�ȭ
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
    /// �޴��� ó�� ���� ���� �⺻������ ���õǴ� ��ư�� ��ų
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
    /// ��ư Ŭ���� Ȱ��ȭ �� ��ư�� �� �� �� ������ ������, �ٸ� ��ư�� �������� �Ǻ��ϴ� �Լ�
    /// </summary>
    public void ClickActButton()
    {
        Button Tmp = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();  //Ŭ���� ��ư �ӽ÷� ��������

        if (NowSelectedButton == Tmp)
        {
            //���� Ȱ��ȭ�� ��ư�� ������ Ŭ���� ��ư�� ��ġ�� ���� �ൿ
            CompareWhichActButtonClicked(Tmp);
        }
        else
        {
            //��ġ���� ������ Ȱ��ȭ�� ��ư�� ������
            NowSelectedButton.image.color = UnselectedActButtonColor;
            NowSelectedButton = Tmp;
            Tmp.image.color = SelectedActButtonColor;
            Debug.Log("Ȱ��ȭ �� ��ư ��ü");
        }
    }

    /// <summary>
    /// � ��ư�� Ŭ�� �Ǿ����� ����
    /// </summary>
    /// <param name="Tmp">���� ���� ��ư</param>
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
                Debug.Log("����: ��ư�� �νĵ��� ����");
                break;
        }
    }

    #endregion

    #region SelectSkillMenuButton

    /// <summary>
    /// ��ųâ ���� �⺻������ Ȱ��ȭ�Ǵ� ��ų ��ư�� ù��° ��ų
    /// </summary>
    private void SetDefaultSelectedSkillButton()
    {
        NowSelectedButton = SkillButton[0]; //ù��° ��ų�� Ȱ��ȭ�� ��ư����
        NowSelectedButton.image.color = SelectedSkillButtonColor;
    }

    public void ClickSkillButton()
    {
        Button Tmp = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();  //Ŭ���� ��ư �ӽ÷� ��������

        if (NowSelectedButton == Tmp)
        {
            //���� Ȱ��ȭ�� ��ư�� ������ Ŭ���� ��ư�� ��ġ�� ���� �ൿ
            CompareWhichSkillButtonClicked(Tmp);
        }
        else
        {
            //��ġ���� ������ Ȱ��ȭ�� ��ư�� ������
            NowSelectedButton.image.color = UnselectedSkillButtonColor;
            NowSelectedButton = Tmp;
            Tmp.image.color = SelectedSkillButtonColor;
            NowSelectedSkillIndex = int.Parse(Regex.Replace(Tmp.name, @"[^0-9]", ""));
            Debug.Log("Ȱ��ȭ �� ��ư ��ü");
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
        //        Debug.Log("����: ��ư�� �νĵ��� ����");
        //        break;
        //}
    }



    #endregion


    #region GetInput
    /// <summary>
    /// ���� Ű���� ���� Ű�Է� �޴� ��
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetPlayerKeyboardInput()
    {
        

        while (true)
        {
            //���� ��ư
            if (Input.GetKeyDown(KeyCode.W))
            {
                //Ű�Է� ����
                if (NowSelectedSkillIndex != 1)
                {
                    NowSelectedButton.image.color = UnselectedSkillButtonColor;
                    NowSelectedSkillIndex--;
                    NowSelectedButton = SkillButton[NowSelectedSkillIndex - 1];
                    NowSelectedButton.image.color = SelectedSkillButtonColor;
                    Debug.Log(NowSelectedSkillIndex);
                }

            }

            //�Ʒ��� ��ư
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
        

        

        //���� ��ư
        //if (Input.GetKey(KeyCode.F))
            ;
    }

    #endregion
}
