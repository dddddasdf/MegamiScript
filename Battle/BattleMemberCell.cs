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

    [SerializeField] private GameObject MemberInfo;  //빈 멤버가 아닐 경우 출력하는 정보값들의 집합
    [SerializeField] private Image Frame;  //파티 프레임
    [SerializeField] private Slider HPBar;  //체력 바 
    [SerializeField] private Slider MPBar;  //마나 바
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPTmp;
    [SerializeField] private TextMeshProUGUI MPTmp;
    [SerializeField] private GameObject EmptyText; //빈 멤버일 경우 비어있음 표기

    [SerializeField] private Image TurnBox;         //행동순서 숫자가 표시되는 박스
    [SerializeField] private TextMeshProUGUI TextNumberOfTurn;      //현재 배정받은 행동순서 숫자
    [SerializeField] private Image NotNowTurnScreen;        //자신의 행동턴이 아닐 때는 회색으로 처리
    [SerializeField] private Image DeadScreen;          //주인공(플레이어) 캐릭터 한정으로 사망했을 때 씌우는 검은색 스크린

    private Color SelectedButtonColor = new Color(51 / 255f, 51 / 255f, 51 / 255f, 255 / 255f); //1사이클 동안 행동 완료했을 경우 행동 순서 상자는 검은색 계열
    private Color UnselectedButtonColor = new Color(188 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);   //아직 1사이클 동안 행동하지 않았을 때 행동 순서 상자는 붉은색

    [SerializeField] private Image Portrait;        //초상화

    private AsyncOperationHandle<Sprite> PortraitSpriteAssetHandle;
    private StringBuilder PortraitNameSB = new StringBuilder(20);       //포트레이트명용 StringBuilder

    private OnBattleObject thisCellMemberData;

    #endregion

    #region DataField


    #endregion

    private void Awake()
    {

    }

    private void OnDisable()
    {
        
        //Addressables.Release(PortraitSpriteAssetHandle);        //어드레서블 해제
    }

#nullable enable
    /// <summary>
    /// 정보 받아와서 UI에 출력시키기
    /// </summary>
    /// <param name="MemberData"></param>
    public void SetMemberCell(OnBattleObject? MemberData)
    {
        if (MemberData == null)
        {
            //받은 멤버가 비어있을(null) 경우 비어있음 출력
            SetEmptyText();
        }
        else
        {
            thisCellMemberData = MemberData;
            PartyMemberData Tmp = thisCellMemberData.ReturnMemberData();    //임시 변수
            NameTMP.text = Tmp.ReturnName();      //이름 출력
            HPTmp.text = Tmp.ReturnNowHP().ToString();      //현재 체력 출력
            MPTmp.text = Tmp.ReturnNowMP().ToString();      //현재 마나 출력
            HPBar.value = ((float)Tmp.ReturnNowHP() / (float)Tmp.ReturnMaxHP()) * 100f;
            MPBar.value = ((float)Tmp.ReturnNowMP() / (float)Tmp.ReturnMaxMP()) * 100f;

            //파티원에 따라 받아올 초상화 변수명 결정
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
    /// 파티 멤버 교체 or 이탈, 이 함수는 동료 악마들에게만 사용한다 (주인공은 사망해도 파티에서 이탈하지 않으며, 동료악마와 교체할 수 없다)
    /// </summary>
    /// <param name="MemberData"></param>
    public void SwapMemberData(OnBattleObject? MemberData)
    {
        if (MemberData == null)
        {
            //받은 멤버가 비어있을(null) 경우 비어있음 출력
            SetEmptyText();
        }
        else
        {
            thisCellMemberData = MemberData;
            PartyMemberData Tmp = thisCellMemberData.ReturnMemberData();    //임시 변수
            NameTMP.text = Tmp.ReturnName();      //이름 출력
            HPTmp.text = Tmp.ReturnNowHP().ToString();      //현재 체력 출력
            MPTmp.text = Tmp.ReturnNowMP().ToString();      //현재 마나 출력
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
    /// 비어있는 멤버인지 확인
    /// </summary>
    /// <param name="IsEmpty"> 비어있으면 true</param>
    private void SetEmptyText()
    {
        MemberInfo.SetActive(false);
        EmptyText.SetActive(true);
    }

    /// <summary>
    /// 현재 체력 표기 갱신
    /// </summary>
    public void RefreshHP()
    {
        HPTmp.text = thisCellMemberData.ReturnMemberData().ReturnNowHP().ToString();
        HPBar.value = ((float)thisCellMemberData.ReturnMemberData().ReturnNowHP() / (float)thisCellMemberData.ReturnMemberData().ReturnMaxHP()) * 100f;
    }

    /// <summary>
    /// 현재 마나 표기 갱신
    /// </summary>
    public void RefreshMP()
    {
        MPTmp.text = thisCellMemberData.ReturnMemberData().ReturnNowMP().ToString();
        MPBar.value = ((float)thisCellMemberData.ReturnMemberData().ReturnNowMP() / (float)thisCellMemberData.ReturnMemberData().ReturnMaxMP()) * 100f;
    }

    /// <summary>
    /// 현재 조작턴일 경우 프레임을 녹색으로 변경
    /// </summary>
    public void NowTurn()
    {
        Frame.color = Color.green;
    }

    /// <summary>
    /// 조작턴이 종료되면 프레임을 흰색으로 변경
    /// </summary>
    public void EndTurn()
    {
        Frame.color = Color.white;
    }
}
