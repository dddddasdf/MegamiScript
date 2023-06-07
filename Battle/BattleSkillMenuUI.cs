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

public class BattleSkillMenuUI : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private Canvas SelectSkillMenuCanvas;
    [SerializeField] private List<Button> SkillButton;
    private int NowSelectedSkillIndex;  //현재 선택 중인 스킬 버튼 인덱스: 키보드 입력용 및 저장용

    //선택된 버튼 관련 변수
    private Button NowSelectedButton;   //선택된 버튼 저장용
    private Color SelectedSkillButtonColor; //선택된 스킬 버튼 색상+투명도
    private Color UnselectedSkillButtonColor;   //선택되지 않은 스킬 버튼 색상+투명도

    private int ShowNumberofSkill;  //캐릭터가 습득하고 있는 스킬수-해당 수에 맞춰서 슬롯 출력

    private List<SkillCellData> ShowSkillList = new List<SkillCellData>();        //UI에서 보여주기 위한 스킬 데이터
    private SkillCellData NowSelectedData = new SkillCellData();
    public InfiniteScroll VerticalISkillList = null;

    public bool IsChanged;          //선택된 스킬이 바뀌었는지 배틀매니저와 정보 교환용

    private Queue<Action> JobQueue = new Queue<Action>();      //순차적으로 진행시킬 작업 목록
    public void AddJobQueueMethod(Action Method)
    {
        JobQueue.Enqueue(Method);
    }

    #endregion

    private void Awake()
    {
        SelectedSkillButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
        UnselectedSkillButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100 / 255f);

        NowSelectedSkillIndex = 0;
        SetDefaultSelectedSkillButton();
    }

    private void Start()
    {
        VerticalISkillList.AddSelectCallback((data) =>
        {
            if ((SkillCellData)data != NowSelectedData)
            {
                //원래 선택된 데이터와 새로 선택된 데이터가 같지 않을 경우
                UpdateData((SkillCellData)data);
                IsChanged = true;
            }
            else
            {
                //원래 선택된 데이터와 새로 선택된 데이터가 같을 경우
                IsChanged = false;
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
                NewSkillCellData.SetIsSelected(true);
                NowSelectedData = NewSkillCellData;
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
        //NowSelectedData.
        NowSelectedData.SetIsSelected(false); //원래 선택 중이던 아이템셀은 선택 해제
        VerticalISkillList.UpdateData(NowSelectedData);   //선택 해제 업데이트
        NowSelectedData = NewSelectedData;  //지금 선택 중인 셀 교체
        NowSelectedData.SetIsSelected(true);  //선택 중 업데이트
        VerticalISkillList.UpdateData(NowSelectedData);   //새로 선택된 셀 업데이트
    }


    /// <summary>
    /// 선택된 스킬이 바뀌었는지 배틀매니저에게 전달
    /// </summary>
    /// <param name="IsChanged"></param>
    public void IsSkillChanged(out bool IsChanged)
    {
        IsChanged = this.IsChanged;
    }


    public SkillDataRec ReturnNowSelectedSkillData()
    {
        return NowSelectedData.ReturnSkillDataRec();
    }




    //아래로 레거시

    /// <summary>
    /// 스킬창 들어가면 기본적으로 활성화되는 스킬 버튼은 첫번째 스킬 - 알고리즘 갈아엎으면 삭제 예정
    /// </summary>
    private void SetDefaultSelectedSkillButton()
    {
        NowSelectedButton = SkillButton[NowSelectedSkillIndex];
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
        }
    }

    private void CompareWhichSkillButtonClicked(Button Tmp)
    {
        string TmpButtonName = Tmp.name;
    }
}
