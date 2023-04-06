using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gpm.Ui;

public class SkillCell : InfiniteScrollItem
{
    [SerializeField] private TextMeshProUGUI SkillName;    //��ų��
    [SerializeField] private TextMeshProUGUI UseMP; //�Ҹ� ����
    [SerializeField] private Image Skillicon; //��ų ������
    [SerializeField] private Image ThisCellImage;

    private SkillCellData ThisCellData;  //�� ������Ʈ�� ������ ��ų ����

    private Color SelectedButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255 / 255f); //���õ� ��ų ��ư ����+����
    private Color UnselectedButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100 / 255f);   //���õ��� ���� ��ų ��ư ����+����


    /// <summary>
    /// ������ ���� ��ü
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
    /// ������ �� Ŭ��
    /// </summary>
    public void Clicked()
    {
        OnSelect(); //�׼� ȣ��
    }
}
