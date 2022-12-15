using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Transform Pivot;
    [SerializeField] private Transform RightHand;   //캐릭터의 오른손 위치
    private Vector3 WaistPosition;   //캐릭터의 허리에 있는 검의 위치
    private Transform SwordPosition;    //검의 위치
    private Renderer SwordRenderer;

    public Vector3 PivotVector { get { return (Pivot) ? Pivot.position : Vector3.zero; } set { if (Pivot) Pivot.position = value; } }
    public Vector3 RightHandPosition { get { return RightHand ? RightHand.position : Vector3.zero; } }
    public Quaternion RightHandRotation { get { return RightHand ? RightHand.rotation : Quaternion.identity; } }


    private void Awake()
    {
        Pivot = gameObject.transform;
        WaistPosition = gameObject.transform.position;
        SwordRenderer = gameObject.GetComponent<Renderer>();
    }

    /// <summary>
    /// 공격시 칼 렌더링 활성화
    /// </summary>
    public void ShowSword()
    {
        SwordRenderer.enabled = true;
    }

    /// <summary>
    /// 공격 모션이 끝나면 칼 렌더링을 비활성화
    /// </summary>
    public void HideSword()
    {
        SwordRenderer.enabled = false;
    }

    public void SetBackSword()
    {
        gameObject.transform.Translate(WaistPosition);  //공격 모션이 끝나면 칼 원위치로 수납
    }
}
