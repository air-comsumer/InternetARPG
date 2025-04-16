using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnoterPlayerController : CharacterMovementControlBase
{
    public string playerID;
    private float rotX;
    private float rotY;
    private float rotZ;
    private bool isRotate = false;
    [Header("动画参数")]
    private bool isAnimal = false;
    private bool HasInput;
    private bool Run;
    private float Move;
    [Header("移动参数")]
    [SerializeField]private Vector3 pos;
    private bool isMove;
    protected override void Awake()
    {
        base.Awake();

    }
    protected override void Start()
    {
        base.Start();
        pos = new Vector3(transform.position.x,
            transform.position.y - _groundDetectionPositionOffset, transform.position.z);
        NetMgrAsync.Instance.AddListener(2009, OnPlayerRotate);
        NetMgrAsync.Instance.AddListener(2010, OnPlayerAnim);
        NetMgrAsync.Instance.AddListener(2011, OnPlayerMove);
    }
    protected override void OnAnimatorMove()
    {
        if (isMove)
        {
            transform.position = pos;
            isMove = false;
        }
    }
    private void OnDestroy()
    {
        NetMgrAsync.Instance.RemoveListener(2009, OnPlayerRotate);
        NetMgrAsync.Instance.RemoveListener(2010, OnPlayerAnim);
        NetMgrAsync.Instance.RemoveListener(2011, OnPlayerMove);
    }
    private void LateUpdate()
    {
        if(isAnimal)
        {
            _animator.SetBool(AnimationID.HasInputID, HasInput);
            _animator.SetBool(AnimationID.RunID, Run);
            _animator.SetFloat(AnimationID.MovementID, Move);
            isAnimal = false;
        }
        if(isRotate)
        {
            transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
            isRotate = false;
        }
    }
    private void OnPlayerMove(BaseMsg arg0)
    {
        Debug.Log("收到移动消息");
        var playerMove = arg0 as PlayerMoveMsg;
        if (playerMove.id != playerID) return;
        pos = new Vector3(playerMove.posX, playerMove.posY, playerMove.posZ);//预测新位置
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
        
        rotY = playerRotate.rotY;
        rotX = playerRotate.rotX;
        rotZ = playerRotate.rotZ;
        isRotate = true;
    }
}
