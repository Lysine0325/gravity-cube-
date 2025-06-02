using UnityEngine;

/// <summary>
/// 表示一个可以被玩家拾取、携带、放置的模块道具（如拉杆、惯性锁）
/// </summary>
public enum ModuleType
{
    None,
    拉杆,
    惯性锁,
    防护罩,
    引力器,
    转盘插柄
}

public class ModuleItem : MonoBehaviour
{
    [Header("模块基本信息")]
    public ModuleType type = ModuleType.None;   // 模块类型（插销、惯性锁等）
    public string itemName = "模块道具";          // 可选名称，用于 UI 显示

    [Header("交互属性")]
    public bool canInsertIntoSocket = true;     // 是否支持插入插槽（如拉杆 = true，惯性锁 = false）
    public bool isHeld = false;                 // 当前是否被玩家持有
    public bool allowReuse = true;              // 是否允许拔出并重复使用

    [Header("音效")]
    public AudioSource pickUpAudioSource;       // 用于播放拾取音效的音频源
    public AudioClip pickUpSound;               // 捡到道具时播放的音效

    private Transform originalParent;           // 初始生成位置
    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        // 初始化，记录原始位置，获取 Rigidbody 和 Collider 引用
        originalParent = transform.parent;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // 返回初始生成位置
    public void ResetToOriginalPosition()
    {
        isHeld = false;
        transform.SetParent(originalParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (rb) rb.isKinematic = false;
        if (col) col.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ModuleInventory inventory = other.GetComponent<ModuleInventory>();
            if (inventory != null)
            {
                inventory.AddModule(type);
                gameObject.SetActive(false); // 隐藏该道具
                Debug.Log($"玩家自动拾取模块：{type}");
                isHeld = true;

                // 播放捡到道具的音效
                if (pickUpAudioSource != null && pickUpSound != null)
                {
                    pickUpAudioSource.PlayOneShot(pickUpSound);
                }
            }
        }
    }
}
