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

    void Update()
    {
        if (inventory == null) return;

        // 拉杆：显示数量 + 图标变化
        int num_拉杆 = inventory.GetModuleCount(ModuleType.拉杆);
        count_拉杆.text = num_拉杆 > 0 ? num_拉杆.ToString() : "";
        icon_拉杆.sprite = num_拉杆 > 0 ? 拉杆_彩色 : 拉杆_灰色;

        // 惯性锁
        bool has_惯性锁 = inventory.HasModule(ModuleType.惯性锁);
        icon_惯性锁.sprite = has_惯性锁 ? 惯性锁_彩色 : 惯性锁_灰色;

        // 防护罩
        bool has_防护罩 = inventory.HasModule(ModuleType.防护罩);
        icon_防护罩.sprite = has_防护罩 ? 防护罩_彩色 : 防护罩_灰色;

        // 引力器
        bool has_引力器 = inventory.HasModule(ModuleType.引力器);
        icon_引力器.sprite = has_引力器 ? 引力器_彩色 : 引力器_灰色;
    }
}
