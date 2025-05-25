using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public GameObject proximityPrompt;  // 靠近提示UI
    public GameObject clickPromptPrefab; // 点击提示预制体

    void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保留
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 显示点击提示
    public void ShowClickPrompt(Transform target)
    {
        GameObject prompt = Instantiate(clickPromptPrefab, transform);
        prompt.GetComponent<WorldSpaceFollow>().target = target;
    }

    // 隐藏点击提示（可选）
    public void HideClickPrompt(Transform target)
    {
        // 需扩展代码实现（见后续步骤）
    }
}