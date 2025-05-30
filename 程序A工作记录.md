# 程序A工作记录 5.26

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
- 新增逻辑：
  - 通过背包里是否有惯性锁和防护罩自动修改能否启用

## 5. 场景中的拾取道具逻辑：ModuleItem.cs

- 挂载对象：场景中可捡拾的物品（已经挂载到prefab了其实，不需要手动挂载）

- 每个可捡拾的模块预制体都挂有该脚本
- 玩家触碰（`OnTriggerEnter`）时自动拾取该道具
- 拾取后：
  - 道具物体 `.SetActive(false)`
  - 模块加入背包 `inventory.AddModule(type)`



# --------------------------以下是背包里数据结构的详细解释

## 一、核心数据结构讲解

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

##  二、数字键 1~3 固定选中机制

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

##  三、背包行为流程总结

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

## 四、总结图示关系（关键变量之间的关系）：

```
Player
 └── ModuleInventory
      ├── heldModules      ← 真实背包，用于数量管理
      ├── selectedIndex    ← 当前操作目标
      ├── modulePrefabs    ← 模块类型对应的 prefab 映射（丢弃用）
      └── prefab_XXX       ← 在 Inspector 中手动拖入绑定的实际资源
```

# --------------------------

### 表面行为 vs 实际逻辑总结(拉杆、背包)

| **功能**         | **表面行为**                              | **实际逻辑**                                                 |
| ---------------- | ----------------------------------------- | ------------------------------------------------------------ |
| **捡起拉杆**     | 玩家接触拉杆，拉杆消失并放入背包          | `ModuleItem.cs` 中的 `inventory.AddModule(type)` 被调用，将拉杆加入背包并更新数量 |
| **放入背包**     | 拉杆加入背包，UI 更新背包数量             | 背包检查是否已有该模块，若有则增加数量，否则创建新条目       |
| **插入插槽**     | 按 F 键插入拉杆，拉杆模型显示，机关激活   | `ModuleSocket.cs` 检查玩家背包是否有拉杆，减少背包中的拉杆数量，触发激活事件 |
| **使用拉杆功能** | 拉杆插入后触发机关激活或取消激活          | `拉杆脚本` 启用/禁用，根据状态触发相应的事件，如开启门或启动平台等 |
| **丢弃物品**     | 按 Q 键丢弃背包中的物品，物品出现在场景中 | `ModuleInventory.cs` 中的 `DropCurrentModule()` 被调用，在玩家前方实例化生成物品并减少背包数量 |



## 6. 拉杆的补充：插槽：ModuleSocket.cs

**用处**：为模块物品（如拉杆）提供插槽功能。玩家可以将物品插入插槽并触发相应的事件（例如激活机关、平台等）。

**挂载**：将 `ModuleSocket.cs` 挂载到插槽物体上，用于控制模块的插入、拔出及交互。

**Inspector需要做的**

- 初始拉杆状态一般设为false。
- 拉杆一般是子物体，需要拖入到拉杆模型的框内。
- 激活事件等没啥用，不用管。

**要求**：

- 插槽物体需要具备 `Collider`，并设置为 `isTrigger = true`，用于检测玩家是否接近插槽。
- 插槽中需要显示相应的物品模型（如拉杆模型），用于显示物品插入插槽后的效果。
- 拉杆等模块物品需要能够被插入插槽并激活/取消激活相应的功能。

**使用方法**：

- 玩家靠近插槽并按下 **F 键**，可以将物品（如拉杆）插入插槽。
- 插槽会显示物品的模型，可以开始使用拉杆的功能（目前是按G调用）（如开启平台、开门等）。
- 玩家按 **F 键** 拔出物品时，会重新获得物品
- 只能在背包里面已有拉杆的时候插入，如果背包中没有该物品，则没法使用。


