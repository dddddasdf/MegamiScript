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
    [SerializeField] private Transform thisTransform;   //������ ������
    [SerializeField] private Image EnemyVisualSprite;  //���� ���־� ��������Ʈ
    [SerializeField] private Image EnemyMark;    //�� ��ũ ǥ�� �̹���

    [SerializeField] private Canvas InformationUICanvas;
    [SerializeField] private TextMeshProUGUI LevelTMP;
    [SerializeField] private TextMeshProUGUI TribeTMP;
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPTMP;
    [SerializeField] private Slider HPBar;


    private OnBattleEnemyObject thisCellEnemyData;

    //private CancellationTokenSource StopTaskToken = new CancellationTokenSource();      //��ũ �����̰� �ϴ� UniTask �ߴܿ� ��ū

    //�Ʒ��� ��ũ ǥ�� ���� ��������Ʈ
    [SerializeField] private Sprite[] MarkWeak = new Sprite[2];
    [SerializeField] private Sprite[] MarkVoid = new Sprite[2];
    [SerializeField] private Sprite[] MarkResist = new Sprite[2];
    [SerializeField] private Sprite[] MarkReflect = new Sprite[2];
    [SerializeField] private Sprite[] MarkDrain = new Sprite[2];
    [SerializeField] private Sprite[] MarkUnknown = new Sprite[2];
    [SerializeField] private Sprite MarkScout;

    private StringBuilder SpriteNameSB = new StringBuilder(20); //��������Ʈ��� StringBuilder
    
    private bool TaskWorkManage = false;        //UniTask ���� Ż��� ����

    private void Awake()
    {
        

    }

    private void OnDisable()
    {

        
    }


    public void SetEnemyCell(OnBattleEnemyObject EnemyData)
    {
        //ǥ���ؾ� �� ��...
        //���� �ճ� �ٵ�

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

            //��巹������ �޸𸮿� ������ �ʵ��� ����� ����
            //Addressables.Release(ImageHandle);
        };
    }

    /// <summary>
    /// �̸�, ����, ����, ü�� ���� ǥ��
    /// </summary>
    public void SetInformation()
    {
        //���� �̸� ���� ����
        //���� ü�� ���� ����
        //���� ���� ���� ����
        //���� ���� ���� ����
    }

    /// <summary>
    /// ���Ͱ� Ÿ���� �Ǿ��� �� ���� ���� ����ֱ�
    /// </summary>
    public void ShowInformation()
    {
        InformationUICanvas.enabled = true;
    }

    /// <summary>
    /// ���� ���߱�
    /// </summary>
    public void HideInformation()
    {
        InformationUICanvas.enabled = false;
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
                //�� ����
                EnemyMark.enabled = false;
                TaskWorkManage = false;
                break;
        }
    }

    /// <summary>
    /// ��ũ ǥ�� ����
    /// </summary>
    public void TurnOffAffinityMark()
    {
        TaskWorkManage = false;
        EnemyMark.enabled = false;
    }


    /// <summary>
    /// �ʿ��� ��ũ�� �����Ÿ��� �ϴ� �޼ҵ�
    /// </summary>
    /// <param name="SwapMark"></param>
    /// <returns></returns>
    private async UniTaskVoid BlinkMark(Sprite[] SwapMark)
    {
        CancellationTokenSource StopTaskToken = new CancellationTokenSource();      //��ũ �����̰� �ϴ� UniTask �ߴܿ� ��ū

        int i = 0;      //�迭 �ε��� �����

        while (TaskWorkManage)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: StopTaskToken.Token);   //1�ʸ��� ��ũ ��ü
            i = (i + 1) % 2;
            EnemyMark.sprite = SwapMark[i];
        }

        StopTaskToken.Cancel();
        StopTaskToken.Dispose();
    }
}
