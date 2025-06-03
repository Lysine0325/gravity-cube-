using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class VictorySettlement : MonoBehaviour
{
    [Header("胜利结算界面")]
    public GameObject victoryCanvas; // 拖入胜利结算的Canvas对象

    [Header("按钮设置")]
    public Button nextLevelButton;    // 下一关按钮
    public Button replayButton;       // 重玩本关按钮
    public Button selectLevelButton;  // 返回选关按钮

    [Header("音效设置")]
    public AudioClip victorySound;    // 胜利音效
    private AudioSource audioSource;

    [Header("星星显示设置")]
    public Image[] starImages;        // 拖入3个Image对象，用于显示星星
    public Sprite starOnSprite;       // 已点亮图片
    public Sprite starOffSprite;      // 未点亮图片

    private StarCollector starCollector; // 用于获取星星数量
    private bool isVictory = false;   // 是否已胜利

    void Start()
    {
        // 确保结算界面初始隐藏
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(false);
        }

        // 设置音频源
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 获取StarCollector组件
        starCollector = FindObjectOfType<StarCollector>();

        // 绑定按钮事件
        nextLevelButton.onClick.AddListener(LoadNextLevel);
        replayButton.onClick.AddListener(ReplayLevel);
        selectLevelButton.onClick.AddListener(BackToLevelSelection);
    }

    void OnTriggerEnter(Collider other)
    {
        // 检测玩家到达终点
        if (other.CompareTag("Player") && !isVictory)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        isVictory = true;

        // 停止当前场景的音效
        foreach (var audio in FindObjectsOfType<AudioSource>())
        {
            if (audio != audioSource) // 排除胜利音效的AudioSource
            {
                audio.Stop();
            }
        }

        // 播放胜利音效
        if (victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

        // 显示结算界面
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(true);
        }

        // 更新星星数量（根据StarCollector的得分）
        if (starCollector != null)
        {
            UpdateStars(starCollector.totalStars);
        }

        // 暂停游戏逻辑（可选）
        Time.timeScale = 0f;

        // 锁定玩家输入（根据您的实现方式）
        // 例如：FindObjectOfType<PlayerController>().DisableInput();
    }

    void UpdateStars(int starCount)
    {
        // 更新星星显示
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < starCount)
            {
                starImages[i].sprite = starOnSprite;
            }
            else
            {
                starImages[i].sprite = starOffSprite;
            }
        }
    }

    // 加载下一关
    void LoadNextLevel()
    {
        ResumeGame();
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // 检查是否有下一关
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // 如果是最后一关，返回选关界面
            SceneManager.LoadScene("SelectScene");
        }
    }

    // 重玩本关
    void ReplayLevel()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 返回选关界面
    void BackToLevelSelection()
    {
        ResumeGame();
        SceneManager.LoadScene("SelectScene");
    }

    // 恢复游戏状态
    void ResumeGame()
    {
        Time.timeScale = 1f;
        // 解锁玩家输入（根据您的实现方式）
        // 例如：FindObjectOfType<PlayerController>().EnableInput();
    }
}
