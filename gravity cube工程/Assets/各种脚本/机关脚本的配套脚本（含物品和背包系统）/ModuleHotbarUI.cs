using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModuleHotbarUI : MonoBehaviour
{
    public ModuleInventory inventory;

    [Header("拉杆")]
    public Image icon_拉杆;
    public TextMeshProUGUI count_拉杆;
    public Sprite 拉杆_彩色;
    public Sprite 拉杆_灰色;

    [Header("惯性锁")]
    public Image icon_惯性锁;
    public Sprite 惯性锁_彩色;
    public Sprite 惯性锁_灰色;

    [Header("防护罩")]
    public Image icon_防护罩;
    public Sprite 防护罩_彩色;
    public Sprite 防护罩_灰色;

    [Header("引力器")]
    public Image icon_引力器;
    public Sprite 引力器_彩色;
    public Sprite 引力器_灰色;

    [Header("边框（选中高亮）")]
    public Image border_拉杆;
    public Image border_惯性锁;
    public Image border_防护罩;
    public Image border_引力器;

    void Update()
    {
        if (inventory == null) return;

        // 拉杆：数量 + 图标
        int num_拉杆 = inventory.GetModuleCount(ModuleType.拉杆);
        count_拉杆.text = num_拉杆 > 0 ? num_拉杆.ToString() : "";
        icon_拉杆.sprite = num_拉杆 > 0 ? 拉杆_彩色 : 拉杆_灰色;

        // 其他模块图标变化
        icon_惯性锁.sprite = inventory.HasModule(ModuleType.惯性锁) ? 惯性锁_彩色 : 惯性锁_灰色;
        icon_防护罩.sprite = inventory.HasModule(ModuleType.防护罩) ? 防护罩_彩色 : 防护罩_灰色;
        icon_引力器.sprite = inventory.HasModule(ModuleType.引力器) ? 引力器_彩色 : 引力器_灰色;

        // 控制边框显示
        ModuleType 当前选中 = inventory.GetCurrentModule();
        border_拉杆.enabled = 当前选中 == ModuleType.拉杆;
        border_惯性锁.enabled = 当前选中 == ModuleType.惯性锁;
        border_防护罩.enabled = 当前选中 == ModuleType.防护罩;
        border_引力器.enabled = 当前选中 == ModuleType.引力器;
    }
}
