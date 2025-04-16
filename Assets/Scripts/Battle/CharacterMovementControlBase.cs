using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovementControlBase : MonoBehaviour
{
    protected CharacterController _control;
    protected Animator _animator;
    protected Vector3 _movedirection;//��ɫ���ƶ�����
    //������
    protected bool _characterOnGround;//��ɫ�Ƿ��ڵ���
    [SerializeField, Header("������")] protected float _groundDetectionPositionOffset;
    [SerializeField] protected float _detectionRang;
    [SerializeField] protected LayerMask _whatIsGround;
    //����
    protected readonly float CharacterGravity = -9.8f;
    protected float _characterVerticalVelocity;//�������½�ɫY����ٶȣ�����������������Ծ�߶�
    protected float _fallOutDeltaTime;
    protected float _fallOutTime = 0.15f;
    protected readonly float _characterVerticalMaxVelocity = 54f;//��ɫ�ڵ������ֵ��ʱ�򣬲���ҪӦ������
    protected Vector3 _characterVerticalDirection;//��ɫ��Y���ƶ�����

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

    //����
    private void SetCharacterGravity()
    {
        _characterOnGround = GroundDetection();
        if (_characterOnGround)
        {
            /*
             * 1.�����ɫ�����ڵ����ϣ���ô��Ҫ����FallOutTime
             * 2.���ý�ɫ�Ĵ�ֱ�ٶ�
             */
            _fallOutDeltaTime = _fallOutTime;
            if (_characterVerticalVelocity < 0f)
            {
                _characterVerticalVelocity = -2f;
            }
        }
        else
        {
            //���ڵ���
            if (_fallOutDeltaTime > 0)
            {
                _fallOutDeltaTime -= Time.deltaTime;//�ȴ�0.15�룬������ɫ�ӽϵ͵ĸߴ�����
            }
            else
            {
                //˵����ɫ��û����أ���ô���ܲ�������¥�ݣ���Ҫ�������䶯����
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
    //�µ����
    private Vector3 StopResetDirection(Vector3 moveDirection)
    {
        //����ɫ�����Ƿ��������ƶ�����ֹ��ɫ�����ٶȹ���
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
