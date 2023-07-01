using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivateSkillButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SkillName;    //��ų��
    [SerializeField] private TextMeshProUGUI UseMP; //�Ҹ� ����
    [SerializeField] private Image Skillicon; //��ų ������

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


    //private SkillCellData ;  //���� ���õ� ��ų


    public void UpdateActivateSkillData(SkillCellData NowSelectedSkill)
    {
        SkillName.text = NowSelectedSkill.ReturnSkillDataRec().ReturnName();       //�����ؾ� �� ��ų �̸����� �̸� ����
        UseMP.text = NowSelectedSkill.ReturnSkillDataRec().ReturnUseMP().ToString() + "MP";        //�����ؾ� �� ��ų�� �Ҹ��ϴ� ���������� ����

        //�����ؾ� �� ��ų ���������� ��������Ʈ ����
        switch (NowSelectedSkill.ReturnSkillDataRec().ReturnSkillType())
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
    }

    /// <summary>
    /// ���õ� ��ų ��ư �ѱ�
    /// </summary>
    public void ShowActivateSkillButton()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ���õ� ��ų ��ư ����
    /// </summary>
    public void HideActivateSkillButton()
    {
        gameObject.SetActive(false);
    }
}
