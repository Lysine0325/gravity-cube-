using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("关卡按钮设置")]
    public Button[] levelButtons; // 存储三个按钮

    [Header("音效设置")]
    public AudioClip unlockedSound;    // 解锁关卡点击音效
    public AudioClip lockedSound;      // 锁定关卡警告音效

    private AudioSource audioSource;

    void Start()
    {
        // 确保有AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // 设置按钮点击事件
        SetupButtonListeners();
    }

    void SetupButtonListeners()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i; // 创建局部变量用于闭包
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(index));
        }
    }

    void OnLevelButtonClicked(int buttonIndex)
    {
        // 根据按钮索引直接设置场景名称
        string[] sceneNames = { "第一关", "第二关", "第三关" };
        string sceneName = sceneNames[buttonIndex]; // 根据按钮索引获取对应的场景名称

        // 播放解锁音效
        if (unlockedSound != null)
        {
            audioSource.PlayOneShot(unlockedSound);
        }

        // 加载对应场景
        SceneManager.LoadScene(sceneName);
    }
}
