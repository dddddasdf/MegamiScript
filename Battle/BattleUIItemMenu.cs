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
    private bool IsSelected = false; //�ش� ���� ���� �Ǿ����� Ȯ�ο�
    
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
    [SerializeField] private Canvas SelectItemMenuCanvas;    
    private List<ItemCellData> ShowInventoryList = new List<ItemCellData>();        //UI���� �����ֱ� ���� �κ��丮 ������

    public InfiniteScroll VerticalItemList = null;

    private int Count = 1;  //�׽�Ʈ �ϵ��ڵ���
    private ItemCellData NowSelectedItemData = new ItemCellData();  //���� ���õ� ������

    private void SetItemScroll()
    {
        VerticalItemList.AddSelectCallback((data) =>
        {
            if ((ItemCellData)data != NowSelectedItemData)
            {
                //���� ���õ� �����Ϳ� ���� ���õ� �����Ͱ� ���� ���� ���
                UpdateData((ItemCellData)data);
            }
            else
            {
                //���� ���õ� �����Ϳ� ���� ���õ� �����Ͱ� ���� ���
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
            //�ӽÿ� �ϵ��ڵ�
            UsableItemInformaiton NewItemInfo = new UsableItemInformaiton();
            NewItemInfo.Name = "����" + Count.ToString();
            NewItemInfo.NumberOfNowPossess = Count;
            NewItemInfo.NumberOfMax = Count + 10;
            Count++;
            //�ӽÿ� �ϵ��ڵ� ����

            ItemCellData NewItemCellData = new ItemCellData();
            NewItemCellData.ItemInfo = NewItemInfo;

            if (i == 0)
            {
                NewItemCellData.SetIsSelected(true);  //���� ù��°�� ���õ�
                NowSelectedItemData = NewItemCellData;
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
            //������ ������ 0��: ������ �� ������ �������� �ʴ´�
            return;
        }

        NowSelectedItemData.SetIsSelected(false); //���� ���� ���̴� �����ۼ��� ���� ����
        VerticalItemList.UpdateData(NowSelectedItemData);   //���� ���� ������Ʈ
        NowSelectedItemData = NewSelectedData;  //���� ���� ���� �� ��ü
        NowSelectedItemData.SetIsSelected(true);  //���� �� ������Ʈ
        VerticalItemList.UpdateData(NowSelectedItemData);   //���� ���õ� �� ������Ʈ
    }
}