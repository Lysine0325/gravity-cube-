# 程序A工作记录

## 1. 玩家的移动/相机的互动

- 首先得明确我们相机和移动方式的互动

| 模式                                 | 相机行为             | 移动参考方向                           | 游戏体验                | 适用类型               |
| ------------------------------------ | -------------------- | -------------------------------------- | ----------------------- | ---------------------- |
| **① 世界固定方向**                   | 相机固定角度，不转动 | 按键永远对应世界方向（W=Z+, S=Z-）     | 最直觉、不会变          | 传统解谜游戏、策略游戏 |
| **② 相机可旋转，移动方向不变**       | 相机旋转（E/Q）      | 移动永远是世界坐标方向，不随相机旋转   | 转了视角，W可能变成向后 | 非常不推荐             |
| **③ 相机旋转，移动方向跟着相机变** ✅ | 相机旋转（E/Q）      | 按键方向与视觉一致（W 永远是“我眼前”） | 很直观，也很常见        | 箱庭、纪念碑谷式游戏   |
| **④ 第三人称跟随式**                 | 相机跟着人物移动     | 相机始终在人物后方（TPS）              | 控制灵活，自由探索      | 动作类、RPG            |

1. 基本角色控制脚本

- 涉及：CameraController.cs

- 用处：控制角色移动、角色重力下落
- 挂载：挂载到角色模型
- 操作：wasd/上下左右移动



2. 相机视角控制脚本

- 涉及：PlayerController.cs

- 用处：控制相机旋转
- 挂载：挂载给相机

- 要求：需要有一个空物体放在世界中心，然后把空物体给脚本的Target

## 2. 玩家模型被遮挡后的轮廓显示

- 涉及：插件/Outline.cs（插件自带）/OutlineEffect.cs（插件自带）/OcclusionOutlineController.cs

- 使用插件：https://github.com/cakeslice/Outline-Effect.git
- 用处：玩家模型被遮挡后的轮廓显示

- 挂载：
  - Outline挂载给人物
  - OcclusionOutlineController.cs挂载给人物
  - Outline Effect挂载给主相机

- 要求：OcclusionOutlineController可以选择遮挡物的图层，默认选择了obstacleMask（把遮挡物都加入这个Mask里面吧）

## 3. 人物爬梯子

- 涉及：CameraController1.cs（暂时这样命名）

- 用处：人物爬梯子

- 挂载：直接代替PlayerController.cs挂载给人物

- 要求：梯子的模型要加Ladder的Tag
- 使用方法：靠近之后按w（之后想改一下），空格可以脱离

---

以下是物品捡拾和交互系统：

整体的思路是：

- 物品的实体在地图中、可以捡拾进入背包、可以丢弃（用prefab生成）

- 物品实体不直接挂载在道具上，使用道具的时候，是通过检测背包里是否有物品来确定是否能使用该功能（功能在其他中文命名中的脚本）

- 像拉杆这种可插入的物品，需要插入插槽激活之后才能够实用



## 4. 玩家背包系统：ModuleInventory.cs

- 挂载对象：玩家

- **数据结构：** 每种道具有一个 `ModuleEntry { type, count }`
- **模块切换方式：** 数字键 1~3 固定对应拉杆、惯性锁、防护罩
- **操作逻辑：**
  - 玩家可以始终选中这些模块类型
  - 如果模块数为 0，仍然可以选中，但无法使用/丢弃（并有警告）
- **丢弃逻辑：**
  - Q 键触发丢弃
  - 若模块数 > 0，则在玩家面前生成 prefab 并减少数量

## 5. 场景中的拾取道具逻辑：ModuleItem.cs

- 挂载对象：场景中可捡拾的物品（已经挂载到prefab了其实，不需要手动挂载）

- 每个可捡拾的模块预制体都挂有该脚本
- 玩家触碰（`OnTriggerEnter`）时自动拾取该道具
- 拾取后：
  - 道具物体 `.SetActive(false)`
  - 模块加入背包 `inventory.AddModule(type)`



# --------------------------以下是背包里数据结构的详细解释

## 核心数据结构讲解

------

### 1️⃣ `heldModules : List<ModuleEntry>`

这是你**真正的“玩家背包”数据列表**，结构如下：

```
csharp复制编辑public class ModuleEntry
{
    public ModuleType type;  // 模块类型，例如 拉杆、惯性锁
    public int count;        // 玩家持有的该模块数量
}
```

✅ 功能说明：

- 每种模块只存一项（不重复）
- 用 `count` 表示数量，支持堆叠

🧩 示例状态（表示：有 2 个拉杆和 1 个惯性锁）：

```
csharp复制编辑heldModules = [
    { type: 拉杆, count: 2 },
    { type: 惯性锁, count: 1 }
]
```

------

### 2️⃣ `modulePrefabs : List<ModulePrefabEntry>`

这是一个**模块类型与其对应 prefab 的映射表**，用于在玩家丢弃模块时实例化道具。

```
csharp复制编辑public class ModulePrefabEntry
{
    public ModuleType type;
    public GameObject prefab;
}
```

在 `Start()` 函数中自动填充：

```
csharp


复制编辑
modulePrefabs.Add(new ModulePrefabEntry { type = ModuleType.拉杆, prefab = prefab_拉杆 });
```

🧩 用途：

- Q 键丢弃模块时调用 `GetPrefabForType(type)`
- 在玩家面前生成对应模块的实体

------

### 3️⃣ `selectedIndex : int`

这是**当前选中模块在 `heldModules` 中的索引**。

```
csharp


复制编辑
heldModules[selectedIndex].type
```

用来获取当前模块类型、数量、执行丢弃/使用等行为。

🧩 例如：

```
csharp复制编辑selectedIndex = 0
heldModules = [{ 拉杆, 2 }, { 惯性锁, 1 }]
```

表示玩家当前选中的是拉杆。

------

## 🎮 二、数字键 1~3 固定选中机制

你通过以下代码实现：

```
csharp复制编辑if (Input.GetKeyDown(KeyCode.Alpha1))
    SelectFixedType(ModuleType.拉杆);
```

然后在 `SelectFixedType()` 中查找背包是否已持有该模块：

```
csharp复制编辑int index = heldModules.FindIndex(m => m.type == type);
if (index >= 0)
    selectedIndex = index;
else
    heldModules.Add(new ModuleEntry { type = type, count = 0 }); // 添加“虚拟空模块”
```

✅ 作用：

- 即使你没捡过这个模块，也可以切换视角到它
- 如果之后捡到这个模块，就会复用这个 entry，自动变成 count > 0

------

## 🔁 三、背包行为流程总结

### ✅ 捡起模块（例如碰到拉杆）

- 如果 `heldModules` 中已有拉杆 → `count++`
- 否则新建一个 `{ type: 拉杆, count: 1 }`

### ✅ 丢弃当前模块（按 Q）

- 如果 `count > 0` → 实例化对应 prefab，数量 -1
- 如果 `count == 0` → 控制台警告，无法丢弃

### ✅ 切换当前模块（按 1/2/3）

- 改变 `selectedIndex` 指向
- 即使该模块没持有，也会加入 `{ type, count: 0 }`

------

## ✅ 总结图示关系（关键变量之间的关系）：

```
text复制编辑Player
 └── ModuleInventory
      ├── heldModules      ← 真实背包，用于数量管理
      ├── selectedIndex    ← 当前操作目标
      ├── modulePrefabs    ← 模块类型对应的 prefab 映射（丢弃用）
      └── prefab_XXX       ← 在 Inspector 中手动拖入绑定的实际资源
```







# --------------------------







## ✅ 三、脚本需求总结

| 脚本名                | 挂载对象                   | 作用                                       | 关键字段                                                     |
| --------------------- | -------------------------- | ------------------------------------------ | ------------------------------------------------------------ |
| `ModuleItem.cs`       | 插销、惯性锁等 3D 道具物体 | 表示这是一个模块道具，可拾取/放置          | `ModuleType type`, `bool isHeld`, `Transform originalParent` |
| `ModuleInventory.cs`  | 玩家                       | 管理背包内的模块物品，处理拾取、切换、使用 | `List<ModuleType> 持有模块`, `int 当前选中索引`              |
| `ModuleSocket.cs`     | 场景中插座/插槽            | 接收指定模块类型道具并触发事件             | `ModuleType 可接受类型`, `Transform 插入位置`, `UnityEvent 激活事件` |
| `ModuleUI.cs`（可选） | 玩家UI                     | 显示当前手持道具图标和切换提示             | `List<Image> 图标列表`                                       |

| 操作     | 插槽上行为                  | 玩家状态更新                  | 地面实体      |
| -------- | --------------------------- | ----------------------------- | ------------- |
| 拾取模块 | -                           | 玩家 `inventory.Add(type)`    | ❌ 不需要      |
| 插入模块 | 插槽模型显现 + 可交互启用   | 玩家 `inventory.Remove(type)` | ❌ 不需要      |
| 拔出模块 | 插槽模型隐藏 + 禁用拉杆功能 | 玩家 `inventory.Add(type)`    | ❌ 不生成      |
| 丢弃模块 | -                           | 玩家 `inventory.Remove(type)` | ✅ 生成 prefab |





## `FloatingItem.cs`

```
using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("旋转参数")]
    public float rotationSpeed = 50f;

    [Header("浮动参数")]
    public float floatAmplitude = 0.15f;  // 上下浮动幅度
    public float floatFrequency = 1f;     // 浮动频率

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // 上下浮动
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);
    }
}
```



## `ModuleInventory.cs` 功能目标：

| 功能               | 说明                                                |
| ------------------ | --------------------------------------------------- |
| 持有模块记录       | 玩家当前拥有的模块种类（如拉杆、惯性锁）            |
| 当前选中模块       | 玩家当前“准备使用”的模块类型（可用数字键/滚轮切换） |
| 添加模块           | 拾取时添加（例如拿起一个拉杆）                      |
| 移除模块           | 插入插槽或丢弃时移除                                |
| 判断是否持有某模块 | 插槽逻辑使用                                        |
| 丢弃模块           | 在玩家脚下生成 prefab                               |
