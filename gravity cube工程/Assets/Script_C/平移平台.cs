using UnityEngine;

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
    [Tooltip("水平移动方向角度（0=正X轴，逆时针增加）")]
    [Range(0, 360)] public float horizontalAngle = 0f;
    [Tooltip("平台单程移动距离")]
    public float moveDistance = 5f;
    [Tooltip("移动速度（单位/秒）")]
    public float moveSpeed = 2f;
    [Tooltip("触发模式下的停留时间")]
    public float pauseDuration = 1f;

    [Header("Waypoints (可选)")]
    [Tooltip("手动指定路径点（至少2个）")]
    public Transform[] customWaypoints;

    private Rigidbody rb;
    private Vector3[] waypoints;
    private int currentIndex = 0;
    private bool isMoving = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        InitializeWaypoints();
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
            // 根据角度计算水平方向
            Quaternion rotation = Quaternion.Euler(0, horizontalAngle, 0);
            return rotation * Vector3.right;
        }
        else
        {
            return transform.up;
        }
    }

    void FixedUpdate()
    {
        if (isMoving && (movementMode == MovementMode.Auto || IsActivated))
        {
            MovePlatform();
        }
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
            currentIndex = (currentIndex + 1) % waypoints.Length;
            if (movementMode == MovementMode.TriggerActivated)
                StartCoroutine(PauseMovement());
        }
    }

    System.Collections.IEnumerator PauseMovement()
    {
        isMoving = false;
        yield return new WaitForSeconds(pauseDuration);
        isMoving = true;
    }

    // 外部触发接口
    public bool IsActivated { get; private set; }

    public void Activate()
    {
        if (movementMode == MovementMode.TriggerActivated)
        {
            IsActivated = true;
            isMoving = true;
        }
    }

    public void Deactivate()
    {
        if (movementMode == MovementMode.TriggerActivated)
        {
            IsActivated = false;
        }
    }

    // 编辑器可视化
    void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawSphere(waypoints[i], 0.2f);
            if (i < waypoints.Length - 1)
                Gizmos.DrawLine(waypoints[i], waypoints[i + 1]);
        }

        // 绘制方向指示器
        if (directionMode == DirectionMode.Horizontal)
        {
            Vector3 dir = GetDirectionVector();
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, dir * 2f);
            Gizmos.DrawWireSphere(transform.position + dir * 2f, 0.3f);
        }
    }

#if UNITY_EDITOR
    // 在Inspector值改变时实时更新路径
    void OnValidate()
    {
        if (Application.isPlaying) return;
        InitializeWaypoints();
    }
#endif
}