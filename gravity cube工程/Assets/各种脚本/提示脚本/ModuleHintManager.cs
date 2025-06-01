using UnityEngine;
using TMPro;

public class ModuleHintManager : MonoBehaviour
{
    public TextMeshProUGUI 提示文字;
    public ModuleInventory inventory;

    public 引力器 gravityTool;
    public 惯性锁 inertiaLock;
    public 防护罩 shieldTool;

    public TextMeshProUGUI 防护状态文本;
    public TextMeshProUGUI 冷却状态文本;

    public bool 靠近插槽 = false;
    public bool 插槽可插入 = false;
    public bool 插槽已插入 = false;

    void Update()
    {
        if (提示文字 == null || inventory == null)
            return;

        // -------- 1. 状态提示优先级 --------
        if (gravityTool != null && gravityTool.正在控制状态())
        {
            设置提示("控制中：WASD移动，G 退出控制");
            return;
        }

        if (inertiaLock != null && inertiaLock.惯性锁生效状态())
        {
            设置提示("再次右键取消惯性锁功能");
            return;
        }

        if (shieldTool != null && inventory.HasModule(ModuleType.防护罩))
        {
            float 无敌剩余 = shieldTool.当前无敌剩余时间;
            float 冷却剩余 = shieldTool.当前冷却剩余时间;

            if (防护状态文本 != null)
            {
                if (无敌剩余 > 0f)
                {
                    防护状态文本.text = $"无敌剩余：{无敌剩余:F1} 秒";
                    防护状态文本.enabled = true;
                }
                else
                {
                    防护状态文本.text = "";
                    防护状态文本.enabled = false;
                }
            }

            if (冷却状态文本 != null)
            {
                if (冷却剩余 > 0f)
                {
                    冷却状态文本.text = $"技能冷却中：{冷却剩余:F1} 秒";
                    冷却状态文本.enabled = true;
                }
                else
                {
                    冷却状态文本.text = "";
                    冷却状态文本.enabled = false;
                }
            }
        }
        else
        {
            // 没有防护罩 → 清空倒计时文字
            if (防护状态文本 != null)
            {
                防护状态文本.text = "";
                防护状态文本.enabled = false;
            }

            if (冷却状态文本 != null)
            {
                冷却状态文本.text = "";
                冷却状态文本.enabled = false;
            }
        }

        // -------- 2. 插槽提示（拉杆） --------
        ModuleType 当前 = inventory.GetCurrentModule();

        if (当前 == ModuleType.拉杆)
        {
            if (靠近插槽)
            {
                if (插槽可插入)
                {
                    设置提示("拥有拉杆之后，按 F 将拉杆插入无拉杆的插槽");
                    return;
                }
                else if (插槽已插入)
                {
                    设置提示("按 G 操作拉杆");
                    return;
                }
            }

            设置提示("拥有拉杆之后，按 F 将拉杆插入无拉杆的插槽");
            return;
        }

        // -------- 3. 默认模块提示 --------
        switch (当前)
        {
            case ModuleType.防护罩:
                设置提示("按 G 开启无敌保护");
                break;
            case ModuleType.惯性锁:
                设置提示("持有时，鼠标悬停平台，右键锁定");
                break;
            case ModuleType.引力器:
                设置提示("持有时靠近可选物体，按 G 控制");
                break;
        }

        // -------- 4. 选中但未持有模块提示 --------
        if (当前 != ModuleType.None && !inventory.HasModule(当前))
        {
            设置提示($"暂未拥有 {当前}");
        }
    }

    void 设置提示(string 内容)
    {
        if (提示文字.text != 内容)
        {
            提示文字.text = 内容;
            提示文字.enabled = true;
        }
    }
}
