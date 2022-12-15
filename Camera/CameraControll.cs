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

        FreeLookComponent.m_XAxis.m_MaxSpeed = (Alt ? 0.0f : CameraXSpeed);    //ALT키를 누르고 있을 때는 UI 조작
        FreeLookComponent.m_YAxis.m_MaxSpeed = (Alt ? 0.0f : CameraYSpeed);    //ALT키를 누르고 있을 때는 UI 조작


        //float fov = FreeLookComponent.m_Lens.FieldOfView;
        //fov -= (Alt ? 0.0f : ZoomSpeed) * MouseScroll;
        //FreeLookComponent.m_Lens.FieldOfView = Mathf.Clamp(fov, CameraMinFOV, CameraMaxFOV);

        if (MouseScroll > 0f && FreeLookComponent.m_Lens.FieldOfView > CameraMinFOV)
        {
            //마우스 스크롤업이 감지되었고 현재 줌 상태가 최대 줌인 수치보다 클 경우 줌인 - ALT키를 누르고 있을 때는 X 
            FreeLookComponent.m_Lens.FieldOfView -= (Alt ? 0.0f : ZoomSpeed);
        }
        else if (MouseScroll < 0f && FreeLookComponent.m_Lens.FieldOfView < CameraMaxFOV)
        {
            //마우스 스크롤다운이 감지되었고 현재 줌 상태가 최대 줌아웃 수치보다 작을 경우 줌아웃 - ALT키를 누르고 있을 때는 X 
            FreeLookComponent.m_Lens.FieldOfView += (Alt ? 0.0f : ZoomSpeed);
        }
    }
}
