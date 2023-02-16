using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class SimpleScrollViewConroller : MonoBehaviour
{
    [SerializeField] private Transform ContentTransform;
    [SerializeField] private Scrollbar ContentScrollbar;

    private Text[] Texts;

    private int Current;
    private int Final;
    private int Length;
    private int MaxSize;   //아이템 UI 전체 관리자가 Awake 단계에서 사이즈 전달 필요

    public int MaxNumber => MaxSize;

    public int CreatedNumber { get; private set; }

    public bool NoNeedScrollbar => CreatedNumber == MaxSize;

    // Start is called before the first frame update
    void Start()
    {
        MaxSize = 50;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 출력해야 할 아이템 개수 출력
    /// </summary>
    /// <param name="Size"></param>
    public void GetSize(int Size)
    {
        this.MaxSize = Size;
    }
}
