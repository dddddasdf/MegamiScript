using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Text;
using DG.Tweening;

public class EnemyCell : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;   //사이즈 조절용
    [SerializeField] private Image EnemyVisualSprite;  //적의 비주얼 스프라이트
    [SerializeField] private Image EnemyMark;    //상성 마크 표시 이미지

    [SerializeField] private Canvas InformationUICanvas;
    [SerializeField] private TextMeshProUGUI LevelTMP;
    [SerializeField] private TextMeshProUGUI TribeTMP;
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPTMP;
    [SerializeField] private Slider HPBar;


    private OnBattleEnemyObject thisCellEnemyData;      //이 에너미 셀이 가지게 되는 적의 정보

    private CancellationTokenSource StopTaskToken;      //마크 깜빡이게 하는 UniTask 중단용 토큰

    //아래로 마크 표시 관련 스프라이트
    [SerializeField] private Sprite[] MarkWeak = new Sprite[2];
    [SerializeField] private Sprite[] MarkVoid = new Sprite[2];
    [SerializeField] private Sprite[] MarkResist = new Sprite[2];
    [SerializeField] private Sprite[] MarkReflect = new Sprite[2];
    [SerializeField] private Sprite[] MarkDrain = new Sprite[2];
    [SerializeField] private Sprite[] MarkUnknown = new Sprite[2];
    [SerializeField] private Sprite MarkScout;

    private StringBuilder SpriteNameSB = new StringBuilder(20); //스프라이트명용 StringBuilder
    
    private void OnDisable()
    {
        //if (!StopTaskToken.IsCancellationRequested)
        //{
        //    StopTaskToken.Cancel();
        //    StopTaskToken.Dispose();
        //}
    }


    public void SetEnemyCell(OnBattleEnemyObject EnemyData)
    {
        //표시해야 할 거...
        //뭐가 잇냐 근데

        thisCellEnemyData = EnemyData;

        SetInformation();

        SpriteNameSB.Clear();
        SpriteNameSB.Append("Demon").Append(thisCellEnemyData.ReturnID());

        AsyncOperationHandle<Sprite> ImageHandle;
        ImageHandle = Addressables.LoadAssetAsync<Sprite>(SpriteNameSB.ToString());
        ImageHandle.Completed += Handle =>
        {

            thisTransform.transform.localScale = (ImageHandle.Result.bounds.size) * 2;
            EnemyVisualSprite.sprite = ImageHandle.Result;
            EnemyVisualSprite.enabled = true;

            //어드레서블이 메모리에 쌓이지 않도록 명시적 해제
            //Addressables.Release(ImageHandle);
        };
    }

    /// <summary>
    /// 이름, 종족, 레벨, 체력 정보 표시
    /// </summary>
    private void SetInformation()
    {
        NameTMP.text = thisCellEnemyData.ReturnName();
        HPTMP.text = thisCellEnemyData.ReturnHP().ToString();
        TribeTMP.text = thisCellEnemyData.ReturnTribe();
        LevelTMP.text = thisCellEnemyData.ReturnLevel().ToString();
    }


    /// <summary>
    /// 타격 맞고 HP 정보 갱신하는 건데 옵저버로 바꿔야 할 필요가 있을까 고찰해야 한다
    /// </summary>
    public void UpdateHP()
    {
        HPTMP.text = thisCellEnemyData.ReturnHP().ToString();
    }

    /// <summary>
    /// 몬스터가 타겟팅 되었을 때 세부 정보 띄워주기
    /// </summary>
    public void ShowInformation()
    {
        InformationUICanvas.enabled = true;
    }

    /// <summary>
    /// 세부 정보 감추기
    /// </summary>
    public void HideInformation()
    {
        InformationUICanvas.enabled = false;
    }


    /// <summary>
    /// 깜박거려야 하는 마크를 설정 후 작업 시작
    /// </summary>
    /// <param name="TargetSkill"></param>
    public void TurnOnAffinityMark(SkillDataRec TargetSkill)
    {
        if (thisCellEnemyData.ReturnHP() <= 0)
            return;         //적이 죽은 상태면 표시X
        
        switch (thisCellEnemyData.ReturnAffinity(TargetSkill.ReturnSkillType()))
        {
            case SkillAffinities.Weak:
                MarkTaskActivate(MarkWeak);
                break;
            case SkillAffinities.Resist:
                MarkTaskActivate(MarkResist);
                break;
            case SkillAffinities.Void:
                MarkTaskActivate(MarkVoid);
                break;
            case SkillAffinities.Reflect:
                MarkTaskActivate(MarkReflect);
                break;
            case SkillAffinities.Drain:
                MarkTaskActivate(MarkDrain);
                break;
            default:
                //상성 없음
                EnemyMark.enabled = false;
                break;
        }
    }

    /// <summary>
    /// 마크를 깜빡이게 하는 Task 켜기
    /// </summary>
    private void MarkTaskActivate(Sprite[] SwapMark)
    {
        MarkTaskCancel();
        //Task 작동 전에 캔슬 요청을 한 번 더 하는 이유: 마크 깜빡임을 켜기 전에 한 번 더 토큰 확인 후 꺼져 있지 않으면 꺼주는 작업을 한다(안 하면 Task가 무한정 쌓일 수 있다)


        BlinkMark(SwapMark).Forget();
        EnemyMark.sprite = SwapMark[0];
        EnemyMark.enabled = true;
    }

    /// <summary>
    /// 마크에 관여하는 Task 캔슬 요청
    /// </summary>
    private void MarkTaskCancel()
    {
        //StopTaskToken이 null일 경우: 할 필요가 없으니까 넘긴다
        //StopTaskToken.IsCancellationRequested = true일 경우 해당 토큰은 이미 취소요청이 발행된 이후로 새로 발급받은 적이 없기에 작동하지 않고 있다→중단 요청을 할 필요가 없다(하면 오히려 에러난다)
        if (StopTaskToken != null && !StopTaskToken.IsCancellationRequested)
        {
            StopTaskToken.Cancel();
            StopTaskToken.Dispose();
        }
    }

    /// <summary>
    /// 마크 표시 종료
    /// </summary>
    public void TurnOffAffinityMark()
    {
        EnemyMark.enabled = false;
        MarkTaskCancel();
    }


    /// <summary>
    /// 필요한 마크를 깜빡거리게 하는 메소드
    /// </summary>
    /// <param name="SwapMark"></param>
    /// <returns></returns>
    private async UniTaskVoid BlinkMark(Sprite[] SwapMark)
    {
        StopTaskToken = new CancellationTokenSource();      //마크 깜빡이게 하는 UniTask 중단용 토큰
        int i = 0;      //배열 인덱스 변경용

        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: StopTaskToken.Token);   //1초마다 마크 교체
            i = (i + 1) % 2;
            EnemyMark.sprite = SwapMark[i];
        }
    }

    public void GetDamaged()
    {

    }

    public void Click()
    {
        BattleUIManager.ClickedEnemyCell = this;
    }
}
