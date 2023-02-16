using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gpm.Ui;

public class ItemCell : InfiniteScrollItem
{
    public TextMeshProUGUI ItemName;    //아이템명
    public TextMeshProUGUI NumberOfPossess; //아이템 소지수
    public Image ItemThumbnail; //아이템 썸네일

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
    /// 셀 정보 세팅
    /// </summary>
    private void SetCellInfo()
    {
        ItemName.text = ItemInfo.Name.ToString();
        NumberOfPossess.text = ItemInfo.NumberOfNowPossess + "/" + ItemInfo.NumberOfMax.ToString();
    }

    
}