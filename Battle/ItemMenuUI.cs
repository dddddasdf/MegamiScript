using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClassInMenu
{

}

public class ItemMenuUI : MonoBehaviour
{
    
    
    public InfiniteScroll VerticalItemList = null;
    private List<UsableItemInformaiton> ItemList = new List<UsableItemInformaiton>();   //이거 존재 이유가 있을지? 잘 몰?루

    private int Count = 1;  //테스트 하드코딩용

    private void Awake()
    {
        InsertItemData();

        int i = 0;
    }

    private void Start()
    {
        VerticalItemList.AddSelectCallback((data) =>
        {
            
        });

    }

    private void InsertItemData()
    {
        //본게임이라면 이 구간에서 아이템 관리자든 뭐든 데이터 받아오는 함수를 넣어야 함. 지금은 일단 대충 하드코딩 ㄱㄱ

        for (int i = 0; i < 25; i++)
        {
            UsableItemInformaiton NewItemInfo = new UsableItemInformaiton();
            NewItemInfo.Name = "보옥" + Count.ToString();
            NewItemInfo.NumberOfNowPossess = Count;
            NewItemInfo.NumberOfMax = Count + 10;
            Count++;

            ItemList.Add(NewItemInfo);

            VerticalItemList.InsertData(NewItemInfo);
        }
    }

    /// <summary>
    /// 갖고 있던 아이템이 다 써서 0개가 되었을 경우 리스트에서 해제
    /// </summary>
    public void RemoveItemData()
    {

    }

    /// <summary>
    /// 아이템 사용시 개수 차감 반영
    /// </summary>
    public void UpdateData()
    {

    }
}