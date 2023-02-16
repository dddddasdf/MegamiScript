using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(RectTransform))]
public class UIRecycableViewController<T> : MonoBehaviour
{
    protected List<T> TableData = new List<T>();    //����Ʈ �׸��� �����͸� ����
    [SerializeField] protected GameObject CellBase = null;  //������ ���� ��
    [SerializeField] private RectOffset Padding;    //��ũ���� ������ �е�
    [SerializeField] private float SpacingHeight = 10f; //�� ���� ����
    [SerializeField] private RectOffset VisibleRectPadding = null;  //VisibleRect�� �е�

    private LinkedList<UIRecycableViewController<T>> Cells = new LinkedList<UIRecycableViewController<T>>();    //�� ���� ����Ʈ

    private Vector2 PrevScrollPostion;  //�ٷ� ���� ��ũ�� ��ġ�� ����

    [SerializeField] Scrollbar Test;
    ScrollRect Test2;

    public RectTransform CachedRectTransform => GetComponent<RectTransform>();  //RectTransform ĳ�̿�

}





/*
���� ��ũ�Ѻ�� 10�� ������ �����̸� ���� �κ��丮���� ���� ������ �� ���� ����� �� �Դ� ���ҽ��� ����(500�� ������� ���忡 ���� ������ �� �ְ�����, ������ �� ���� ���� ���� �ҷ����δٸ� ���ϰ� Ŭ ���ۿ� ����)
���� ������Ʈ Ǯ���� �̿��Ͽ� ��ũ�� �� ���� ��Ȱ�� �ϴ� ������� ���ϸ� �� ����� ã�ƺ��Ҵ�.

������ �ҽ��ڵ�
https://wonjuri.tistory.com/entry/Unity-UI-%EC%9E%AC%EC%82%AC%EC%9A%A9-%EC%8A%A4%ED%81%AC%EB%A1%A4%EB%B7%B0-%EC%A0%9C%EC%9E%91
*/