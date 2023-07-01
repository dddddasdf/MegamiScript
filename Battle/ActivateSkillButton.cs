using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActivateSkillButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SkillName;    //스킬명
    [SerializeField] private TextMeshProUGUI UseMP; //소모 마나
    [SerializeField] private Image Skillicon; //스킬 아이콘

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


    //private SkillCellData ;  //현재 선택된 스킬


    public void UpdateActivateSkillData(SkillCellData NowSelectedSkill)
    {
        SkillName.text = NowSelectedSkill.ReturnSkillDataRec().ReturnName();       //갱신해야 할 스킬 이름으로 이름 변경
        UseMP.text = NowSelectedSkill.ReturnSkillDataRec().ReturnUseMP().ToString() + "MP";        //갱신해야 할 스킬이 소모하는 마나량으로 변경

        //갱신해야 할 스킬 아이콘으로 스프라이트 변경
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
    /// 선택된 스킬 버튼 켜기
    /// </summary>
    public void ShowActivateSkillButton()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 선택된 스킬 버튼 끄기
    /// </summary>
    public void HideActivateSkillButton()
    {
        gameObject.SetActive(false);
    }
}
