using UnityEngine;
using UnityEngine.EventSystems; // 用于UI事件处理

public class 制作人员 : MonoBehaviour
{
    [Header("UI设置")]
    [Tooltip("制作人员名单的Canvas")]
    public GameObject creditsCanvas; // 拖入显示制作人员的Canvas

    [Tooltip("按钮点击音效(可选)")]
    public AudioClip clickSound; // 点击音效

    private AudioSource audioSource;

    // 标志，表示当前正在显示制作人员名单
    private bool creditsActive = false;

    void Awake()
    {
        // 确保游戏开始时名单不可见
        if (creditsCanvas != null)
            creditsCanvas.SetActive(false);

        // 获取音频组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // 如果制作人员名单处于激活状态，检测任意按键（但排除UI导航键）
        if (creditsActive && Input.anyKeyDown)
        {
            // 检查当前按下的键是否是UI导航键（方向键、确认键等），则忽略
            if (Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.Escape)) // 通常Escape键用于取消
            {
                return;
            }

            HideCredits();
        }
    }

    // 当按钮被点击时调用
    public void OnClick()
    {
        // 播放音效
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);

        // 显示制作人员名单
        ShowCredits();
    }

    // 显示制作人员名单
    public void ShowCredits()
    {
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(true);
            creditsActive = true;
        }
    }

    // 隐藏制作人员名单
    public void HideCredits()
    {
        if (creditsCanvas != null)
        {
            creditsCanvas.SetActive(false);
            creditsActive = false;
        }
    }
}