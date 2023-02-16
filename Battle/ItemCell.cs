using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gpm.Ui;

public class ItemCell : InfiniteScrollItem
{
    public TextMeshProUGUI ItemName;    //�����۸�
    public TextMeshProUGUI NumberOfPossess; //������ ������
    public Image ItemThumbnail; //������ �����

    private UsableItemInformaiton ItemInfo;
    private int CellIndex;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        UsableItemInformaiton ItemInfo = (UsableItemInformaiton)scrollData;
        ItemName.text = ItemInfo.Name.ToString();
        NumberOfPossess.text = ItemInfo.NumberOfNowPossess + "/" + ItemInfo.NumberOfMax.ToString();

    }

    public void GetCellInfo(UsableItemInformaiton ItemInfo, int CellIndex)
    {
        this.ItemInfo = ItemInfo;
        this.CellIndex = CellIndex;
    }

    /// <summary>
    /// �� ���� ����
    /// </summary>
    private void SetCellInfo()
    {
        ItemName.text = ItemInfo.Name.ToString();
        NumberOfPossess.text = ItemInfo.NumberOfNowPossess + "/" + ItemInfo.NumberOfMax.ToString();
    }

    
}