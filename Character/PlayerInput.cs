using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerInput : MonoBehaviour
{
    //A, D �̵�
    public float MoveX { get; private set; }
    //W, S �̵�
    public float MoveZ { get; private set; }

    /// <summary>
    /// ���콺 ��Ŭ��-�ʵ� ����
    /// </summary>
    public bool Attack { get; private set; }

    private bool Alt;

    private void Update()
    {
        Alt = Input.GetKey(KeyCode.LeftAlt);
        Attack = Input.GetMouseButtonDown(0) & !Alt;   //ALTŰ�� ������ ���� ���� UI ����

        MoveX = Input.GetAxis("Horizontal");
        MoveZ = Input.GetAxis("Vertical");
    }
}
