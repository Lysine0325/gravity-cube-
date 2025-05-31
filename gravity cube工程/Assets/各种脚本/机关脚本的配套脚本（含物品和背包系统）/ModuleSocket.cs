using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 插槽系统：用于放置拉杆并激活相应功能（如激活机关、平台）
/// </summary>
public class ModuleSocket : MonoBehaviour
{
    [Header("插槽设置")]
    public GameObject 拉杆模型;        // 插槽上的拉杆模型（默认为隐藏）
    public 拉杆 拉杆脚本;  // 拉杆.cs 脚本组件引用
    public float 触发距离 = 3f;         // 玩家触发插槽的距离
    public KeyCode 交互按键 = KeyCode.F; // 交互按键（默认为F键）
    public UnityEvent 激活事件;         // 拉杆插入后激活的事件
    public UnityEvent 取消激活事件;     // 拉杆拔出后取消激活的事件
    public bool 初始拉杆状态;

    private bool 处于触发范围 = false;  // 玩家是否在插槽触发范围内
    private bool 当前激活状态 = false;  // 当前拉杆是否激活，影响拔插

    private void Start()
    {
        拉杆模型.SetActive(初始拉杆状态);
        if (初始拉杆状态) 启用拉杆功能();
        if (初始拉杆状态) 当前激活状态 = true;
        else 禁用拉杆功能();//拉杆功能是对子对象的影响
    }

    private void Update()
    {
        // 当玩家按下 F 键并且处于插槽的触发范围内时
        if (处于触发范围 && Input.GetKeyDown(交互按键))
        {
            切换拉杆状态();
        }
    }

    // 检测玩家是否接触插槽
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            处于触发范围 = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            处于触发范围 = false;
        }
    }

    private void 禁用拉杆功能()
    {
        if (拉杆脚本 != null)
            拉杆脚本.enabled = false;
    }

    private void 启用拉杆功能()
    {
        if (拉杆脚本 != null)
            拉杆脚本.enabled = true;
    }

    // 放置/拔出拉杆
    private void 切换拉杆状态()
    {
        // 获取玩家的背包系统
        ModuleInventory inventory = GameObject.FindWithTag("Player").GetComponent<ModuleInventory>();

        // 判断玩家背包中是否有拉杆
        if (!inventory.HasModule(ModuleType.拉杆) && !当前激活状态)
        {
            Debug.Log("警告：背包中没有拉杆，无法插入！");
            return; // 如果背包中没有拉杆，禁止插入
        }

        if (当前激活状态)
        {
            // 如果已经激活，拔出拉杆，隐藏模型，取消激活
            拉杆模型.SetActive(false);
            当前激活状态 = false;
            取消激活事件.Invoke(); // 触发取消激活事件
            禁用拉杆功能();
            inventory.AddModule(ModuleType.拉杆);
            Debug.Log("拉杆已拔出");
        }
        else
        {
            // 如果没有激活，放置拉杆，显示模型，激活功能
            拉杆模型.SetActive(true);
            当前激活状态 = true;
            激活事件.Invoke(); // 触发激活事件
            启用拉杆功能();
            // 激活后减少背包中拉杆的数量
            inventory.Use拉杆();

            Debug.Log("拉杆已放置，背包中拉杆数量减少！");
        }
    }
}
