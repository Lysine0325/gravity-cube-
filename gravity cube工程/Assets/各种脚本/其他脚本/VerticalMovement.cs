using UnityEngine;
using System.Collections;

public class VerticalMovement : MonoBehaviour
{
    [Header("竖直移动设置")]
    [Tooltip("上升高度")]
    public float riseHeight = 3f;
    [Tooltip("上升/下降速度")]
    public float verticalSpeed = 2f;
    [Tooltip("到达目标高度后的停留时间")]
    public float stayDuration = 0.5f;

    // 私有变量
    private Vector3 originalPosition;  // 初始位置
    private Vector3 targetPosition;    // 目标位置
    private bool isActivated = false;  // 是否激活上升
    private bool isMoving = false;     // 是否正在移动

    void Start()
    {
        // 记录初始位置
        originalPosition = transform.position;
        // 计算目标位置（仅Y轴变化）
        targetPosition = originalPosition + Vector3.up * riseHeight;
    }

    public void ActivateVertical()
    {
        if (!isMoving && !isActivated)
        {
            StartCoroutine(MoveToPosition(targetPosition));
            isActivated = true;
        }
    }

    public void DeactivateVertical()
    {
        if (!isMoving && isActivated)
        {
            StartCoroutine(MoveToPosition(originalPosition));
            isActivated = false;
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPos)
    {
        isMoving = true;

        // 平滑移动到目标位置
        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                verticalSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 确保精确到达目标位置
        transform.position = targetPos;

        // 停留一段时间
        yield return new WaitForSeconds(stayDuration);

        isMoving = false;
    }

    // 调试可视化
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(originalPosition, Vector3.one * 0.5f);
            Gizmos.DrawLine(originalPosition, originalPosition + Vector3.up * riseHeight);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(originalPosition + Vector3.up * riseHeight, Vector3.one * 0.5f);
        }
    }
}