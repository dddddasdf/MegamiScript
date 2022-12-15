using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Transform Pivot;
    [SerializeField] private Transform RightHand;   //ĳ������ ������ ��ġ
    private Vector3 WaistPosition;   //ĳ������ �㸮�� �ִ� ���� ��ġ
    private Transform SwordPosition;    //���� ��ġ
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
    /// ���ݽ� Į ������ Ȱ��ȭ
    /// </summary>
    public void ShowSword()
    {
        SwordRenderer.enabled = true;
    }

    /// <summary>
    /// ���� ����� ������ Į �������� ��Ȱ��ȭ
    /// </summary>
    public void HideSword()
    {
        SwordRenderer.enabled = false;
    }

    public void SetBackSword()
    {
        gameObject.transform.Translate(WaistPosition);  //���� ����� ������ Į ����ġ�� ����
    }
}
