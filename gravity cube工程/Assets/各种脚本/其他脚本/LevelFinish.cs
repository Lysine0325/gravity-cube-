using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    [Header("关卡设置")]
    public int levelIndex = 0; // 当前关卡索引 (0,1,2)

    [Header("星星收集器")]
    public StarCollector starCollector;

    private bool isCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCompleted) return;

        if (other.CompareTag("Player"))
        {
            isCompleted = true;
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        // 保存星星数据
        GameDataManager.SaveLevelStars(levelIndex, starCollector.totalStars);

        // 这里可以添加关卡完成后的其他逻辑（如显示结算画面等）
        Debug.Log($"Level {levelIndex + 1} completed! Stars: {starCollector.totalStars}");
    }
}