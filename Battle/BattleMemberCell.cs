using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class BattleMemberCell : MonoBehaviour
{
    #region SetVariables

    [SerializeField] private GameObject MemberInfo;  //�� ����� �ƴ� ��� ����ϴ� ���������� ����
    [SerializeField] private Image Frame;  //��Ƽ ������
    [SerializeField] private Slider HPBar;  //ü�� �� 
    [SerializeField] private Slider MPBar;  //���� ��
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPTmp;
    [SerializeField] private TextMeshProUGUI MPTmp;
    [SerializeField] private GameObject EmptyText; //�� ����� ��� ������� ǥ��

    [SerializeField] private Image TurnBox;         //�ൿ���� ���ڰ� ǥ�õǴ� �ڽ�
    [SerializeField] private TextMeshProUGUI TextNumberOfTurn;      //���� �������� �ൿ���� ����
    [SerializeField] private Image NotNowTurnScreen;        //�ڽ��� �ൿ���� �ƴ� ���� ȸ������ ó��
    [SerializeField] private Image DeadScreen;          //���ΰ�(�÷��̾�) ĳ���� �������� ������� �� ����� ������ ��ũ��

    private Color SelectedButtonColor = new Color(51 / 255f, 51 / 255f, 51 / 255f, 255 / 255f); //1����Ŭ ���� �ൿ �Ϸ����� ��� �ൿ ���� ���ڴ� ������ �迭
    private Color UnselectedButtonColor = new Color(188 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);   //���� 1����Ŭ ���� �ൿ���� �ʾ��� �� �ൿ ���� ���ڴ� ������

    [SerializeField] private Image Portrait;        //�ʻ�ȭ

    private AsyncOperationHandle<Sprite> PortraitSpriteAssetHandle;
    private StringBuilder PortraitNameSB = new StringBuilder(20);       //��Ʈ����Ʈ��� StringBuilder

    private OnBattleObject thisCellMemberData;

    #endregion

    #region DataField


    #endregion

    private void Awake()
    {

    }

    private void OnDisable()
    {
        
        //Addressables.Release(PortraitSpriteAssetHandle);        //��巹���� ����
    }

#nullable enable
    /// <summary>
    /// ���� �޾ƿͼ� UI�� ��½�Ű��
    /// </summary>
    /// <param name="MemberData"></param>
    public void SetMemberCell(OnBattleObject? MemberData)
    {
        if (MemberData == null)
        {
            //���� ����� �������(null) ��� ������� ���
            SetEmptyText();
        }
        else
        {
            thisCellMemberData = MemberData;
            PartyMemberData Tmp = thisCellMemberData.ReturnMemberData();    //�ӽ� ����
            NameTMP.text = Tmp.ReturnName();      //�̸� ���
            HPTmp.text = Tmp.ReturnNowHP().ToString();      //���� ü�� ���
            MPTmp.text = Tmp.ReturnNowMP().ToString();      //���� ���� ���
            HPBar.value = ((float)Tmp.ReturnNowHP() / (float)Tmp.ReturnMaxHP()) * 100f;
            MPBar.value = ((float)Tmp.ReturnNowMP() / (float)Tmp.ReturnMaxMP()) * 100f;

            //��Ƽ���� ���� �޾ƿ� �ʻ�ȭ ������ ����
            if (thisCellMemberData.ReturnIsPlayerCharacter())
            {
                PortraitNameSB.Clear();
                PortraitNameSB.Append("Flynn");
            }
            else
            {
                PartyDemonData TmpDemon = (PartyDemonData)Tmp;
                PortraitNameSB.Clear();
                PortraitNameSB.Append("PartyPortrait").Append(TmpDemon.ReturnID());
            }

            PortraitSpriteAssetHandle = Addressables.LoadAssetAsync<Sprite>(PortraitNameSB.ToString());

            PortraitSpriteAssetHandle.Completed += Handle =>
            {
                Portrait.sprite = PortraitSpriteAssetHandle.Result;
            };
        }
    }

    /// <summary>
    /// ��Ƽ ��� ��ü or ��Ż, �� �Լ��� ���� �Ǹ��鿡�Ը� ����Ѵ� (���ΰ��� ����ص� ��Ƽ���� ��Ż���� ������, ����Ǹ��� ��ü�� �� ����)
    /// </summary>
    /// <param name="MemberData"></param>
    public void SwapMemberData(OnBattleObject? MemberData)
    {
        if (MemberData == null)
        {
            //���� ����� �������(null) ��� ������� ���
            SetEmptyText();
        }
        else
        {
            thisCellMemberData = MemberData;
            PartyMemberData Tmp = thisCellMemberData.ReturnMemberData();    //�ӽ� ����
            NameTMP.text = Tmp.ReturnName();      //�̸� ���
            HPTmp.text = Tmp.ReturnNowHP().ToString();      //���� ü�� ���
            MPTmp.text = Tmp.ReturnNowMP().ToString();      //���� ���� ���
            HPBar.value = ((float)Tmp.ReturnNowHP() / (float)Tmp.ReturnMaxHP()) * 100f;
            MPBar.value = ((float)Tmp.ReturnNowMP() / (float)Tmp.ReturnMaxMP()) * 100f;

            PartyDemonData TmpDemon = (PartyDemonData)Tmp;
            PortraitNameSB.Clear();
            PortraitNameSB.Append("PartyPortrait").Append(TmpDemon.ReturnID());

            PortraitSpriteAssetHandle = Addressables.LoadAssetAsync<Sprite>(PortraitNameSB);

            PortraitSpriteAssetHandle.Completed += Handle =>
            {
                Portrait.sprite = PortraitSpriteAssetHandle.Result;
            };
        }
    }
#nullable disable

    /// <summary>
    /// ����ִ� ������� Ȯ��
    /// </summary>
    /// <param name="IsEmpty"> ��������� true</param>
    private void SetEmptyText()
    {
        MemberInfo.SetActive(false);
        EmptyText.SetActive(true);
    }

    /// <summary>
    /// ���� ü�� ǥ�� ����
    /// </summary>
    public void RefreshHP()
    {
        HPTmp.text = thisCellMemberData.ReturnMemberData().ReturnNowHP().ToString();
        HPBar.value = ((float)thisCellMemberData.ReturnMemberData().ReturnNowHP() / (float)thisCellMemberData.ReturnMemberData().ReturnMaxHP()) * 100f;
    }

    /// <summary>
    /// ���� ���� ǥ�� ����
    /// </summary>
    public void RefreshMP()
    {
        MPTmp.text = thisCellMemberData.ReturnMemberData().ReturnNowMP().ToString();
        MPBar.value = ((float)thisCellMemberData.ReturnMemberData().ReturnNowMP() / (float)thisCellMemberData.ReturnMemberData().ReturnMaxMP()) * 100f;
    }

    /// <summary>
    /// ���� �������� ��� �������� ������� ����
    /// </summary>
    public void NowTurn()
    {
        Frame.color = Color.green;
    }

    /// <summary>
    /// �������� ����Ǹ� �������� ������� ����
    /// </summary>
    public void EndTurn()
    {
        Frame.color = Color.white;
    }
}
