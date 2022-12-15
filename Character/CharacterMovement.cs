using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class CharacterMovement : MonoBehaviour
{
    #region SetVariables
    [SerializeField] private float MovingSpeed = 8f; //캐릭터 이동 속도
    [SerializeField] private float RotateSpeed = 0.4f; //캐릭터 회전 속도
    [SerializeField] private Camera MainCamera; //메인 카메라

    private PlayerInput CharaInput;
    [SerializeField] private GameObject SwordObject;
    private Sword SwordScript;

    private Rigidbody CharaRigid;
    private Animator CharaAnimator;
    private Transform MainCameraTr;
    private Vector3 CameraLook; //카메라가 바라보는 방향
    private Vector3 MovingDirection; //이동 방향
    private CheckAnimeState AnimeState; //애니메이션 액션
    private bool IsAttackAnime; //공격 모션 중복 방지용
    private bool CanMove;   //움직일 수 있는지 여부

    #endregion

    private void Start()
    {
        CharaInput = GetComponent<PlayerInput>();
        SwordScript = SwordObject.GetComponent<Sword>();

        CharaRigid = GetComponent<Rigidbody>();
        CharaAnimator = GetComponent<Animator>();

        if (MainCamera == null)
            MainCamera = Camera.main;   //메인 카메라를 가져오는 변수가 null이면 메인 카메라를 찾아서 저장한다
        MainCameraTr = MainCamera.transform;

        AnimeState = CharaAnimator.GetBehaviour<CheckAnimeState>();
        IsAttackAnime = false;
        CanMove = true;
    }

    private void Update()
    {
        /*
        1. 공격 버튼이 입력되면 공격 모션+이미 공격 모션을 취하는 중이라면 중복 방지
        2. 이동 키가 입력되면 이동+공격 모션 중일 때같이 이동이 불가능한 상황이라면 이동 방지
        3. 아무것도 하고 있지 않을 때는 서있음-IDLE
        */
        if (CharaInput.Attack && !IsAttackAnime)
            SwordAttack();
        else if ((CharaInput.MoveX != 0 || CharaInput.MoveZ != 0) && CanMove)
            Move();
        else
            CharaAnimator.SetBool("IsRunning", false);
    }

    /// <summary>
    /// 캐릭터 이동을 담당
    /// </summary>
    private void Move()
    {
        if (CharaInput && CharaRigid)
        {
            //캐릭터 회전
            CameraLook = MainCameraTr.forward * CharaInput.MoveZ + MainCameraTr.right * CharaInput.MoveX;
            CameraLook.y = 0;
            CameraLook.Normalize();

            CharaRigid.rotation = Quaternion.Lerp(CharaRigid.rotation, Quaternion.LookRotation(CameraLook), RotateSpeed);

            //캐릭터 이동
            Vector3 V = transform.forward * CharaInput.MoveZ * MovingSpeed;
            CharaRigid.velocity = V;
            
            MovingDirection = CameraLook * CharaInput.MoveZ * MovingSpeed;
            CharaRigid.velocity = CameraLook * MovingSpeed;

            CharaAnimator.SetBool("IsRunning", true);
        }
    }

    /// <summary>
    /// 검을 휘두르는 공격 모션
    /// </summary>
    private void SwordAttack()
    {
        CanMove = false;
        //CharaAnimator.SetBool("IsRunning", false);
        CharaAnimator.SetBool("IsAttack", true);
        //CharaAnimator.SetTrigger("OnAttack");
        SwordScript.ShowSword();
        IsAttackAnime = true;   //공격 모션 중복 방지용 변수값 true
        AnimeState.AnimeStateHandler += () =>{
            IsAttackAnime = false;
            SwordScript.HideSword();
            CanMove = true;
            CharaAnimator.SetBool("IsAttack", false);
            Debug.Log("공격 모션 완료");
        };    //공격 모션이 끝나면 IsAttack을 거짓으로, CanMove를 true로 설정하여 다시 움직일 수 있게
    }
}
