using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class 开头剧情 : MonoBehaviour
{
    [Header("剧情文本设置")]
    [TextArea(3, 5)]
    public string[] dialogues = new string[]
{
    "在丛林深处，隐藏着阿兹柯尔遗迹 - 一座为力量而非神灵建造的古老迷宫。",

    "它的主人不是魔法师，而是工程师！这里的一切都由能量与重力网络驱动。",

    "三十米深的管道像血脉般流淌，支撑着浮空平台与自动巨门...",

    "这个文明的覆灭充满讽刺：当他们试图抽取星球核心能量时，",

    "重力系统突然崩塌，岩浆倒灌，防御地刺如骨刺般贯穿整个文明。",

    "现在，你 - 一个解密过无数失落遗迹的探险家，站在这片沉寂千年的遗迹前方。",

    "空气冰冷，充满尘土与烧焦的味道。突然，刺耳的警报撕裂寂静！",

    "在这片由秩序与遗忘交织的迷宫前，你的抉择会唤醒智慧...",

    "还是释放尘封千年的灾厄？按下左键开启这场冒险旅途..."
};
    public float typingSpeed = 0.05f; // 打字速度（秒）
    public TextMeshProUGUI dialogueText; // 对话文本组件
    public TextMeshProUGUI skipText; // 跳过提示文本

    [Header("UI组件")]
    public GameObject skipButton; // 跳过按钮
    public CanvasGroup dialoguePanel; // 整个对话面板

    [Header("音效设置")]
    public AudioSource audioSource;
    public AudioClip typeSound; // 打字音效
    public AudioClip nextDialogueSound; // 下一段对话音效

    [Header("场景设置")]
    public string nextSceneName = "SelectScene"; // 跳转的目标场景名

    private int currentIndex = 0; // 当前显示的对话索引
    private bool isTyping = false; // 是否正在打字
    private bool finishedTyping = false; // 当前对话是否完成
    private Coroutine typingCoroutine; // 打字协程

    private bool skipRequested = false; // 是否请求跳过

    void Start()
    {
        // 确保对话面板正确设置
        if (dialogueText == null) dialogueText = GetComponentInChildren<TextMeshProUGUI>();
        if (dialoguePanel == null) dialoguePanel = GetComponent<CanvasGroup>();

        // 初始状态设置
        dialogueText.text = "";
        currentIndex = -1; // 初始化为-1，这样第一次调用会显示索引0

        // 隐藏跳过按钮文本
        if (skipText != null) skipText.gameObject.SetActive(false);

        // 开始第一句话
        ShowNextDialogue();
    }

    void Update()
    {
        // 点击鼠标左键继续对话
        if (Input.GetMouseButtonDown(0))
        {
            // 如果正在打字，则立即完成当前对话
            if (isTyping)
            {
                FinishTyping();
            }
            // 如果当前对话已完成，则显示下一句
            else if (finishedTyping && !skipRequested)
            {
                ShowNextDialogue();
            }
        }

        // 显示跳过提示
        if (finishedTyping && !isTyping && !skipRequested && skipText != null)
        {
            skipText.gameObject.SetActive(true);
        }
    }

    // 显示下一句对话
    void ShowNextDialogue()
    {
        // 先递增索引
        currentIndex++;

        // 如果所有对话已完成，则跳转场景
        if (currentIndex >= dialogues.Length)
        {
            JumpToScene();
            return;
        }

        // 播放音效
        if (audioSource != null && nextDialogueSound != null)
        {
            audioSource.PlayOneShot(nextDialogueSound);
        }

        // 重置状态
        finishedTyping = false;

        // 隐藏跳过文本
        if (skipText != null) skipText.gameObject.SetActive(false);

        // 开始打字效果
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(dialogues[currentIndex]));
    }

    // 打字效果协程
    System.Collections.IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        // 逐字符显示
        foreach (char letter in text.ToCharArray())
        {
            // 如果请求跳过，则停止打字
            if (skipRequested) break;

            dialogueText.text += letter;

            // 播放打字音效
            if (audioSource != null && typeSound != null)
            {
                audioSource.PlayOneShot(typeSound);
            }

            // 等待一段时间
            yield return new WaitForSeconds(typingSpeed);
        }

        // 打字完成
        FinishTyping();
    }

    // 立即完成当前打字
    void FinishTyping()
    {
        if (isTyping)
        {
            // 停止协程
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }

            // 立即显示完整文本
            dialogueText.text = dialogues[currentIndex];

            // 更新状态
            isTyping = false;
            finishedTyping = true;
        }
    }

    // 跳转场景
    public void JumpToScene()
    {
        skipRequested = true; // 请求跳过
        SceneManager.LoadScene(nextSceneName);
    }

    // 跳过按钮事件
    public void OnSkipButtonClick()
    {
        JumpToScene();
    }
}