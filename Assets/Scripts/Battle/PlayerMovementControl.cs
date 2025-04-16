using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovementControl : CharacterMovementControlBase
{
    private float _rotationAngle;
    private float _angleVelocity = 0f;
    public TextMeshPro _playerName;
    [SerializeField] private float _rotationSmoothTime;
    public Transform _mainCamera;
    private float SEND_MOVE_MSG_TIME = 0.2f;
    [SerializeField]private float lastSendMoveMsgTime;
    //脚步声
    private float _nextFootTime;
    [SerializeField] private float _slowFootTime;
    [SerializeField] private float _fastFootTime;
    private Vector3 _characterTargetDirection;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Start()
    {
        base.Start();

    }
    protected override void OnAnimatorMove()
    {
        base.OnAnimatorMove();
        
    }
    protected override void Update()
    {
        base.Update();
        Debug.Log("发送移动消息");
        PlayerMoveMsg playerMoveMsg = new PlayerMoveMsg();
        playerMoveMsg.id = GameManager.Instance.playerName;
        playerMoveMsg.posX = transform.position.x;
        playerMoveMsg.posY = transform.position.y;
        playerMoveMsg.posZ = transform.position.z;
        PlayerChangeMessage playerChangeMessage = new PlayerChangeMessage();
        playerChangeMessage.playerID = GameManager.Instance.playerName;
        playerChangeMessage.rotX = transform.eulerAngles.x;
        playerChangeMessage.rotY = transform.eulerAngles.y;
        playerChangeMessage.rotZ = transform.eulerAngles.z;
        PlayerAnimeMsg playerAnimeMsg = new PlayerAnimeMsg();
        playerAnimeMsg.id = GameManager.Instance.playerName;
        playerAnimeMsg.MovementID = _animator.GetFloat(AnimationID.MovementID);
        playerAnimeMsg.RunID = _animator.GetBool(AnimationID.RunID);
        playerAnimeMsg.HasInputID = _animator.GetBool(AnimationID.HasInputID);
        NetMgrAsync.Instance.Send(playerMoveMsg);
        NetMgrAsync.Instance.Send(playerChangeMessage);
        NetMgrAsync.Instance.Send(playerAnimeMsg);
    }
    private void LateUpdate()
    {
        UpdateAnimation();
        CharacterRotationControl();
    }
    //override
    //protected void Update()
    //{
    //    base.Update();
    //    CharacterRotationControl();
    //}
    private void CharacterRotationControl()
    {
        if (!_characterOnGround) return;
        if (_animator.GetBool(AnimationID.HasInputID))
        {
            _rotationAngle = Mathf.Atan2(GameInputManager.
            Instance.Movement.x, GameInputManager.Instance.Movement.y) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
        }
        if (_animator.GetBool(AnimationID.HasInputID) && _animator.GetCurrentAnimatorStateInfo(0).IsTag("Motion"))
        {
            Debug.Log("转弯");
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y,
           _rotationAngle, ref _angleVelocity, _rotationSmoothTime);
            _characterTargetDirection = Quaternion.Euler(0f, _rotationAngle, 0f) * Vector3.forward;
        }
        //_animator.SetFloat(AnimationID.DetalAngleID, DevelopmentToos.GetDeltaAngle(transform, _characterTargetDirection.normalized));
    }

    private void UpdateAnimation()
    {
        if (!_characterOnGround) return;
        _animator.SetBool(AnimationID.HasInputID, GameInputManager.Instance.Movement != Vector2.zero);

        if (_animator.GetBool(AnimationID.HasInputID))
        {
            if (GameInputManager.Instance.Run)
            {
                _animator.SetBool(AnimationID.RunID, true);
 
            }
            else
            {

            }
            _animator.SetFloat(AnimationID.MovementID, (_animator.GetBool(AnimationID.RunID) ? 2f : GameInputManager.Instance.Movement.sqrMagnitude), 0.25f, Time.deltaTime);

        }
        else
        {
            _animator.SetFloat(AnimationID.MovementID, 0f, 0.25f, Time.deltaTime);
            if (_animator.GetFloat(AnimationID.MovementID) < 0.2f)
            {
                _animator.SetBool(AnimationID.RunID, false);

            }
            else
            {

            }
        }

        //NetMgrAsync.Instance.Send(playerAnimeMsg);
    }


}