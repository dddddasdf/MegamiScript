using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Gpm.Ui;

public class SkillCellData : InfiniteScrollData
{
    private SkillDataRec Skillinfo;
    private bool IsSelected = false;

    public void SetSkillInfo(SkillDataRec NewData)
    {
        Skillinfo = NewData;
    }

    public SkillDataRec ReturnSkillInfo()
    {
        return Skillinfo;
    }

    public void SwapIsSelected(bool Swap)
    {
        IsSelected = Swap;
    }

    public bool ReturnIsSelected()
    {
        return IsSelected;
    }
}

public class BattleSkillMenuUI : MonoBehaviour
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

    private List<SkillCellData> ShowSkillList = new List<SkillCellData>();        //UI���� �����ֱ� ���� ��ų ������
    private SkillCellData NowSelectedData = new SkillCellData();
    public InfiniteScroll VerticalISkillList = null;


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
    /// ���� �ൿ���� ĳ������ ��ų ����Ʈ ����, �ൿ�� �ٲ� ������ �� ���� ȣ���Ѵ�(��ų �޴��� �� ������ ȣ���ϴ� �� �ƴϴ�)
    /// </summary>
    /// <param name="NowTurnCharacter">���� �ൿ ���� �Ʊ� ĳ����</param>
    public void SetSkillList(OnBattleObject NowTurnCharacter)
    {
        if (ShowSkillList != null)
            ShowSkillList.Clear();      //��ų �����Ͱ� �� ���� ��� �� �� ����ش�

        if (VerticalISkillList != null)
            VerticalISkillList.Clear();     //���������� ����ش�

        
        int NumberOfSkill = NowTurnCharacter.ReturnMemberData().ReturnNumberOfSkill();

        for (int i = 0; i < NumberOfSkill; i++)
        {
            SkillCellData NewSkillCellData = new SkillCellData();
            NewSkillCellData.SetSkillInfo(NowTurnCharacter.ReturnMemberData().ReturnSkillByIndex(i));      //���� �ൿ���� ĳ���Ϳ��Լ� ��ų �����͸� ��ȯ �޾� ����

            if (i == 0)
            {
                NewSkillCellData.SwapIsSelected(true);
                NowSelectedData = NewSkillCellData;
            }

            ShowSkillList.Add(NewSkillCellData);
            VerticalISkillList.InsertData(NewSkillCellData);
        }
    }



    /// <summary>
    /// ��ųâ ���� �⺻������ Ȱ��ȭ�Ǵ� ��ų ��ư�� ù��° ��ų - �˰��� ���ƾ����� ���� ����
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
        }
    }

    private void CompareWhichSkillButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
    }
}
