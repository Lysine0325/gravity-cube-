using UnityEngine;

public class ProximityInteractable : MonoBehaviour
{
    [Header("交互设置")]
    public float radius = 3f;         // 触发半径
    public GameObject promptUI;       // 提示UI对象

    private Transform player;         // 玩家引用
    private bool isActive;             // 当前是否可交互

    void Start()
    {
        // 查找玩家对象（需确保玩家有"Player"标签）
        player = GameObject.FindGameObjectWithTag("Player").transform;
        promptUI.SetActive(false);    // 初始隐藏提示
    }

    void Update()
    {
        // 添加空值保护
        if (player == null || promptUI == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isActive = distance <= radius && StateModuleA.IsSystemActive;

        promptUI.SetActive(isActive);

        if (isActive && Input.GetKeyDown(KeyCode.E))
        {
            Activate();
        }
    }

    void Activate()
    {
        Debug.Log("机关已激活！");
        // 在此处添加触发逻辑（如开门、播放动画等）
    }
}