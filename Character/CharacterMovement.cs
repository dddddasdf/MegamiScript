using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterMovement : MonoBehaviour
{
    #region SetVariables
    [SerializeField] private float MovingSpeed = 8f; //ĳ���� �̵� �ӵ�
    [SerializeField] private float RotateSpeed = 0.4f; //ĳ���� ȸ�� �ӵ�
    [SerializeField] private Camera MainCamera; //���� ī�޶�

    private PlayerInput CharaInput;
    [SerializeField] private GameObject SwordObject;
    private Sword SwordScript;

    private Rigidbody CharaRigid;
    private Animator CharaAnimator;
    private Transform MainCameraTr;
    private Vector3 CameraLook; //ī�޶� �ٶ󺸴� ����
    private Vector3 MovingDirection; //�̵� ����
    private CheckAnimeState AnimeState; //�ִϸ��̼� �׼�
    private bool IsAttackAnime; //���� ��� �ߺ� ������
    private bool CanMove;   //������ �� �ִ��� ����

    #endregion

    private void Start()
    {
        CharaInput = GetComponent<PlayerInput>();
        SwordScript = SwordObject.GetComponent<Sword>();

        CharaRigid = GetComponent<Rigidbody>();
        CharaAnimator = GetComponent<Animator>();

        if (MainCamera == null)
            MainCamera = Camera.main;   //���� ī�޶� �������� ������ null�̸� ���� ī�޶� ã�Ƽ� �����Ѵ�
        MainCameraTr = MainCamera.transform;

        AnimeState = CharaAnimator.GetBehaviour<CheckAnimeState>();
        IsAttackAnime = false;
        CanMove = true;
    }

    private void Update()
    {
        /*
        1. ���� ��ư�� �ԷµǸ� ���� ���+�̹� ���� ����� ���ϴ� ���̶�� �ߺ� ����
        2. �̵� Ű�� �ԷµǸ� �̵�+���� ��� ���� ������ �̵��� �Ұ����� ��Ȳ�̶�� �̵� ����
        3. �ƹ��͵� �ϰ� ���� ���� ���� ������-IDLE
        */
        if (CharaInput.Attack && !IsAttackAnime)
            SwordAttack();
        else if ((CharaInput.MoveX != 0 || CharaInput.MoveZ != 0) && CanMove)
            Move();
        else
            CharaAnimator.SetBool("IsRunning", false);
    }

    /// <summary>
    /// ĳ���� �̵��� ���
    /// </summary>
    private void Move()
    {
        if (CharaInput && CharaRigid)
        {
            //ĳ���� ȸ��
            CameraLook = MainCameraTr.forward * CharaInput.MoveZ + MainCameraTr.right * CharaInput.MoveX;
            CameraLook.y = 0;
            CameraLook.Normalize();

            CharaRigid.rotation = Quaternion.Lerp(CharaRigid.rotation, Quaternion.LookRotation(CameraLook), RotateSpeed);

            //ĳ���� �̵�
            Vector3 V = transform.forward * CharaInput.MoveZ * MovingSpeed;
            CharaRigid.velocity = V;
            
            MovingDirection = CameraLook * CharaInput.MoveZ * MovingSpeed;
            CharaRigid.velocity = CameraLook * MovingSpeed;

            CharaAnimator.SetBool("IsRunning", true);
        }
    }

    /// <summary>
    /// ���� �ֵθ��� ���� ���
    /// </summary>
    private void SwordAttack()
    {
        CanMove = false;
        //CharaAnimator.SetBool("IsRunning", false);
        CharaAnimator.SetBool("IsAttack", true);
        //CharaAnimator.SetTrigger("OnAttack");
        SwordScript.ShowSword();
        IsAttackAnime = true;   //���� ��� �ߺ� ������ ������ true
        AnimeState.AnimeStateHandler += () =>{
            IsAttackAnime = false;
            SwordScript.HideSword();
            CanMove = true;
            CharaAnimator.SetBool("IsAttack", false);
            Debug.Log("���� ��� �Ϸ�");
        };    //���� ����� ������ IsAttack�� ��������, CanMove�� true�� �����Ͽ� �ٽ� ������ �� �ְ�
    }
}
