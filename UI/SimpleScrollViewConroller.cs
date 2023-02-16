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
    private int MaxSize;   //������ UI ��ü �����ڰ� Awake �ܰ迡�� ������ ���� �ʿ�

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
    /// ����ؾ� �� ������ ���� ���
    /// </summary>
    /// <param name="Size"></param>
    public void GetSize(int Size)
    {
        this.MaxSize = Size;
    }
}
