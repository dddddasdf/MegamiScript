using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerInput : MonoBehaviour
{
    //A, D 이동
    public float MoveX { get; private set; }
    //W, S 이동
    public float MoveZ { get; private set; }

    /// <summary>
    /// 마우스 좌클릭-필드 공격
    /// </summary>
    public bool Attack { get; private set; }

    private bool Alt;

    private void Update()
    {
        Alt = Input.GetKey(KeyCode.LeftAlt);
        Attack = Input.GetMouseButtonDown(0) & !Alt;   //ALT키를 누르고 있을 때는 UI 조작

        MoveX = Input.GetAxis("Horizontal");
        MoveZ = Input.GetAxis("Vertical");
    }
}
