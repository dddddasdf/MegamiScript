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
    private List<UsableItemInformaiton> ItemList = new List<UsableItemInformaiton>();   //�̰� ���� ������ ������? �� ��?��

    private int Count = 1;  //�׽�Ʈ �ϵ��ڵ���

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
        //�������̶�� �� �������� ������ �����ڵ� ���� ������ �޾ƿ��� �Լ��� �־�� ��. ������ �ϴ� ���� �ϵ��ڵ� ����

        for (int i = 0; i < 25; i++)
        {
            UsableItemInformaiton NewItemInfo = new UsableItemInformaiton();
            NewItemInfo.Name = "����" + Count.ToString();
            NewItemInfo.NumberOfNowPossess = Count;
            NewItemInfo.NumberOfMax = Count + 10;
            Count++;

            ItemList.Add(NewItemInfo);

            VerticalItemList.InsertData(NewItemInfo);
        }
    }

    /// <summary>
    /// ���� �ִ� �������� �� �Ἥ 0���� �Ǿ��� ��� ����Ʈ���� ����
    /// </summary>
    public void RemoveItemData()
    {

    }

    /// <summary>
    /// ������ ���� ���� ���� �ݿ�
    /// </summary>
    public void UpdateData()
    {

    }
}