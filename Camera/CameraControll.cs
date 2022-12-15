using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControll : MonoBehaviour
{
    private CinemachineFreeLook FreeLookComponent;

    private bool Alt;

    private float MouseScroll;
    private float CameraXSpeed;
    private float CameraYSpeed;
    private float CameraMinFOV = 50;
    private float CameraMaxFOV = 75;
    [SerializeField] private float ZoomSpeed;

    private void Awake()
    {
        FreeLookComponent = gameObject.GetComponent<CinemachineFreeLook>();
        CameraXSpeed = FreeLookComponent.m_XAxis.m_MaxSpeed;
        CameraYSpeed = FreeLookComponent.m_YAxis.m_MaxSpeed;
    }
    private void Update()
    {
        Alt = Input.GetKey(KeyCode.LeftAlt);
        MouseScroll = Input.GetAxis("Mouse ScrollWheel");

        FreeLookComponent.m_XAxis.m_MaxSpeed = (Alt ? 0.0f : CameraXSpeed);    //ALTŰ�� ������ ���� ���� UI ����
        FreeLookComponent.m_YAxis.m_MaxSpeed = (Alt ? 0.0f : CameraYSpeed);    //ALTŰ�� ������ ���� ���� UI ����


        //float fov = FreeLookComponent.m_Lens.FieldOfView;
        //fov -= (Alt ? 0.0f : ZoomSpeed) * MouseScroll;
        //FreeLookComponent.m_Lens.FieldOfView = Mathf.Clamp(fov, CameraMinFOV, CameraMaxFOV);

        if (MouseScroll > 0f && FreeLookComponent.m_Lens.FieldOfView > CameraMinFOV)
        {
            //���콺 ��ũ�Ѿ��� �����Ǿ��� ���� �� ���°� �ִ� ���� ��ġ���� Ŭ ��� ���� - ALTŰ�� ������ ���� ���� X 
            FreeLookComponent.m_Lens.FieldOfView -= (Alt ? 0.0f : ZoomSpeed);
        }
        else if (MouseScroll < 0f && FreeLookComponent.m_Lens.FieldOfView < CameraMaxFOV)
        {
            //���콺 ��ũ�Ѵٿ��� �����Ǿ��� ���� �� ���°� �ִ� �ܾƿ� ��ġ���� ���� ��� �ܾƿ� - ALTŰ�� ������ ���� ���� X 
            FreeLookComponent.m_Lens.FieldOfView += (Alt ? 0.0f : ZoomSpeed);
        }
    }
}
