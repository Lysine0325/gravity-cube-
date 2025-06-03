using UnityEngine;

public static class GameDataManager
{
    private const string LEVEL_STARS_KEY = "LevelStarsData";

    // 保存关卡星星数据
    public static void SaveLevelStars(int levelIndex, int stars)
    {
        // 确保关卡索引在有效范围内
        levelIndex = Mathf.Clamp(levelIndex, 0, 2);

        // 获取当前保存的数据
        int[] levelStars = GetLevelStarsData();

        // 只保存更高的星星数量
        if (stars > levelStars[levelIndex])
        {
            levelStars[levelIndex] = stars;

            // 将数组转换为字符串保存
            PlayerPrefs.SetString(LEVEL_STARS_KEY,
                $"{levelStars[0]},{levelStars[1]},{levelStars[2]}");
            PlayerPrefs.Save();
        }
    }

    // 获取所有关卡的星星数据
    public static int[] GetLevelStarsData()
    {
        string data = PlayerPrefs.GetString(LEVEL_STARS_KEY, "0,0,0");
        string[] values = data.Split(',');

        return new int[] {
            int.Parse(values[0]),
            int.Parse(values[1]),
            int.Parse(values[2])
        };
    }

    // 获取特定关卡的星星数量
    public static int GetStarsForLevel(int levelIndex)
    {
        int[] stars = GetLevelStarsData();
        levelIndex = Mathf.Clamp(levelIndex, 0, 2);
        return stars[levelIndex];
    }
}