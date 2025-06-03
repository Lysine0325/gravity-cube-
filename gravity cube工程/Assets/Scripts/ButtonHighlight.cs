using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 需要添加这个命名空间

[RequireComponent(typeof(Button))]
public class ButtonHighlight : MonoBehaviour
{
    [Header("背景图片设置")]
    public Sprite normalSprite;    // 正常状态背景
    public Sprite highlightedSprite; // 悬停状态背景

    [Header("文字颜色设置")]
    public Color normalTextColor = new Color(0.3725f, 0.4863f, 0.7373f, 1); // #5F7CBC
    public Color highlightedTextColor = new Color(0f, 0.1686f, 0.5294f, 1); // #002B87

    [Header("音效设置")]
    public AudioClip hoverSound; // 鼠标悬浮时播放的音效

    private Image buttonImage;
    private Text buttonText; // 如果是传统Text组件
    private TMPro.TextMeshProUGUI buttonTextPro; // 如果是TextMeshPro
    private AudioSource audioSource; // 音频源

    void Start()
    {
        buttonImage = GetComponent<Image>();

        // 获取文字组件（兼容传统Text和TextMeshPro）
        buttonText = GetComponentInChildren<Text>();
        buttonTextPro = GetComponentInChildren<TMPro.TextMeshProUGUI>();

        // 获取音频源组件
        audioSource = gameObject.AddComponent<AudioSource>(); // 如果没有音频源，自动添加一个

        // 设置初始状态
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }

        SetTextColor(normalTextColor);

        // 自动添加EventTrigger并绑定悬停事件
        AddEventTriggers();
    }

    // 自动添加鼠标悬停事件监听
    private void AddEventTriggers()
    {
        EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

        // 清除旧事件（避免重复）
        trigger.triggers.Clear();

        // 鼠标进入事件
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnPointerEnter(); });
        trigger.triggers.Add(entryEnter);

        // 鼠标离开事件
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnPointerExit(); });
        trigger.triggers.Add(entryExit);
    }

    // 鼠标进入时调用
    public void OnPointerEnter()
    {
        if (buttonImage != null && highlightedSprite != null)
        {
            buttonImage.sprite = highlightedSprite;
        }

        SetTextColor(highlightedTextColor);

        // 播放悬浮音效
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    // 鼠标离开时调用
    public void OnPointerExit()
    {
        if (buttonImage != null && normalSprite != null)
        {
            buttonImage.sprite = normalSprite;
        }

        SetTextColor(normalTextColor);
    }

    // 设置文字颜色（兼容传统Text和TextMeshPro）
    private void SetTextColor(Color color)
    {
        if (buttonText != null)
        {
            buttonText.color = color;
        }

        if (buttonTextPro != null)
        {
            buttonTextPro.color = color;
        }
    }
}
