using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class 防护罩 : MonoBehaviour
{
    [Header("防护罩设置")]
    public bool 是否持有防护罩 = false; // 用于控制是否持有防护罩
    public float 无敌时间 = 3f; // 无敌时间（秒）
    public float 冷却时间 = 5f; // 冷却时间（秒）
    public float 当前无敌剩余时间 => 当前无敌时间;
    public float 当前冷却剩余时间 => 当前冷却时间;


    [Header("UI设置")]
    public TextMeshProUGUI 倒计时文本; // 用于显示倒计时
    public TextMeshProUGUI 冷却倒计时文本; // 用于显示冷却倒计时

    private bool 正在无敌 = false; // 是否处于无敌状态
    private float 当前无敌时间 = 0f; // 当前无敌时间
    private float 当前冷却时间 = 0f; // 当前冷却时间

    private Renderer 角色Renderer; // 角色的渲染器，用于控制透明度
    private bool 已免疫死亡 = false; // 标记是否已经免疫死亡

    void Start()
    {
        角色Renderer = GetComponent<Renderer>(); // 获取角色的渲染器
        if (倒计时文本 != null) 倒计时文本.text = ""; // 初始化倒计时文本
        if (冷却倒计时文本 != null) 冷却倒计时文本.text = ""; // 初始化冷却倒计时文本

    }

    void Update()
    {
        if (是否持有防护罩 && Input.GetKeyDown(KeyCode.G) && 当前冷却时间 <= 0f)
        {
            启动无敌状态();
        }

        if (正在无敌)
        {
            处理无敌状态();
        }
        else
        {
            处理冷却状态();
        }
    }

    void 启动无敌状态()
    {
        正在无敌 = true;
        当前无敌时间 = 无敌时间;
        当前冷却时间 = 冷却时间;
        角色Renderer.material.color = new Color(1f, 1f, 1f, 0.1f); // 角色变得半透明
        已免疫死亡 = true; // 启用免疫死亡
        if (倒计时文本 != null) 倒计时文本.text = "无敌中: " + 当前无敌时间.ToString("F1") + "s"; // 显示无敌倒计时
    }

    void 处理无敌状态()
    {
        // 改用 Time.unscaledDeltaTime 防止受时间暂停影响
        if (当前无敌时间 > 0)
        {
            当前无敌时间 -= Time.unscaledDeltaTime; // 修改此处
            if (倒计时文本 != null) 倒计时文本.text = "无敌中: " + 当前无敌时间.ToString("F1") + "s";
        }
        else
        {
            正在无敌 = false;
            角色Renderer.material.color = new Color(1f, 1f, 1f, 1f);
            已免疫死亡 = false;
            if (倒计时文本 != null) 倒计时文本.text = "";
        }
    }

    public void 强制结束无敌()
    {
        if (正在无敌)
        {
            当前无敌时间 = 0f;
            处理无敌状态(); // 立即触发状态结束
        }
        已免疫死亡 = false;  // 确保结束时清除免疫死亡标志
    }



    void 处理冷却状态()
    {
        // 改用 Time.unscaledDeltaTime 防止受时间暂停影响
        if (当前冷却时间 > 0)
        {
            当前冷却时间 -= Time.unscaledDeltaTime; // 修改此处
            if (冷却倒计时文本 != null) 冷却倒计时文本.text = "防护罩冷却: " + Mathf.Max(0f, 当前冷却时间).ToString("F1") + "s";
        }
        else
        {
            if (冷却倒计时文本 != null) 冷却倒计时文本.text = "";
        }
    }

    // 添加碰撞处理方法，避免陷阱触发死亡
    public bool 是否免疫死亡()
    {
        return 已免疫死亡;
    }
}
