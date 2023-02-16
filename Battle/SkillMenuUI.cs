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
    private int NowSelectedSkillIndex;  //���� ���� ���� ��ų ��ư �ε���: Ű���� �Է¿� �� �����

    //���õ� ��ư ���� ����
    private Button NowSelectedButton;   //���õ� ��ư �����
    private Color SelectedSkillButtonColor; //���õ� ��ų ��ư ����+����
    private Color UnselectedSkillButtonColor;   //���õ��� ���� ��ų ��ư ����+����

    private int ShowNumberofSkill;  //ĳ���Ͱ� �����ϰ� �ִ� ��ų��-�ش� ���� ���缭 ���� ���


    #endregion

    private void Awake()
    {
        SelectedSkillButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
        UnselectedSkillButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100 / 255f);

        NowSelectedSkillIndex = 0;
        SetDefaultSelectedSkillButton();
    }

    /// <summary>
    /// ��ų �޴�â ���̱�
    /// </summary>
    public void ShowSkillMenu()
    {
        SelectSkillMenuCanvas.enabled = true;
    }

    /// <summary>
    /// ��ų �޴�â �����
    /// </summary>
    public void HideSkillMenu()
    {
        SelectSkillMenuCanvas.enabled = false;
    }



    /// <summary>
    /// ��ųâ ���� �⺻������ Ȱ��ȭ�Ǵ� ��ų ��ư�� ù��° ��ų
    /// </summary>
    private void SetDefaultSelectedSkillButton()
    {
        NowSelectedButton = SkillButton[NowSelectedSkillIndex];
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
            Debug.Log(NowSelectedSkillIndex);
        }
    }

    private void CompareWhichSkillButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
        Debug.Log(TmpButtonName);
    }
}
