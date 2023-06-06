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

    [SerializeField] private Sprite IconPhysical;
    [SerializeField] private Sprite IconGun;
    [SerializeField] private Sprite IconFire;
    [SerializeField] private Sprite IconIce;
    [SerializeField] private Sprite IconElectric;
    [SerializeField] private Sprite IconForce;
    [SerializeField] private Sprite IconLight;
    [SerializeField] private Sprite IconDark;
    [SerializeField] private Sprite IconAlmighty;
    [SerializeField] private Sprite IconRecover;
    [SerializeField] private Sprite IconSupport;
    [SerializeField] private Sprite IconAilment;


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
        SkillName.text = ThisCellData.ReturnSkillDataRec().ReturnName();       //갱신해야 할 스킬 이름으로 이름 변경
        UseMP.text = ThisCellData.ReturnSkillDataRec().ReturnUseMP().ToString() + "MP";        //갱신해야 할 스킬이 소모하는 마나량으로 변경

        //갱신해야 할 스킬 아이콘으로 스프라이트 변경
        switch (ThisCellData.ReturnSkillDataRec().ReturnSkillType())
        {
            case SkillTypeSort.Physical:
                Skillicon.sprite = IconPhysical;
                break;
            case SkillTypeSort.Gun:
                Skillicon.sprite = IconGun;
                break;
            case SkillTypeSort.Fire:
                Skillicon.sprite = IconFire;
                break;
            case SkillTypeSort.Ice:
                Skillicon.sprite = IconIce;
                break;
            case SkillTypeSort.Electric:
                Skillicon.sprite = IconElectric;
                break;
            case SkillTypeSort.Force:
                Skillicon.sprite = IconForce;
                break;
            case SkillTypeSort.Light:
                Skillicon.sprite = IconLight;
                break;
            case SkillTypeSort.Dark:
                Skillicon.sprite = IconDark;
                break;
            case SkillTypeSort.Almighty:
                Skillicon.sprite = IconAlmighty;
                break;
            case SkillTypeSort.Recover:
                Skillicon.sprite = IconRecover;
                break;
            case SkillTypeSort.Support:
                Skillicon.sprite = IconSupport;
                break;
            case SkillTypeSort.Ailment:
                Skillicon.sprite = IconAilment;
                break;
        }
        
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
