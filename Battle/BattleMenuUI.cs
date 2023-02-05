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



    private Button NowSelectedButton;   //Ȱ��ȭ ���� ��ư �����

    private Color SelectedColor;    //���õ� ��ư ����
    private Color UnselectedColor;  //���õ��� ���� ��ư ����

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
    /// �޴��� ó�� ���� ���� �⺻������ ���õǴ� ��ư�� ��ų
    /// </summary>
    private void SetDefaultSelectedActMenuButton()
    {
        NowSelectedButton = SkillMenuButton;
        NowSelectedButton.image.color = SelectedColor;
    }

    /// <summary>
    /// ��ųâ�� ���� ���� �⺻������ ���õǴ� ��ư�� �ֻ����� ��ġ�� ��ų
    /// </summary>
    private void SetDefaultSelectedSkilButton()
    {

    }

    public void SwapMenuCanvas(int ToSwapMenuIndex)
    {

    }

    /// <summary>
    /// ��ư Ŭ���� Ȱ��ȭ �� ��ư�� �� �� �� ������ ������, �ٸ� ��ư�� �������� �Ǻ��ϴ� �Լ�
    /// </summary>
    public void ClickButton()
    {
        Button Tmp = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();  //Ŭ���� ��ư �ӽ÷� ��������

        if (NowSelectedButton == Tmp)
        {
            //���� Ȱ��ȭ�� ��ư�� ������ Ŭ���� ��ư�� ��ġ�� ���� �ൿ
            Debug.Log("��ư ��ġ");
        }
        else
        {
            //��ġ���� ������ Ȱ��ȭ�� ��ư�� ������
            NowSelectedButton.image.color = UnselectedColor;
            NowSelectedButton = Tmp;
            Tmp.image.color = SelectedColor;
            Debug.Log("Ȱ��ȭ �� ��ư ��ü");
        }
    }
}
