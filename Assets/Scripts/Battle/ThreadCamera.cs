using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadCamera : MonoBehaviour
{
    public Transform mPlayerTransform;//角色位置
    public Vector3 UPAxis;//与重力方向相反，整体y轴
    public Vector3 mDefaultDir;//摄像机面向方向
    public Vector3 mPitchRotateAxis;//俯仰轴
    public Vector3 mYawRotateAxis;//偏航轴
    Vector3 mRotateVlue;//转动值
    public float distance = 4f;//人与摄像机的距离
    public float speed = 120f;//摄像机变换速度
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);//摄像机偏移值
    public GameObject a;
    public GameObject b;
    public float x = 0;
    public bool invertPich;//反转pitch方向相机滑动
    public Vector2 pitchLimit = new Vector2(-40f, 70f);//pitch方向约束
    public LayerMask obstacleLayerMasker;//碰撞障碍物对象
    public float obstacleSphereRadius = 0.3f;//检测球半径
    float mCurrentDistance;//检测距离
    float mDistanceRecoveryDelayCounter;//移动延迟时间计数
    float mDistanceRecoverySpeed = 3f;//拉近速度
    float mDistanceRecoveryDelay = 1f;//拉近延迟时间
                                      //public GameObject player;
    void Start()
    {
        //mPlayerTransform = player.transform;
        var upAxis = -Physics.gravity.normalized;//根据重力确立Y方向
        Debug.Log(upAxis);
        UPAxis = upAxis;
        mDefaultDir = Vector3.ProjectOnPlane((transform.position - mPlayerTransform.position), upAxis).normalized;//投影的垂线，获得摄像机面向方向
        Debug.Log(mDefaultDir);
        mYawRotateAxis = upAxis;//偏航轴
        mPitchRotateAxis = Vector3.Cross(upAxis, Vector3.ProjectOnPlane(transform.forward, upAxis));//向量叉乘求得俯仰轴
    }
    Vector3 ObstacleProcess(Vector3 from, Vector3 to)
    {
        var dir = (to - from).normalized;
        if (Physics.CheckSphere(from, obstacleSphereRadius, obstacleLayerMasker))
            Debug.Log("错误，障碍物检测球体半径应小于角色胶囊");
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
        var inputDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));//获取鼠标移动输入
        mRotateVlue.x += inputDelta.x * speed * Time.smoothDeltaTime;//x轴变换路程
        mRotateVlue.x = AngleCorrectionX(mRotateVlue.x);//变换修正
                                                        //mRotateVlue.y += inputDelta.y * speed * Time.smoothDeltaTime;
        mRotateVlue.y += inputDelta.y * speed * (invertPich ? -1 : 1) * Time.smoothDeltaTime;//y轴变换路程
        mRotateVlue.y = AngleCorrectionY(mRotateVlue.y);//变换修正
        mRotateVlue.y = Mathf.Clamp(mRotateVlue.y, pitchLimit.x, pitchLimit.y);//约束
        var horizontalQuat = Quaternion.AngleAxis(mRotateVlue.x, mYawRotateAxis);//绕偏航轴旋转的四元数
        var verticalQuat = Quaternion.AngleAxis(mRotateVlue.y, mPitchRotateAxis);//绕俯仰轴旋转的四元数
        var finalDir = horizontalQuat * verticalQuat * mDefaultDir;//面向变量进行四元数的旋转
                                                                   //var from = mPlayerTransform.position;
        var from = mPlayerTransform.localToWorldMatrix.MultiplyPoint3x4(offset);//角色坐标转换世界坐标并加上偏移值
                                                                                //Debug.Log(from);
                                                                                //b.transform.position = mPlayerTransform.position;
                                                                                //a.transform.position = from;
        var to = from + finalDir * distance;//目标位置
        var exceptTo = ObstacleProcess(from, to);//判断是否碰到障碍物
        var exceptDistance = Vector3.Distance(exceptTo, from);//障碍物距离
        if (exceptDistance <= mCurrentDistance)//近的话，就进行拉近
        {
            mCurrentDistance = exceptDistance;
            mDistanceRecoveryDelayCounter = mDistanceRecoveryDelay;

        }
        else
        {
            if (mDistanceRecoveryDelayCounter > 0f)//延迟拉近
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
