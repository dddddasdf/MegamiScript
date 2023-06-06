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
        SkillName.text = ThisCellData.ReturnSkillDataRec().ReturnName();       //�����ؾ� �� ��ų �̸����� �̸� ����
        UseMP.text = ThisCellData.ReturnSkillDataRec().ReturnUseMP().ToString() + "MP";        //�����ؾ� �� ��ų�� �Ҹ��ϴ� ���������� ����

        //�����ؾ� �� ��ų ���������� ��������Ʈ ����
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
    /// ������ �� Ŭ��
    /// </summary>
    public void Clicked()
    {
        OnSelect(); //�׼� ȣ��
    }
}
