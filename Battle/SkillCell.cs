using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gpm.Ui;

public class SkillCell : InfiniteScrollItem
{
    [SerializeField] private TextMeshProUGUI SkillName;    //스킬명
    [SerializeField] private TextMeshProUGUI UseMP; //소모 마나
    [SerializeField] private Image Skillicon; //스킬 아이콘
    [SerializeField] private Image ThisCellImage;

    private SkillCellData ThisCellData;  //이 오브젝트가 보여줄 스킬 내용

    private Color SelectedButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255 / 255f); //선택된 스킬 버튼 색상+투명도
    private Color UnselectedButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100 / 255f);   //선택되지 않은 스킬 버튼 색상+투명도


    /// <summary>
    /// 보여줄 내용 교체
    /// </summary>
    /// <param name="scrollData"></param>
    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        ThisCellData = (SkillCellData)scrollData;
        SkillName.text = ThisCellData.ReturnSkillInfo().ReturnName();
        UseMP.text = ThisCellData.ReturnSkillInfo().ReturnUseMP().ToString() + "MP";
        
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
