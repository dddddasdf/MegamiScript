using Gpm.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemCellData : InfiniteScrollData
{
    public UsableItemInformaiton ItemInfo;  //������ �޾ƿ��� �� ���� ������ ������
    public bool IsSelected = false; //�ش� ���� ���� �Ǿ����� Ȯ�ο�
}

public class ItemMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas SelectItemMenuCanvas;    
    private List<ItemCellData> ShowInventoryList = new List<ItemCellData>();        //UI���� �����ֱ� ���� �κ��丮 ������

    public InfiniteScroll VerticalItemList = null;

    private int Count = 1;  //�׽�Ʈ �ϵ��ڵ���
    private ItemCellData NowSelectedData = new ItemCellData();  //���� ���õ� ������

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
                UpdateData((ItemCellData)data);
            }
            else
            {

            }
        });
    }

    /// <summary>
    /// ������ �޴�â ���̱�
    /// </summary>
    public void ShowItemMenu()
    {
        SelectItemMenuCanvas.enabled = true;
    }

    /// <summary>
    /// ������ �޴�â �����
    /// </summary>
    public void HideItemMenu()
    {
        SelectItemMenuCanvas.enabled = false;
    }

    private void InsertItemData()
    {
        //�������̶�� �� �������� ������ �����ڵ� ���� ������ �޾ƿ��� �Լ��� �־�� ��. ������ �ϴ� ���� �ϵ��ڵ� ����


        for (int i = 0; i < 25; i++)
        {
            UsableItemInformaiton NewItemInfo = new UsableItemInformaiton();
            NewItemInfo.Name = "����" + Count.ToString();
            NewItemInfo.NumberOfNowPossess = Count;
            NewItemInfo.NumberOfMax = Count + 10;
            Count++;

            ItemCellData NewItemCellData = new ItemCellData();
            NewItemCellData.ItemInfo = NewItemInfo;

            if (i == 0)
            {
                NewItemCellData.IsSelected = true;  //���� ù��°�� ���õ�
                NowSelectedData = NewItemCellData;
            }

            ShowInventoryList.Add(NewItemCellData);

            VerticalItemList.InsertData(NewItemCellData);
        }
    }

    /// <summary>
    /// ���� �ִ� �������� �� �Ἥ 0���� �Ǿ��� ��� ����Ʈ���� ����
    /// </summary>
    public void RemoveItemData()
    {

    }

    /// <summary>
    /// ���� ���� ���� �����ۼ��� �ٸ� �����ۼ��� Ŭ������ ���
    /// </summary>
    public void UpdateData(ItemCellData NewSelectedData)
    {
        if (ShowInventoryList.Count <= 0)
        {
            return;
        }

        NowSelectedData.IsSelected = false; //���� ���� ���̴� �����ۼ��� ���� ����
        VerticalItemList.UpdateData(NowSelectedData);   //���� ���� ������Ʈ
        NowSelectedData = NewSelectedData;  //���� ���� ���� �� ��ü
        NowSelectedData.IsSelected = true;  //���� �� ������Ʈ
        VerticalItemList.UpdateData(NowSelectedData);   //���� ���õ� �� ������Ʈ
    }
}