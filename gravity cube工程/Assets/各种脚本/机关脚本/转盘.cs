using UnityEngine;
using UnityEngine.UI;

public class 转盘 : MonoBehaviour
{
    [Header("控制设置")]
    [Tooltip("需要控制的旋转平台")]
    public 旋转平台 targetPlatform;
    [Tooltip("可操作的最大距离")]
    public float 可操作距离 = 3f;
    [Tooltip("每次旋转后锁定时间（防止连续触发）")]
    public float 操作冷却时间 = 0.5f;

    [Header("输入设置")]
    public KeyCode 顺时针键 = KeyCode.E;
    public KeyCode 逆时针键 = KeyCode.Q;

    [Header("界面引导")]
    [Tooltip("操作提示的UI图片")]
    public Image 引导图片;
    [Tooltip("提示图标的高度偏移")]
    public float 引导高度偏移 = 1.5f;

    private float 冷却计时器;
    private bool 允许操作 = true;
    private Transform 玩家;
    private bool 在范围内;

    void Start()
    {
        // 自动获取玩家对象
        玩家 = GameObject.FindGameObjectWithTag("Player").transform;

        // 初始化平台状态
        if (targetPlatform != null)
        {
            targetPlatform.Activate();
        }

        // 隐藏引导图片
        if (引导图片 != null)
        {
            引导图片.enabled = false;
        }
    }

    void Update()
    {
        if (玩家 == null || targetPlatform == null) return;

        // 更新距离检测
        更新距离检测();

        // 更新操作引导
        更新操作引导();

        // 操作冷却系统
        if (!允许操作)
        {
            冷却计时器 += Time.deltaTime;
            if (冷却计时器 >= 操作冷却时间)
            {
                允许操作 = true;
                冷却计时器 = 0f;
            }
        }

        // 仅在范围内处理输入
        if (在范围内 && 允许操作)
        {
            HandleRotationInput();
        }
    }

    void 更新距离检测()
    {
        float 当前距离 = Vector3.Distance(transform.position, 玩家.position);
        在范围内 = 当前距离 <= 可操作距离;
    }

    void 更新操作引导()
    {
        if (引导图片 == null) return;

        // 更新引导图片可见性
        引导图片.enabled = 在范围内;

        // 更新引导位置（世界坐标转屏幕坐标）
        if (在范围内)
        {
            Vector3 世界位置 = transform.position + Vector3.up * 引导高度偏移;
            Vector2 屏幕位置 = Camera.main.WorldToScreenPoint(世界位置);
            引导图片.rectTransform.position = 屏幕位置;
        }
    }

    void HandleRotationInput()
    {
        if (Input.GetKeyDown(顺时针键))
        {
            PerformRotation(RotationDirection.Clockwise);
        }
        else if (Input.GetKeyDown(逆时针键))
        {
            PerformRotation(RotationDirection.CounterClockwise);
        }
    }

    void PerformRotation(RotationDirection direction)
    {
        允许操作 = false;

        switch (direction)
        {
            case RotationDirection.Clockwise:
                targetPlatform.RotateClockwise();
                break;
            case RotationDirection.CounterClockwise:
                targetPlatform.RotateCounterClockwise();
                break;
        }

        // 添加操作反馈
        StartCoroutine(操作反馈());
    }

    System.Collections.IEnumerator 操作反馈()
    {
        Vector3 原始位置 = transform.position;
        float 按下位移 = 0.02f;

        // 轻微下压效果
        transform.position = 原始位置 - new Vector3(0, 按下位移, 0);
        yield return new WaitForSeconds(0.1f);
        transform.position = 原始位置;
    }

    void OnValidate()
    {
        // 自动绑定平台组件（如果为空）
        if (targetPlatform == null)
        {
            targetPlatform = GetComponent<旋转平台>();
        }

        // 确保距离有效
        可操作距离 = Mathf.Max(0.5f, 可操作距离);
    }

    void OnDrawGizmosSelected()
    {
        // 绘制操作范围
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 可操作距离);

        // 绘制平台连接线
        if (targetPlatform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, targetPlatform.transform.position);
        }
    }
}