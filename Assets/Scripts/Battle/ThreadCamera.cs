using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadCamera : MonoBehaviour
{
    public Transform mPlayerTransform;//��ɫλ��
    public Vector3 UPAxis;//�����������෴������y��
    public Vector3 mDefaultDir;//�����������
    public Vector3 mPitchRotateAxis;//������
    public Vector3 mYawRotateAxis;//ƫ����
    Vector3 mRotateVlue;//ת��ֵ
    public float distance = 4f;//����������ľ���
    public float speed = 120f;//������任�ٶ�
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);//�����ƫ��ֵ
    public GameObject a;
    public GameObject b;
    public float x = 0;
    public bool invertPich;//��תpitch�����������
    public Vector2 pitchLimit = new Vector2(-40f, 70f);//pitch����Լ��
    public LayerMask obstacleLayerMasker;//��ײ�ϰ������
    public float obstacleSphereRadius = 0.3f;//�����뾶
    float mCurrentDistance;//������
    float mDistanceRecoveryDelayCounter;//�ƶ��ӳ�ʱ�����
    float mDistanceRecoverySpeed = 3f;//�����ٶ�
    float mDistanceRecoveryDelay = 1f;//�����ӳ�ʱ��
                                      //public GameObject player;
    void Start()
    {
        //mPlayerTransform = player.transform;
        var upAxis = -Physics.gravity.normalized;//��������ȷ��Y����
        Debug.Log(upAxis);
        UPAxis = upAxis;
        mDefaultDir = Vector3.ProjectOnPlane((transform.position - mPlayerTransform.position), upAxis).normalized;//ͶӰ�Ĵ��ߣ���������������
        Debug.Log(mDefaultDir);
        mYawRotateAxis = upAxis;//ƫ����
        mPitchRotateAxis = Vector3.Cross(upAxis, Vector3.ProjectOnPlane(transform.forward, upAxis));//���������ø�����
    }
    Vector3 ObstacleProcess(Vector3 from, Vector3 to)
    {
        var dir = (to - from).normalized;
        if (Physics.CheckSphere(from, obstacleSphereRadius, obstacleLayerMasker))
            Debug.Log("�����ϰ���������뾶ӦС�ڽ�ɫ����");
        var hit = default(RaycastHit);
        var isHit = Physics.SphereCast(new Ray(from, dir), obstacleSphereRadius, out hit, distance, obstacleLayerMasker);
        if (isHit)
            return hit.point + (-dir * obstacleSphereRadius);
        return to;
    }
    public float AngleCorrectionX(float value)
    {
        if (value > 180f) return mRotateVlue.x - 360f;
        else if (value < -180f) return mRotateVlue.x + 360f;
        return value;
    }
    public float AngleCorrectionY(float value)
    {
        if (value > 180f) return mRotateVlue.y - 360f;
        else if (value < -180f) return mRotateVlue.y + 360f;
        return value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(this.transform.position, UPAxis);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(mPlayerTransform.position, transform.position);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(this.transform.position, mDefaultDir);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, mPitchRotateAxis);


    }
    void Update()
    {
        var inputDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));//��ȡ����ƶ�����
        mRotateVlue.x += inputDelta.x * speed * Time.smoothDeltaTime;//x��任·��
        mRotateVlue.x = AngleCorrectionX(mRotateVlue.x);//�任����
                                                        //mRotateVlue.y += inputDelta.y * speed * Time.smoothDeltaTime;
        mRotateVlue.y += inputDelta.y * speed * (invertPich ? -1 : 1) * Time.smoothDeltaTime;//y��任·��
        mRotateVlue.y = AngleCorrectionY(mRotateVlue.y);//�任����
        mRotateVlue.y = Mathf.Clamp(mRotateVlue.y, pitchLimit.x, pitchLimit.y);//Լ��
        var horizontalQuat = Quaternion.AngleAxis(mRotateVlue.x, mYawRotateAxis);//��ƫ������ת����Ԫ��
        var verticalQuat = Quaternion.AngleAxis(mRotateVlue.y, mPitchRotateAxis);//�Ƹ�������ת����Ԫ��
        var finalDir = horizontalQuat * verticalQuat * mDefaultDir;//�������������Ԫ������ת
                                                                   //var from = mPlayerTransform.position;
        var from = mPlayerTransform.localToWorldMatrix.MultiplyPoint3x4(offset);//��ɫ����ת���������겢����ƫ��ֵ
                                                                                //Debug.Log(from);
                                                                                //b.transform.position = mPlayerTransform.position;
                                                                                //a.transform.position = from;
        var to = from + finalDir * distance;//Ŀ��λ��
        var exceptTo = ObstacleProcess(from, to);//�ж��Ƿ������ϰ���
        var exceptDistance = Vector3.Distance(exceptTo, from);//�ϰ������
        if (exceptDistance <= mCurrentDistance)//���Ļ����ͽ�������
        {
            mCurrentDistance = exceptDistance;
            mDistanceRecoveryDelayCounter = mDistanceRecoveryDelay;

        }
        else
        {
            if (mDistanceRecoveryDelayCounter > 0f)//�ӳ�����
            {
                mDistanceRecoveryDelayCounter -= Time.deltaTime;
            }
            else
            {
                mCurrentDistance = Mathf.Lerp(mCurrentDistance, exceptDistance, Time.smoothDeltaTime * mDistanceRecoverySpeed);
            }
        }
        transform.position = from + finalDir * mCurrentDistance;
        //transform.position = to;
        transform.LookAt(from);
    }
}
