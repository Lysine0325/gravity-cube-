using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 玩家模块背包：支持模块类型 + 数量统计（如拉杆 ×2）
/// 支持数字键1~3固定选择模块类型（拉杆/惯性锁/防护罩）
/// </summary>
public class ModuleInventory : MonoBehaviour
{
    [Header("模块背包列表")]
    public List<ModuleEntry> heldModules = new List<ModuleEntry>(); // 每种模块 + 拥有数量

    [Header("当前选中模块索引")]
    public int selectedIndex = 0;

    [Header("模块类型对应的Prefab")]
    public List<ModulePrefabEntry> modulePrefabs = new List<ModulePrefabEntry>();

    [Header("丢弃生成偏移")]
    public Vector3 dropOffset = new Vector3(0, 0.5f, 1f);

    [Header("模块类型预设体绑定")]
    public GameObject prefab_拉杆;
    public GameObject prefab_惯性锁;
    public GameObject prefab_防护罩;
    public GameObject prefab_引力器;

    private 防护罩 shieldComponent;
    private 惯性锁 inertiaComponent;
    private 引力器 GravityComponent;

    void Start()
    {
        // 自动填充 modulePrefabs 列表
        modulePrefabs.Clear();
        modulePrefabs.Add(new ModulePrefabEntry { type = ModuleType.拉杆, prefab = prefab_拉杆 });
        modulePrefabs.Add(new ModulePrefabEntry { type = ModuleType.惯性锁, prefab = prefab_惯性锁 });
        modulePrefabs.Add(new ModulePrefabEntry { type = ModuleType.防护罩, prefab = prefab_防护罩 });
        modulePrefabs.Add(new ModulePrefabEntry { type = ModuleType.引力器, prefab = prefab_引力器 });
        // 获取功能组件引用（假设和背包在同一 GameObject 上）
        inertiaComponent = GetComponent<惯性锁>();
        shieldComponent = GetComponent<防护罩>();
        GravityComponent = GetComponent <引力器>();
    }

    void Update()
    {
        HandleFixedKeySwitching();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropCurrentModule();
        }

        UpdateSpecialModuleStates();
    }

    /// <summary>
    /// 固定数字键 1~3 分别绑定拉杆、惯性锁、防护罩
    /// </summary>
    void HandleFixedKeySwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectFixedType(ModuleType.拉杆);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectFixedType(ModuleType.惯性锁);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectFixedType(ModuleType.防护罩);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectFixedType(ModuleType.引力器);
    }

    void SelectFixedType(ModuleType type)
    {
        // 如果背包已有该类型，则选中它的索引；否则记录一个“伪选中”索引
        int index = heldModules.FindIndex(m => m.type == type);
        if (index >= 0)
        {
            selectedIndex = index;
        }
        else
        {
            // 若不存在，添加一个虚拟 entry 只是为了指示选中类型（不占用背包）
            heldModules.Add(new ModuleEntry { type = type, count = 0 });
            selectedIndex = heldModules.Count - 1; // ← 关键补回这一行
        }

        Debug.Log($"选中模块：{type}（持有数量：{GetModuleCount(type)}）");
    }

    /// <summary>
    /// 获取当前选中的模块类型
    /// </summary>
    public ModuleType GetCurrentModule()
    {
        if (heldModules.Count == 0) return ModuleType.None;
        return heldModules[selectedIndex].type;
    }

    /// <summary>
    /// 判断是否拥有某类型模块
    /// </summary>
    public bool HasModule(ModuleType type)
    {
        return heldModules.Exists(entry => entry.type == type && entry.count > 0);
    }

    //持有状态更改（选中+持有！）
    void UpdateSpecialModuleStates()
    {
        ModuleType 当前选中 = GetCurrentModule();

        if (inertiaComponent != null)
            inertiaComponent.拥有惯性锁 = (当前选中 == ModuleType.惯性锁) && HasModule(ModuleType.惯性锁);

        if (shieldComponent != null)
            shieldComponent.是否持有防护罩 = (当前选中 == ModuleType.防护罩) && HasModule(ModuleType.防护罩);

        if (GravityComponent != null)
            GravityComponent.拥有引力器 = (当前选中 == ModuleType.引力器) && HasModule(ModuleType.引力器);
    }

    /// <summary>
    /// 增加一个模块（拾取）
    /// </summary>
    public void AddModule(ModuleType type)
    {
        ModuleEntry entry = heldModules.Find(e => e.type == type);
        if (entry != null)
        {
            entry.count++;
        }
        else
        {
            heldModules.Add(new ModuleEntry { type = type, count = 1 });
        }

        Debug.Log($"获得模块：{type}，当前数量：{GetModuleCount(type)}");
    }




    /// <summary>
    /// 减少当前模块的数量（暂时好像没用）
    /// </summary>
    public void UseCurrentModule()
    {
        if (heldModules.Count == 0) return;

        ModuleEntry entry = heldModules[selectedIndex];
        entry.count--;

        Debug.Log($"使用模块：{entry.type}，剩余数量：{entry.count}");

        if (entry.count <= 0)
        {
            heldModules.RemoveAt(selectedIndex);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, heldModules.Count - 1);
        }
    }
    /// <summary>
    /// 减少一个拉杆
    /// </summary>
    public void Use拉杆()
    {

        SelectFixedType(ModuleType.拉杆);
        ModuleEntry entry = heldModules[selectedIndex];
        entry.count--;

        Debug.Log($"使用模块：{entry.type}，剩余数量：{entry.count}");

        if (entry.count <= 0)
        {
            heldModules.RemoveAt(selectedIndex);
            selectedIndex = Mathf.Clamp(selectedIndex, 0, heldModules.Count - 1);
        }
    }


    /// <summary>
    /// 丢弃当前模块（在地面生成实体）
    /// </summary>
    public void DropCurrentModule()
    {
        if (heldModules.Count == 0) return;

        ModuleEntry entry = heldModules[selectedIndex];
        if (entry.count <= 0)
        {
            Debug.LogWarning($"当前模块 [{entry.type}] 数量为 0，无法丢弃！");
            return;
        }

        GameObject prefab = GetPrefabForType(entry.type);
        if (prefab != null)
        {
            Vector3 dropPos = transform.position + transform.forward * dropOffset.z + Vector3.up * dropOffset.y;
            Instantiate(prefab, dropPos, Quaternion.identity);
        }

        UseCurrentModule();
    }


    /// <summary>
    /// 获取某模块当前数量
    /// </summary>
    public int GetModuleCount(ModuleType type)
    {
        ModuleEntry entry = heldModules.Find(e => e.type == type);
        return entry != null ? entry.count : 0;
    }

    GameObject GetPrefabForType(ModuleType type)
    {
        foreach (var entry in modulePrefabs)
        {
            if (entry.type == type)
                return entry.prefab;
        }
        return null;
    }

    [System.Serializable]
    public class ModuleEntry
    {
        public ModuleType type;
        public int count;
    }

    [System.Serializable]
    public class ModulePrefabEntry
    {
        public ModuleType type;
        public GameObject prefab;
    }
}
