using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gpm.Ui;

public class ItemCell : InfiniteScrollItem
{
    [SerializeField] private TextMeshProUGUI ItemName;    //아이템명
    [SerializeField] private TextMeshProUGUI NumberOfPossess; //아이템 소지수
    [SerializeField] private Image ItemThumbnail; //아이템 썸네일
    [SerializeField] private Image ThisCellImage;

    private ItemCellData ThisCellData;  //이 오브젝트가 보여줄 아이템 내용

    private Color SelectedButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255 / 255f); //선택된 스킬 버튼 색상+투명도
    private Color UnselectedButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100 / 255f);   //선택되지 않은 스킬 버튼 색상+투명도


    /// <summary>
    /// 보여줄 내용 교체
    /// </summary>
    /// <param name="scrollData"></param>
    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        ThisCellData = (ItemCellData)scrollData;
        ItemName.text = ThisCellData.ItemInfo.Name.ToString();
        NumberOfPossess.text = ThisCellData.ItemInfo.NumberOfNowPossess.ToString() + "/" + ThisCellData.ItemInfo.NumberOfMax.ToString();

        if (ThisCellData.ReturnIsSelected())
            ThisCellImage.color = SelectedButtonColor;
        else if (ThisCellImage.color == SelectedButtonColor && ThisCellData.ReturnIsSelected() == false)
            ThisCellImage.color = UnselectedButtonColor;
    }

    /// <summary>
    /// 아이템 셀 클릭
    /// </summary>
    public void Clicked()
    {
        OnSelect(); //액션 호출
    }
}