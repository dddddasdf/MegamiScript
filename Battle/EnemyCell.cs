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
    [SerializeField] private Transform thisTransform;   //������ ������
    [SerializeField] private Image EnemyVisualSprite;  //���� ���־� ��������Ʈ
    [SerializeField] private Image EnemyMark;    //�� ��ũ ǥ�� �̹���

    [SerializeField] private Canvas InformationUICanvas;
    [SerializeField] private TextMeshProUGUI LevelTMP;
    [SerializeField] private TextMeshProUGUI TribeTMP;
    [SerializeField] private TextMeshProUGUI NameTMP;
    [SerializeField] private TextMeshProUGUI HPTMP;
    [SerializeField] private Slider HPBar;


    private OnBattleEnemyObject thisCellEnemyData;      //�� ���ʹ� ���� ������ �Ǵ� ���� ����

    private CancellationTokenSource StopTaskToken;      //��ũ �����̰� �ϴ� UniTask �ߴܿ� ��ū

    //�Ʒ��� ��ũ ǥ�� ���� ��������Ʈ
    [SerializeField] private Sprite[] MarkWeak = new Sprite[2];
    [SerializeField] private Sprite[] MarkVoid = new Sprite[2];
    [SerializeField] private Sprite[] MarkResist = new Sprite[2];
    [SerializeField] private Sprite[] MarkReflect = new Sprite[2];
    [SerializeField] private Sprite[] MarkDrain = new Sprite[2];
    [SerializeField] private Sprite[] MarkUnknown = new Sprite[2];
    [SerializeField] private Sprite MarkScout;

    private StringBuilder SpriteNameSB = new StringBuilder(20); //��������Ʈ��� StringBuilder
    
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
        //ǥ���ؾ� �� ��...
        //���� �ճ� �ٵ�

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

            //��巹������ �޸𸮿� ������ �ʵ��� ����� ����
            //Addressables.Release(ImageHandle);
        };
    }

    /// <summary>
    /// �̸�, ����, ����, ü�� ���� ǥ��
    /// </summary>
    private void SetInformation()
    {
        NameTMP.text = thisCellEnemyData.ReturnName();
        HPTMP.text = thisCellEnemyData.ReturnHP().ToString();
        TribeTMP.text = thisCellEnemyData.ReturnTribe();
        LevelTMP.text = thisCellEnemyData.ReturnLevel().ToString();
    }


    /// <summary>
    /// Ÿ�� �°� HP ���� �����ϴ� �ǵ� �������� �ٲ�� �� �ʿ䰡 ������ �����ؾ� �Ѵ�
    /// </summary>
    public void UpdateHP()
    {
        HPTMP.text = thisCellEnemyData.ReturnHP().ToString();
    }

    /// <summary>
    /// ���Ͱ� Ÿ���� �Ǿ��� �� ���� ���� ����ֱ�
    /// </summary>
    public void ShowInformation()
    {
        InformationUICanvas.enabled = true;
    }

    /// <summary>
    /// ���� ���� ���߱�
    /// </summary>
    public void HideInformation()
    {
        InformationUICanvas.enabled = false;
    }


    /// <summary>
    /// ���ڰŷ��� �ϴ� ��ũ�� ���� �� �۾� ����
    /// </summary>
    /// <param name="TargetSkill"></param>
    public void TurnOnAffinityMark(SkillDataRec TargetSkill)
    {
        if (thisCellEnemyData.ReturnHP() <= 0)
            return;         //���� ���� ���¸� ǥ��X
        
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
                //�� ����
                EnemyMark.enabled = false;
                break;
        }
    }

    /// <summary>
    /// ��ũ�� �����̰� �ϴ� Task �ѱ�
    /// </summary>
    private void MarkTaskActivate(Sprite[] SwapMark)
    {
        MarkTaskCancel();
        //Task �۵� ���� ĵ�� ��û�� �� �� �� �ϴ� ����: ��ũ �������� �ѱ� ���� �� �� �� ��ū Ȯ�� �� ���� ���� ������ ���ִ� �۾��� �Ѵ�(�� �ϸ� Task�� ������ ���� �� �ִ�)


        BlinkMark(SwapMark).Forget();
        EnemyMark.sprite = SwapMark[0];
        EnemyMark.enabled = true;
    }

    /// <summary>
    /// ��ũ�� �����ϴ� Task ĵ�� ��û
    /// </summary>
    private void MarkTaskCancel()
    {
        //StopTaskToken�� null�� ���: �� �ʿ䰡 �����ϱ� �ѱ��
        //StopTaskToken.IsCancellationRequested = true�� ��� �ش� ��ū�� �̹� ��ҿ�û�� ����� ���ķ� ���� �߱޹��� ���� ���⿡ �۵����� �ʰ� �ִ١��ߴ� ��û�� �� �ʿ䰡 ����(�ϸ� ������ ��������)
        if (StopTaskToken != null && !StopTaskToken.IsCancellationRequested)
        {
            StopTaskToken.Cancel();
            StopTaskToken.Dispose();
        }
    }

    /// <summary>
    /// ��ũ ǥ�� ����
    /// </summary>
    public void TurnOffAffinityMark()
    {
        EnemyMark.enabled = false;
        MarkTaskCancel();
    }


    /// <summary>
    /// �ʿ��� ��ũ�� �����Ÿ��� �ϴ� �޼ҵ�
    /// </summary>
    /// <param name="SwapMark"></param>
    /// <returns></returns>
    private async UniTaskVoid BlinkMark(Sprite[] SwapMark)
    {
        StopTaskToken = new CancellationTokenSource();      //��ũ �����̰� �ϴ� UniTask �ߴܿ� ��ū
        int i = 0;      //�迭 �ε��� �����

        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: StopTaskToken.Token);   //1�ʸ��� ��ũ ��ü
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
