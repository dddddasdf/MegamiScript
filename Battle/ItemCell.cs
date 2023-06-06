using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gpm.Ui;

public class ItemCell : InfiniteScrollItem
{
    [SerializeField] private TextMeshProUGUI ItemName;    //�����۸�
    [SerializeField] private TextMeshProUGUI NumberOfPossess; //������ ������
    [SerializeField] private Image ItemThumbnail; //������ �����
    [SerializeField] private Image ThisCellImage;

    private ItemCellData ThisCellData;  //�� ������Ʈ�� ������ ������ ����

    private Color SelectedButtonColor = new Color(0 / 255f, 0 / 255f, 0 / 255f, 255 / 255f); //���õ� ��ų ��ư ����+����
    private Color UnselectedButtonColor = new Color(255 / 255f, 255 / 255f, 255 / 255f, 100 / 255f);   //���õ��� ���� ��ų ��ư ����+����


    /// <summary>
    /// ������ ���� ��ü
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
    /// ������ �� Ŭ��
    /// </summary>
    public void Clicked()
    {
        OnSelect(); //�׼� ȣ��
    }
}