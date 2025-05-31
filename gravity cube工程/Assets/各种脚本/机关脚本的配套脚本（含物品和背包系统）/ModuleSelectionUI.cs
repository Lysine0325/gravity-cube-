using UnityEngine;
using TMPro;

public class ModuleSelectionUI : MonoBehaviour
{
    public ModuleInventory inventory;

    public TextMeshProUGUI 模块名称Text;
    public TextMeshProUGUI 模块提示Text;

    void Update()
    {
        if (inventory == null || 模块名称Text == null || 模块提示Text == null) return;

        ModuleType 当前模块 = inventory.GetCurrentModule();

        // 模块中文名称
        string 中文名 = 模块中文名(当前模块);
        模块名称Text.text = $"当前选中：{中文名}";

        // 对应的操作提示
        模块提示Text.text = 模块提示内容(当前模块);
    }

    string 模块中文名(ModuleType type)
    {
        return type switch
        {
            ModuleType.拉杆 => "拉杆",
            ModuleType.惯性锁 => "惯性锁",
            ModuleType.防护罩 => "防护罩",
            ModuleType.引力器 => "引力器",
            _ => "未选中",
        };
    }

    string 模块提示内容(ModuleType type)
    {
        return type switch
        {
            ModuleType.拉杆 => "靠近插槽，按 F 插拔控制开关，插入后用G操作",
            ModuleType.惯性锁 => "鼠标悬停平台，右键锁定/解除",
            ModuleType.防护罩 => "按 G 开启无敌保护",
            ModuleType.引力器 => "靠近物体，按 J 控制移动",
            _ => "",
        };
    }
}
