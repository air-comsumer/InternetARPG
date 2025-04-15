using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnoterPlayerController : CharacterMovementControlBase
{
    public string playerID;
    private float _rotationAngle;
    private float rotY;
    private float _angleVelocity = 0f;
    private float rotationSmoothTime;
    private bool isRotate;
    [Header("动画参数")]
    private bool isAnimal;
    private bool HasInput;
    private bool Run;
    private float Move;
    [Header("移动参数")]
    private float posX;
    private float posY;
    private float posZ;
    private bool isMove;
    protected override void Awake()
    {
        base.Awake();
        NetMgrAsync.Instance.AddListener(2009, OnPlayerRotate);
        NetMgrAsync.Instance.AddListener(2010, OnPlayerAnim);
        NetMgrAsync.Instance.AddListener(2011, OnPlayerMove);
    }
    protected override void OnAnimatorMove()
    {
        _animator.ApplyBuiltinRootMotion();
        _control.Move(new Vector3(posX, posY, posZ) * Time.deltaTime);
    }
    private void OnDestroy()
    {
        NetMgrAsync.Instance.RemoveListener(2009, OnPlayerRotate);
        NetMgrAsync.Instance.RemoveListener(2010, OnPlayerAnim);
        NetMgrAsync.Instance.RemoveListener(2011, OnPlayerMove);
    }
    private void LateUpdate()
    {
        UpdateAnimation();
        if (isRotate)
        {
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(rotY,
_rotationAngle, ref _angleVelocity, rotationSmoothTime);
            isRotate = false;
        }


    }
    private void UpdateAnimation()
    {
        if(isAnimal)
        {
            _animator.SetBool(AnimationID.HasInputID, HasInput);
            _animator.SetBool(AnimationID.RunID, Run);
            _animator.SetFloat(AnimationID.MovementID, Move);
            isAnimal = false;
        }
    }
    private void OnPlayerMove(BaseMsg arg0)
    {
        Debug.Log("收到移动消息");
        var playerMove = arg0 as PlayerMoveMsg;
        if (playerMove.id != playerID) return;
        posX = playerMove.posX;
        posY = playerMove.posY;
        posZ = playerMove.posZ;
        isMove = true;
    }

    private void OnPlayerAnim(BaseMsg arg0)
    {
        Debug.Log("收到动画消息");
        var playerAnim = arg0 as PlayerAnimeMsg;
        if(playerAnim.id != playerID) return;
        HasInput = playerAnim.HasInputID;
        Run = playerAnim.RunID;
        Move = playerAnim.MovementID;
        isAnimal = true;
    }

    private void OnPlayerRotate(BaseMsg arg0)
    {
        Debug.Log("收到旋转消息");
        var playerRotate = arg0 as PlayerChangeMessage;
        if (playerRotate.playerID != playerID) return;
        _rotationAngle = playerRotate.rotationAngle;
        rotY = playerRotate.rotY;
        rotationSmoothTime = playerRotate.rotationSmoothTime;
        isRotate = true;
    }
}
