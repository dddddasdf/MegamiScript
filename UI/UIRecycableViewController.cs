using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(RectTransform))]
public class UIRecycableViewController<T> : MonoBehaviour
{
    protected List<T> TableData = new List<T>();    //리스트 항목의 데이터를 저장
    [SerializeField] protected GameObject CellBase = null;  //참조할 원본 셀
    [SerializeField] private RectOffset Padding;    //스크롤할 내용의 패딩
    [SerializeField] private float SpacingHeight = 10f; //각 셀의 간격
    [SerializeField] private RectOffset VisibleRectPadding = null;  //VisibleRect의 패딩

    private LinkedList<UIRecycableViewController<T>> Cells = new LinkedList<UIRecycableViewController<T>>();    //셀 저장 리스트

    private Vector2 PrevScrollPostion;  //바로 전의 스크롤 위치를 저장

    [SerializeField] Scrollbar Test;
    ScrollRect Test2;

    public RectTransform CachedRectTransform => GetComponent<RectTransform>();  //RectTransform 캐싱용

}





/*
기존 스크롤뷰는 10개 정도의 내용이면 몰라도 인벤토리같이 많은 내용을 한 번에 출력할 땐 먹는 리소스가 많다(500개 정도라면 스펙에 따라 감당할 수 있겠지만, 정말로 한 번에 많은 양을 불러들인다면 부하가 클 수밖에 없다)
따라서 오브젝트 풀링을 이용하여 스크롤 할 셀을 재활용 하는 방식으로 부하를 덜 방법을 찾아보았다.

참고한 소스코드
https://wonjuri.tistory.com/entry/Unity-UI-%EC%9E%AC%EC%82%AC%EC%9A%A9-%EC%8A%A4%ED%81%AC%EB%A1%A4%EB%B7%B0-%EC%A0%9C%EC%9E%91
*/