using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MovementMode { Auto, TriggerActivated }
public enum DirectionMode { Horizontal, Vertical }

[RequireComponent(typeof(Rigidbody))]
public class 平移平台 : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("自动模式持续移动/触发模式需外部激活")]
    public MovementMode movementMode = MovementMode.Auto;
    [Tooltip("移动方向轴选择")]
    public DirectionMode directionMode = DirectionMode.Horizontal;
    [Tooltip("垂直移动是否向下（仅当方向为 Vertical 时生效）")]
    public bool verticalDown = false;
    [Tooltip("水平移动方向角度（0=正X轴，逆时针增加）")]
    [Range(0, 360)] public float horizontalAngle = 0f;
    [Tooltip("平台单程移动距离")]
    public float moveDistance = 5f;
    [Tooltip("移动速度（单位/秒）")]
    public float moveSpeed = 2f;
    [Tooltip("是否循环移动（非循环模式仅在触发激活时生效）")]
    public bool shouldLoop = true;

    [Header("停留时间设置")]
    [Tooltip("自动模式折返停留时间")]
    public float autoPauseDuration = 1f;
    [Tooltip("触发模式到达终点停留时间")]
    public float triggerPauseDuration = 1f;

    [Header("Waypoints (可选)")]
    [Tooltip("手动指定路径点（至少2个）")]
    public Transform[] customWaypoints;

    [Header("角色跟随设置")]
    [Tooltip("触发区域高度偏移")]
    public float triggerYOffset = 0.5f;
    [Tooltip("触发区域尺寸")]
    public Vector3 triggerSize = new Vector3(1f, 0.5f, 1f);


    // 私有变量
    private Rigidbody rb;
    private Vector3[] waypoints;
    private int currentIndex = 0;
    private bool isMoving = true;
    private bool isForwardDirection = true;
    private HashSet<CharacterController> activeRiders = new HashSet<CharacterController>();
    private Vector3 previousPosition;
    private BoxCollider platformTrigger;
    private Vector3 originalPosition;  // 初始位置
    private bool isReturning = false;    // 是否正在返回
    private bool reachedEnd = false;     // 是否到达终点
    public void 暂停移动()
    {
        isMoving = false;
    }

    public void 恢复移动()
    {
        isMoving = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        InitializeWaypoints();
        CreatePlatformTrigger();
        previousPosition = transform.position;
        originalPosition = waypoints[0]; // 记录初始位置
    }

    void CreatePlatformTrigger()
    {
        platformTrigger = gameObject.AddComponent<BoxCollider>();
        platformTrigger.isTrigger = true;
        platformTrigger.center = new Vector3(0, triggerYOffset, 0);
        platformTrigger.size = triggerSize;
    }

    void InitializeWaypoints()
    {
        if (customWaypoints.Length >= 2)
        {
            waypoints = new Vector3[customWaypoints.Length];
            for (int i = 0; i < customWaypoints.Length; i++)
                waypoints[i] = customWaypoints[i].position;
        }
        else
        {
            waypoints = new Vector3[2];
            Vector3 direction = GetDirectionVector();
            waypoints[0] = transform.position;
            waypoints[1] = transform.position + direction * moveDistance;
        }
    }

    Vector3 GetDirectionVector()
    {
        if (directionMode == DirectionMode.Horizontal)
        {
            Quaternion rotation = Quaternion.Euler(0, horizontalAngle, 0);
            return rotation * Vector3.right;
        }

        // 垂直方向：向上或向下
        return verticalDown ? -transform.up : transform.up;
    }

    void FixedUpdate()
    {
        UpdateRidersPosition();

        if (isMoving && ShouldMove())
        {
            MovePlatform();
        }
    }

    void UpdateRidersPosition()
    {
        Vector3 positionDelta = transform.position - previousPosition;

        foreach (CharacterController rider in activeRiders)
        {
            if (rider != null)
            {
                rider.Move(positionDelta);
            }
        }

        previousPosition = transform.position;
    }

    bool ShouldMove()
    {
        // 非循环模式下到达终点后停止移动
        if (!shouldLoop && movementMode == MovementMode.TriggerActivated && reachedEnd)
            return false;
        return movementMode == MovementMode.Auto || IsActivated;
    }

    void MovePlatform()
    {
        Vector3 targetPos = waypoints[currentIndex];
        Vector3 newPos = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            HandleWaypointReached();
        }
    }

    void HandleWaypointReached()
    {
        if (movementMode == MovementMode.Auto)
        {
            HandleAutoMode();
        }
        else
        {
            HandleTriggerMode();
        }

        StartCoroutine(PauseMovement(GetPauseDuration()));
    }

    void HandleAutoMode()
    {
        if (shouldLoop)
        {
            isForwardDirection = !isForwardDirection;
            currentIndex = isForwardDirection ?
                Mathf.Clamp(currentIndex + 1, 0, waypoints.Length - 1) :
                Mathf.Clamp(currentIndex - 1, 0, waypoints.Length - 1);
        }
        else
        {
            currentIndex = Mathf.Clamp(currentIndex + 1, 0, waypoints.Length - 1);
            if (currentIndex == waypoints.Length - 1) reachedEnd = true;
        }
    }

    void HandleTriggerMode()
    {
        if (shouldLoop)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
        else
        {
            if (IsActivated)
            {
                currentIndex = Mathf.Clamp(currentIndex + 1, 0, waypoints.Length - 1);
                isReturning = false;
                if (currentIndex == waypoints.Length - 1) reachedEnd = true;
            }
            else
            {
                currentIndex = Mathf.Clamp(currentIndex - 1, 0, waypoints.Length - 1);
            }
        }
    }

    float GetPauseDuration()
    {
        if (!shouldLoop && ((currentIndex == waypoints.Length - 1 && IsActivated) || (currentIndex == 0 && !IsActivated)))
            return 0f;

        return movementMode == MovementMode.Auto ?
            autoPauseDuration :
            triggerPauseDuration;
    }

    System.Collections.IEnumerator PauseMovement(float duration)
    {
        isMoving = false;
        yield return new WaitForSeconds(duration);

        isMoving = true;
    }

    public bool IsActivated { get; private set; }

    public void Activate()
    {
        if (movementMode == MovementMode.TriggerActivated)
        {
            IsActivated = true;
            isMoving = true;
            reachedEnd = false;

            if (!shouldLoop)
            {
                currentIndex = waypoints.Length - 1;
            }
        }
    }

    public void Deactivate()
    {
        if (movementMode == MovementMode.TriggerActivated)
        {
            IsActivated = false;

            if (!shouldLoop)
            {
                isMoving = true;
                currentIndex = 0;
                StartCoroutine(SmoothReturnToStart());
            }
        }
    }

    // 平滑返回到起始位置的协程
    private IEnumerator SmoothReturnToStart()
    {
        Vector3 currentPos = transform.position;
        while (Vector3.Distance(currentPos, originalPosition) > 0.1f)
        {
            currentPos = Vector3.MoveTowards(currentPos, originalPosition, moveSpeed * Time.deltaTime);
            rb.MovePosition(currentPos);
            yield return null;
        }
        rb.MovePosition(originalPosition);  // 确保到达起始位置
    }

    public void ResetPosition()
    {
        StopAllCoroutines();
        rb.MovePosition(originalPosition);
        currentIndex = 0;
        isMoving = true;
        reachedEnd = false;
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            activeRiders.Add(cc);
        }
    }

    void OnTriggerExit(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            activeRiders.Remove(cc);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints != null && waypoints.Length >= 2)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i], 0.2f);
                if (i < waypoints.Length - 1)
                    Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
            }
        }

        if (platformTrigger != null)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(platformTrigger.center, platformTrigger.size);
        }

        if (!shouldLoop && movementMode == MovementMode.TriggerActivated)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(originalPosition, Vector3.one * 0.5f);
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        InitializeWaypoints();
    }
#endif
}
