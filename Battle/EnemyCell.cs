using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Cysharp.Threading.Tasks;

public class EnemyCellData
{
    public string Name { get; private set; }

    private int St;          //힘-칼 기본 공격 위력 상승, 물리 스킬 위력 소폭 상승
    private int Dx;          //기-총 기본 공격 위력 상승, 물리 스킬 위력 대폭 상승
    private int Ma;          //마-마법 스킬 위력 상승
    private int Ag;          //속-선제턴 확률, 행동순서, 명중률, 회피율 상승
    private int Lu;          //운-크리티컬, 회피율, 상태이상 성공률, 상태이상 회복속도, 아이템 드랍률 상승


    public void SetName(string Name)
    {
        this.Name = Name;
    }
}

public class EnemyCell : MonoBehaviour
{
    [SerializeField] private Transform thisTransform;
    [SerializeField] private Image EnemyVisualSprite;  //적의 비주얼 스프라이트
    [SerializeField] private Image EnemyAffinitiesMark;    //상성 표시

    [SerializeField] private Sprite[] MarkWeak = new Sprite[2];
    [SerializeField] private Sprite[] MarkVoid = new Sprite[2];
    [SerializeField] private Sprite[] MarkResist = new Sprite[2];
    [SerializeField] private Sprite[] MarkReflect = new Sprite[2];
    [SerializeField] private Sprite[] MarkDrain = new Sprite[2];
    [SerializeField] private Sprite[] MarkUnknown = new Sprite[2];
    [SerializeField] private Sprite MarkScout;

    private bool Test = false;

    private void Awake()
    {
        AsyncOperationHandle<Sprite> ImageHandle;
        ImageHandle = Addressables.LoadAssetAsync<Sprite>("TestSprite");
        ImageHandle.Completed += Handle =>
        {

            thisTransform.transform.localScale = (ImageHandle.Result.bounds.size) * 2;
            EnemyVisualSprite.sprite = ImageHandle.Result;
            EnemyVisualSprite.enabled = true;
            //어드레서블이 메모리에 쌓이지 않도록 명시적 해제
            //Addressables.Release(ImageHandle);
        };

    }

    private void OnDisable()
    {

        
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

    public void Test_TurnOnMark()
    {
        Test = !Test;
        if (Test)
        {
            EnemyAffinitiesMark.sprite = MarkWeak[0];
            EnemyAffinitiesMark.enabled = true;
            BlinkMark(MarkWeak).Forget();
        }
        else
            EnemyAffinitiesMark.enabled = false;
    }

    private async UniTaskVoid BlinkMark(Sprite[] SwapMark)
    {
        int i = 0;

        while (Test)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));   //1초마다 마크 교체
            i = (i + 1) % 2;
            EnemyAffinitiesMark.sprite = SwapMark[i];
        }
    }
}
