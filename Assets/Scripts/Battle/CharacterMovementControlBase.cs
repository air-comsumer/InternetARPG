using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovementControlBase : MonoBehaviour
{
    protected CharacterController _control;
    protected Animator _animator;
    protected Vector3 _movedirection;//角色的移动方向
    //地面检测
    protected bool _characterOnGround;//角色是否在地面
    [SerializeField, Header("地面检测")] protected float _groundDetectionPositionOffset;
    [SerializeField] protected float _detectionRang;
    [SerializeField] protected LayerMask _whatIsGround;
    //重力
    protected readonly float CharacterGravity = -9.8f;
    protected float _characterVerticalVelocity;//用来更新角色Y轴的速度，可以用于重力和跳跃高度
    protected float _fallOutDeltaTime;
    protected float _fallOutTime = 0.15f;
    protected readonly float _characterVerticalMaxVelocity = 54f;//角色在低于这个值的时候，才需要应用重力
    protected Vector3 _characterVerticalDirection;//角色的Y轴移动方向

    protected virtual void Awake()
    {
        _control = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }
    protected virtual void Start()
    {
        _fallOutDeltaTime = _fallOutTime;
    }
    protected virtual void Update()
    {
        SetCharacterGravity();
        UpdateCharacterGravity();
    }
    private bool GroundDetection()
    {
        var detectionPosition = new Vector3(transform.position.x,
            transform.position.y - _groundDetectionPositionOffset, transform.position.z);
        return Physics.CheckSphere(detectionPosition, _detectionRang, _whatIsGround, QueryTriggerInteraction.Ignore);
    }
    protected virtual void OnAnimatorMove()
    {
        _animator.ApplyBuiltinRootMotion();
        UpdateCharacterMovedirection(_animator.deltaPosition);
    }

    protected void UpdateCharacterMovedirection(Vector3 direction)
    {
        _movedirection = StopResetDirection(direction);
        _control.Move(_movedirection * Time.deltaTime);
    }

    //重力
    private void SetCharacterGravity()
    {
        _characterOnGround = GroundDetection();
        if (_characterOnGround)
        {
            /*
             * 1.如果角色现在在地面上，那么需要重置FallOutTime
             * 2.重置角色的垂直速度
             */
            _fallOutDeltaTime = _fallOutTime;
            if (_characterVerticalVelocity < 0f)
            {
                _characterVerticalVelocity = -2f;
            }
        }
        else
        {
            //不在地面
            if (_fallOutDeltaTime > 0)
            {
                _fallOutDeltaTime -= Time.deltaTime;//等待0.15秒，帮助角色从较低的高处下落
            }
            else
            {
                //说明角色还没有落地，那么可能不是在下楼梯，需要播放下落动画。
            }
            if (_characterVerticalVelocity < _characterVerticalMaxVelocity)
            {
                _characterVerticalVelocity += CharacterGravity * Time.deltaTime;
            }
        }
    }
    private void UpdateCharacterGravity()
    {
        _characterVerticalDirection.Set(0, _characterVerticalVelocity, 0f);
        _control.Move(_characterVerticalDirection * Time.deltaTime);
    }
    //坡道检测
    private Vector3 StopResetDirection(Vector3 moveDirection)
    {
        //检测角色现在是否在坡上移动，防止角色下坡速度过快
        if (Physics.Raycast(transform.position + (transform.up * .5f), Vector3.down, out var hit,
            _control.height * .85f, _whatIsGround, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Dot(Vector3.up, hit.normal) != 0)
            {
                return moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
            }
        }
        return moveDirection;
    }
    private void OnDrawGizmos()
    {
        var detectionPosition = new Vector3(transform.position.x,
            transform.position.y - _groundDetectionPositionOffset, transform.position.z);
        Gizmos.DrawWireSphere(detectionPosition, _detectionRang);
    }
}
