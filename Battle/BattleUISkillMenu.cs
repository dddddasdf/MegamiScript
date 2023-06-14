using System;
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

    public SkillDataRec ReturnSkillDataRec()
    {
        return Skillinfo;
    }

    public void SetIsSelected(bool Value)
    {
        IsSelected = Value;
    }

    public bool ReturnIsSelected()
    {
        return IsSelected;
    }
}

public partial class BattleUIManager : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private Canvas SelectSkillMenuCanvas;
    [SerializeField] private List<Button> SkillButton;
    private int NowSelectedSkillIndex;  //���� ���� ���� ��ų ��ư �ε���: Ű���� �Է¿� �� �����


    private List<SkillCellData> ShowSkillList = new List<SkillCellData>();        //UI���� �����ֱ� ���� ��ų ������
    private SkillCellData NowSelectedSkillData = new SkillCellData();
    public InfiniteScroll VerticalISkillList = null;

    public bool IsSelectedSkillChanged;          //���õ� ��ų�� �ٲ������ ��Ʋ�Ŵ����� ���� ��ȯ��

    

    #endregion

    /// <summary>
    /// ��ų UI �ʱ�ȭ
    /// </summary>
    private void InitSkillUI()
    {

        NowSelectedSkillIndex = 0;
        //SetDefaultSelectedSkillButton();
    }

    /// <summary>
    /// ��ų ��ũ�� ����
    /// </summary>
    private void SetSkillScroll()
    {
        VerticalISkillList.AddSelectCallback((data) =>
        {
            if ((SkillCellData)data != NowSelectedSkillData)
            {
                //���� ���õ� �����Ϳ� ���� ���õ� �����Ͱ� ���� ���� ���
                UpdateData((SkillCellData)data);
                ShowAffinityMarkAll(NowSelectedSkillData.ReturnSkillDataRec());         //��ų �����Ͱ� ���ŵǾ����� �� ǥ�� ��ũ�� ����
                IsSelectedSkillChanged = true;
            }
            else
            {
                //���� ���õ� �����Ϳ� ���� ���õ� �����Ͱ� ���� ���
                IsSelectedSkillChanged = false;
            }

            while (JobQueue.Count > 0)
            {
                Action action = JobQueue.Dequeue();
                action?.Invoke();
            }
            JobQueue.Clear();       //�۾�ť ����ֱ�
        });
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
    public void SetSkillList(OnBattlePartyObject NowTurnCharacter)
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
                NewSkillCellData.SetIsSelected(true);
                NowSelectedSkillData = NewSkillCellData;
            }

            ShowSkillList.Add(NewSkillCellData);
            VerticalISkillList.InsertData(NewSkillCellData);
        }

        while (JobQueue.Count > 0)
        {
            Action action = JobQueue.Dequeue();
            action?.Invoke();
        }
        JobQueue.Clear();       //�۾�ť ����ֱ�
    }

    /// <summary>
    /// ��ų�� ������Ʈ
    /// </summary>
    /// <param name="NewSelectedData"></param>
    private void UpdateData(SkillCellData NewSelectedData)
    {
        NowSelectedSkillData.SetIsSelected(false); //���� ���� ���̴� �����ۼ��� ���� ����
        VerticalISkillList.UpdateData(NowSelectedSkillData);   //���� ���� ������Ʈ
        NowSelectedSkillData = NewSelectedData;  //���� ���� ���� �� ��ü
        NowSelectedSkillData.SetIsSelected(true);  //���� �� ������Ʈ
        VerticalISkillList.UpdateData(NowSelectedSkillData);   //���� ���õ� �� ������Ʈ
    }


    /// <summary>
    /// ���õ� ��ų�� �ٲ������ ��Ʋ�Ŵ������� ����
    /// </summary>
    /// <param name="IsChanged"></param>
    public void IsSkillChanged(out bool IsChanged)
    {
        IsChanged = this.IsSelectedSkillChanged;
    }


    public SkillDataRec ReturnNowSelectedSkillData()
    {
        return NowSelectedSkillData.ReturnSkillDataRec();
    }
}
