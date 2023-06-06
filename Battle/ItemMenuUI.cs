using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemCellData : InfiniteScrollData
{
    public UsableItemInformaiton ItemInfo;  //슬롯이 받아오게 될 소지 아이템 데이터
    private bool IsSelected = false; //해당 슬롯 선택 되었는지 확인용
    
    public void SetIsSelected(bool Value)
    {
        IsSelected = Value;
    }

    public bool ReturnIsSelected()
    {
        return IsSelected;
    }
}

public class ItemMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas SelectItemMenuCanvas;    
    private List<ItemCellData> ShowInventoryList = new List<ItemCellData>();        //UI에서 보여주기 위한 인벤토리 데이터

    public InfiniteScroll VerticalItemList = null;

    private int Count = 1;  //테스트 하드코딩용
    private ItemCellData NowSelectedData = new ItemCellData();  //현재 선택된 데이터

    private void Awake()
    {
        InsertItemData();
    }

    private void Start()
    {
        VerticalItemList.AddSelectCallback((data) =>
        {
            if ((ItemCellData)data != NowSelectedData)
            {
                //원래 선택된 데이터와 새로 선택된 데이터가 같지 않을 경우
                UpdateData((ItemCellData)data);
            }
            else
            {
                //원래 선택된 데이터와 새로 선택된 데이터가 같을 경우
            }
        });
    }

    /// <summary>
    /// 아이템 메뉴창 보이기
    /// </summary>
    public void ShowItemMenu()
    {
        SelectItemMenuCanvas.enabled = true;
    }

    /// <summary>
    /// 아이템 메뉴창 숨기기
    /// </summary>
    public void HideItemMenu()
    {
        SelectItemMenuCanvas.enabled = false;
    }

    private void InsertItemData()
    {
        //본게임이라면 이 구간에서 아이템 관리자든 뭐든 데이터 받아오는 함수를 넣어야 함. 지금은 일단 대충 하드코딩 ㄱㄱ


        for (int i = 0; i < 25; i++)
        {
            //임시용 하드코딩
            UsableItemInformaiton NewItemInfo = new UsableItemInformaiton();
            NewItemInfo.Name = "보옥" + Count.ToString();
            NewItemInfo.NumberOfNowPossess = Count;
            NewItemInfo.NumberOfMax = Count + 10;
            Count++;
            //임시용 하드코딩 종료

            ItemCellData NewItemCellData = new ItemCellData();
            NewItemCellData.ItemInfo = NewItemInfo;

            if (i == 0)
            {
                NewItemCellData.SetIsSelected(true);  //제일 첫번째가 선택됨
                NowSelectedData = NewItemCellData;
            }

            ShowInventoryList.Add(NewItemCellData);

            VerticalItemList.InsertData(NewItemCellData);
        }
    }

    /// <summary>
    /// 갖고 있던 아이템이 다 써서 0개가 되었을 경우 리스트에서 해제
    /// </summary>
    public void RemoveItemData()
    {

    }

    /// <summary>
    /// 현재 선택 중인 아이템셀과 다른 아이템셀을 클릭했을 경우
    /// </summary>
    public void UpdateData(ItemCellData NewSelectedData)
    {
        if (ShowInventoryList.Count <= 0)
        {
            //아이템 개수가 0개: 보여줄 게 없으니 업뎃하지 않는다
            return;
        }

        NowSelectedData.SetIsSelected(false); //원래 선택 중이던 아이템셀은 선택 해제
        VerticalItemList.UpdateData(NowSelectedData);   //선택 해제 업데이트
        NowSelectedData = NewSelectedData;  //지금 선택 중인 셀 교체
        NowSelectedData.SetIsSelected(true);  //선택 중 업데이트
        VerticalItemList.UpdateData(NowSelectedData);   //새로 선택된 셀 업데이트
    }
}