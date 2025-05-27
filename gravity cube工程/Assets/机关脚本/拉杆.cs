using UnityEngine;
using UnityEngine.Events;

public class 拉杆 : MonoBehaviour
{
    [Header("玩家设置")]
    [Tooltip("拖拽玩家对象到这里")]
    public GameObject 玩家;

    [Header("交互设置")]
    [Tooltip("触发交互的有效距离")]
    public float 触发距离 = 3f;
    [Tooltip("显示调试信息")]
    public bool 调试模式 = true;

    [Header("事件设置")]
    public UnityEvent 激活事件;
    public UnityEvent 取消激活事件;

    private bool 处于触发范围 = false;
    private bool 当前激活状态 = false;

    void Update()
    {
        if (玩家 == null) return;

        // 距离检测
        float 当前距离 = Vector3.Distance(transform.position, 玩家.transform.position);
        处于触发范围 = 当前距离 <= 触发距离;

        // 调试信息
        if (调试模式)
        {
            Debug.DrawLine(transform.position, 玩家.transform.position,
                处于触发范围 ? Color.green : Color.red);
        }

        // 按键检测
        if (处于触发范围 && Input.GetKeyDown(KeyCode.G))
        {
            切换状态();
        }
    }

    void 切换状态()
    {
        当前激活状态 = !当前激活状态;

        if (当前激活状态)
        {
            激活事件.Invoke();
            if (调试模式) Debug.Log("拉杆激活，触发激活事件");
        }
        else
        {
            取消激活事件.Invoke();
            if (调试模式) Debug.Log("拉杆关闭，触发取消激活事件");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (调试模式)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 触发距离);
        }
    }
}