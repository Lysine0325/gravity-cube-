using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionMenu : MonoBehaviour
{
    [System.Serializable]
    public class LevelUI
    {
        public Image[] starImages; // 该关卡对应的3个星星Image
    }

    [Header("关卡UI设置")]
    public LevelUI[] levelsUI = new LevelUI[3]; // 三个关卡的UI

    [Header("星星图片")]
    public Sprite starOnSprite;  // 已点亮图片
    public Sprite starOffSprite; // 未点亮图片

    private void Start()
    {
        LoadStarsData();
    }

    private void LoadStarsData()
    {
        // 获取所有关卡的星星数据
        int[] starsData = GameDataManager.GetLevelStarsData();

        for (int levelIndex = 0; levelIndex < 3; levelIndex++)
        {
            UpdateLevelUI(levelIndex, starsData[levelIndex]);
        }
    }

    private void UpdateLevelUI(int levelIndex, int stars)
    {
        if (levelIndex < 0 || levelIndex >= levelsUI.Length) return;

        LevelUI levelUI = levelsUI[levelIndex];

        // 更新星星显示
        for (int i = 0; i < 3; i++)
        {
            if (i < stars)
            {
                levelUI.starImages[i].sprite = starOnSprite;
            }
            else
            {
                levelUI.starImages[i].sprite = starOffSprite;
            }
        }
    }
}