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
    private int NowSelectedSkillIndex;  //현재 선택 중인 스킬 버튼 인덱스: 키보드 입력용 및 저장용


    private List<SkillCellData> ShowSkillList = new List<SkillCellData>();        //UI에서 보여주기 위한 스킬 데이터
    private SkillCellData NowSelectedSkillData = new SkillCellData();               //현재 선택된 스킬
    public InfiniteScroll VerticalISkillList = null;

    [SerializeField] private ActivateSkillButton ActivatedSkillButtonObject;
    

    #endregion

    /// <summary>
    /// 스킬 UI 초기화
    /// </summary>
    private void InitSkillUI()
    {

        NowSelectedSkillIndex = 0;
        //SetDefaultSelectedSkillButton();
    }

    /// <summary>
    /// 스킬 스크롤 세팅
    /// </summary>
    private void SetSkillScroll()
    {
        VerticalISkillList.AddSelectCallback((data) =>
        {
            if ((SkillCellData)data != NowSelectedSkillData)
            {
                //원래 선택된 데이터와 새로 선택된 데이터가 같지 않을 경우
                UpdateData((SkillCellData)data);
                HideAffinityMarkAll();      //상성 표시 마크 한 번 꺼준다
                ShowAffinityMarkAll(NowSelectedSkillData.ReturnSkillDataRec());         //스킬 데이터가 갱신되었으니 상성 표시 마크도 갱신
            }
            else
            {
                //원래 선택된 데이터와 새로 선택된 데이터가 같을 경우: 배틀매니저의 저장용 전역변수에 현재 선택된 스킬 데이터를 담아준다
                BattleManager.SelectedSkillDataBuffer = NowSelectedSkillData;
                IsButtonDoubleclicked = true;       //같은 버튼이 클릭되었음을 기록
            }

            while (JobQueue.Count > 0)
            {
                Action action = JobQueue.Dequeue();
                action?.Invoke();
            }
            JobQueue.Clear();       //작업큐 비워주기
        });
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
    /// 현재 행동턴인 캐릭터의 스킬 리스트 세팅, 행동턴 바뀔 때마다 한 번씩 호출한다(스킬 메뉴로 들어갈 때마다 호출하는 게 아니다)
    /// </summary>
    /// <param name="NowTurnCharacter">현재 행동 턴인 아군 캐릭터</param>
    public void SetSkillList(OnBattlePartyObject NowTurnCharacter)
    {
        if (ShowSkillList != null)
            ShowSkillList.Clear();      //스킬 데이터가 차 있을 경우 한 번 비워준다

        if (VerticalISkillList != null)
            VerticalISkillList.Clear();     //마찬가지로 비워준다

        
        int NumberOfSkill = NowTurnCharacter.ReturnMemberData().ReturnNumberOfSkill();

        for (int i = 0; i < NumberOfSkill; i++)
        {
            SkillCellData NewSkillCellData = new SkillCellData();
            NewSkillCellData.SetSkillInfo(NowTurnCharacter.ReturnMemberData().ReturnSkillByIndex(i));      //현재 행동턴인 캐릭터에게서 스킬 데이터를 반환 받아 저장

            if (i == 0)
            {
                //첫번째 스킬을 선활성화 상태로 미리 지정
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
        JobQueue.Clear();       //작업큐 비워주기
    }

    /// <summary>
    /// 스킬셀 업데이트
    /// </summary>
    /// <param name="NewSelectedData"></param>
    private void UpdateData(SkillCellData NewSelectedData)
    {
        NowSelectedSkillData.SetIsSelected(false); //원래 선택 중이던 아이템셀은 선택 해제
        VerticalISkillList.UpdateData(NowSelectedSkillData);   //선택 해제 업데이트
        NowSelectedSkillData = NewSelectedData;  //지금 선택 중인 셀 교체
        NowSelectedSkillData.SetIsSelected(true);  //선택 중 업데이트
        VerticalISkillList.UpdateData(NowSelectedSkillData);   //새로 선택된 셀 업데이트
    }

    /// <summary>
    /// 스킬을 고르고 타겟 선정하는 UI 진입-적 대상 스킬
    /// </summary>
    public void ShowEnemyTargetSelectUI()
    {
        HideAffinityMarkAll();          //상성 표시 한 번 감춰준다
        ActivatedSkillButtonObject.ShowActivateSkillButton();
        ActivatedSkillButtonObject.UpdateActivateSkillData(BattleManager.SelectedSkillDataBuffer);
        ShowEnemyInformation();
        HideSkillMenu();

        if (NowSelectedSkillData.ReturnSkillDataRec().ReturnNumberOfTarget() == NumberOfTarget.Single)
        {
            //단일 공격 스킬이면 단일 상성 표시만 띄우기
            ShowAffinityMarkSingle();
        }
        else
        {
            //그 외면 전체 표시
            ShowAffinityMarkAll(NowSelectedSkillData.ReturnSkillDataRec());
        }

        OnMenuStack.Push(NowOnMenu.SkillSelected);      //UI 스택에 push
    }

    /// <summary>
    /// 타겟 선정하는 UI에서 빠져나가기-적 대상 스킬
    /// </summary>
    public void HideEnemyTargetSelectUI()
    {
        ActivatedSkillButtonObject.HideActivateSkillButton();
        HideEnemyInformation();
        HideAffinityMarkAll();
    }


    public SkillDataRec ReturnNowSelectedSkillData()
    {
        return NowSelectedSkillData.ReturnSkillDataRec();
    }
}
