using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Rigidbody))]
public class 旋转平台2 : MonoBehaviour
{
    [Header("核心设置")]
    [Tooltip("旋转模式")]
    public RotationMode rotationMode = RotationMode.Auto;
    [Tooltip("基础旋转轴（局部坐标系）")]
    public RotationAxis baseRotationAxis = RotationAxis.Y;
    [Tooltip("轴偏移角度（欧拉角）")]
    public Vector3 axisOffset = Vector3.zero;
    [Tooltip("是否循环旋转")]
    public bool loopRotation = true;

    [Header("旋转参数")]
    [Tooltip("单次旋转时间（秒）")]
    [Range(0.1f, 10f)] public float rotationDuration = 2f;
    [Tooltip("循环模式停留时间（秒）")]
    [Range(0f, 10f)] public float pauseDuration = 1f;

    [Header("触发区域设置")]
    [Tooltip("触发区域高度偏移")]
    public float triggerYOffset = 0.5f;
    [Tooltip("触发区域尺寸")]
    public Vector3 triggerSize = new Vector3(1f, 0.2f, 1f);

    [Header("高级设置")]
    [Tooltip("显示调试信息")]
    public bool showDebug = true;

    private Rigidbody rb;
    private BoxCollider platformTrigger;
    private bool isRotating;
    private Quaternion axisRotation;
    private bool isActivated;
    private RotationDirection currentDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        CreatePlatformTrigger();
        UpdateAxisRotation();

        // 自动模式默认激活
        if (rotationMode == RotationMode.Auto)
        {
            Activate();
        }
    }

    void CreatePlatformTrigger()
    {
        platformTrigger = gameObject.AddComponent<BoxCollider>();
        platformTrigger.isTrigger = true;
        platformTrigger.center = new Vector3(0, triggerYOffset, 0);
        platformTrigger.size = triggerSize;
    }

    void FixedUpdate()
    {
        // 循环模式持续旋转
        if (loopRotation && isActivated && !isRotating)
        {
            StartCoroutine(RotatePlatform(currentDirection));
        }
    }

    void UpdateAxisRotation()
    {
        axisRotation = transform.rotation * Quaternion.Euler(axisOffset) * GetBaseAxis();
    }

    Quaternion GetBaseAxis()
    {
        switch (baseRotationAxis)
        {
            case RotationAxis.X: return Quaternion.Euler(90, 0, 0);
            case RotationAxis.Y: return Quaternion.identity;
            case RotationAxis.Z: return Quaternion.Euler(0, 0, 90);
            default: return Quaternion.identity;
        }
    }

    IEnumerator RotatePlatform(RotationDirection direction)
    {
        isRotating = true;
        UpdateAxisRotation();

        float targetAngle = 90f * (direction == RotationDirection.Clockwise ? -1 : 1);
        Vector3 worldAxis = axisRotation * GetLocalAxis();

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.AngleAxis(targetAngle, worldAxis) * startRotation;

        float elapsed = 0;
        while (elapsed < rotationDuration)
        {
            if (!isActivated) break; // 检查激活状态

            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / rotationDuration);
            rb.MoveRotation(Quaternion.Slerp(startRotation, endRotation, t));
            yield return null;
        }

        if (isActivated)
        {
            rb.MoveRotation(endRotation);

            // 循环模式暂停
            if (loopRotation)
            {
                yield return new WaitForSeconds(pauseDuration);
            }
        }

        isRotating = false;
    }

    Vector3 GetLocalAxis()
    {
        switch (baseRotationAxis)
        {
            case RotationAxis.X: return Vector3.right;
            case RotationAxis.Y: return Vector3.up;
            case RotationAxis.Z: return Vector3.forward;
            default: return Vector3.up;
        }
    }

    // 外部控制接口
    public void Activate()
    {
        isActivated = true;
        currentDirection = RotationDirection.Clockwise;
    }

    public void Deactivate()
    {
        isActivated = false;
        StopAllCoroutines();
        isRotating = false;
    }

    // 非循环模式专用旋转方法
    public void RotateClockwise()
    {
        if (!loopRotation && isActivated && !isRotating)
        {
            currentDirection = RotationDirection.Clockwise;
            StartCoroutine(RotatePlatform(currentDirection));
        }
    }

    public void RotateCounterClockwise()
    {
        if (!loopRotation && isActivated && !isRotating)
        {
            currentDirection = RotationDirection.CounterClockwise;
            StartCoroutine(RotatePlatform(currentDirection));
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!showDebug) return;

        // 旋转轴显示
        Vector3 worldAxis = axisRotation * GetLocalAxis();
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, worldAxis * 2f);
        Gizmos.DrawSphere(transform.position + worldAxis * 2f, 0.15f);

        // 局部坐标系显示
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, axisRotation * Vector3.right);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, axisRotation * Vector3.up);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, axisRotation * Vector3.forward);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        UpdateAxisRotation();
    }
#endif
}
