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

public class EnemyCell : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;   //사이즈 조절용
    [SerializeField] private Image EnemyVisualSprite;  //적의 비주얼 스프라이트
    [SerializeField] private Image EnemyMark;    //상성 마크 표시 이미지
    [SerializeField] private TextMeshProUGUI LevelTMP;
    [SerializeField] private TextMeshProUGUI TribeTMP;
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPTMP;
    [SerializeField] private Slider HPBar;

    private StringBuilder SpriteNameSB = new StringBuilder(20); //스프라이트명용 StringBuilder

    private OnBattleEnemyObject thisCellEnemyData;

    //private CancellationTokenSource StopTaskToken = new CancellationTokenSource();      //마크 깜빡이게 하는 UniTask 중단용 토큰

    //아래로 마크 표시 관련 스프라이트
    [SerializeField] private Sprite[] MarkWeak = new Sprite[2];
    [SerializeField] private Sprite[] MarkVoid = new Sprite[2];
    [SerializeField] private Sprite[] MarkResist = new Sprite[2];
    [SerializeField] private Sprite[] MarkReflect = new Sprite[2];
    [SerializeField] private Sprite[] MarkDrain = new Sprite[2];
    [SerializeField] private Sprite[] MarkUnknown = new Sprite[2];
    [SerializeField] private Sprite MarkScout;

    private bool Test = false;
    private bool TaskWorkManage = false;

    private void Awake()
    {
        

    }

    private void OnDisable()
    {

        
    }


    public void SetEnemyCell(OnBattleEnemyObject EnemyData)
    {
        //표시해야 할 거...
        //뭐가 잇냐 근데

        thisCellEnemyData = EnemyData;

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
    public void ShowInformation()
    {
        //여기 이름 설정 들어가고
        //여기 체력 설정 들어가고
        //여기 종족 설정 들어가고
        //여기 레벨 설정 들어가고
    }

    /// <summary>
    /// 정보 감추기
    /// </summary>
    public void HideInformation()
    {
        //대충 정보 오브젝트들 모아둔 게임오브젝트 감춘단 내용 or TMP의 enable을 false로 바꿔서 출력을 중단한다거나.
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetMarkSpriteArray()
    {

    }

    public void GetEnemyData()
    {

    }


    private void SetSpriteImage()
    {

    }

    public void SetAffinitiesMark()
    {

    }

    public void TurnOnAffinityMark(SkillDataRec TargetSkill)
    {    
        switch (thisCellEnemyData.ReturnAffinity(TargetSkill.ReturnSkillType()))
        {
            case SkillAffinities.Weak:
                TaskWorkManage = true;
                BlinkMark(MarkWeak).Forget();
                EnemyMark.sprite = MarkWeak[0];
                EnemyMark.enabled = true;
                break;
            case SkillAffinities.Resist:
                TaskWorkManage = true;
                BlinkMark(MarkResist).Forget();
                EnemyMark.sprite = MarkResist[0];
                EnemyMark.enabled = true;
                break;
            case SkillAffinities.Void:
                TaskWorkManage = true;
                BlinkMark(MarkVoid).Forget();
                EnemyMark.sprite = MarkVoid[0];
                EnemyMark.enabled = true;
                break;
            case SkillAffinities.Reflect:
                TaskWorkManage = true;
                BlinkMark(MarkReflect).Forget();
                EnemyMark.sprite = MarkReflect[0];
                EnemyMark.enabled = true;
                break;
            case SkillAffinities.Drain:
                TaskWorkManage = true;
                BlinkMark(MarkDrain).Forget();
                EnemyMark.sprite = MarkDrain[0];
                EnemyMark.enabled = true;
                break;
            default:
                //상성 없음
                EnemyMark.enabled = false;
                TaskWorkManage = false;
                break;
        }
    }

    /// <summary>
    /// 마크 표시 종료
    /// </summary>
    public void TurnOffAffinityMark()
    {
        TaskWorkManage = false;
        EnemyMark.enabled = false;
    }

    public void Test_TurnOnMark()
    {
        Test = !Test;
        if (Test)
        {
            EnemyMark.sprite = MarkWeak[0];
            EnemyMark.enabled = true;
            BlinkMark(MarkWeak).Forget();
        }
        else
            EnemyMark.enabled = false;
    }

    /// <summary>
    /// 필요한 마크를 깜빡거리게 하는 메소드
    /// </summary>
    /// <param name="SwapMark"></param>
    /// <returns></returns>
    private async UniTaskVoid BlinkMark(Sprite[] SwapMark)
    {
        CancellationTokenSource StopTaskToken = new CancellationTokenSource();      //마크 깜빡이게 하는 UniTask 중단용 토큰
        int i = 0;

        while (TaskWorkManage)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: StopTaskToken.Token);   //1초마다 마크 교체
            i = (i + 1) % 2;
            EnemyMark.sprite = SwapMark[i];
        }

        StopTaskToken.Cancel();
        StopTaskToken.Dispose();
    }
}
