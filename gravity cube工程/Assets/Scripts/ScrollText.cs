using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollText : MonoBehaviour
{
    public float scrollSpeed = 30f;
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;

    private ScrollRect scrollRect;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI tmpText;
    private bool isScrolling = true;
    private float fadeTimer = 0f;
    private bool isFadingIn = true;
    private bool isFadingOut = false;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        canvasGroup = GetComponent<CanvasGroup>();
        tmpText = scrollRect.content.GetComponentInChildren<TextMeshProUGUI>();

        // 初始设置
        canvasGroup.alpha = 0f;
        // 从顶部开始(1 = 底部, 0 = 顶部)
        scrollRect.verticalNormalizedPosition = 1f; // ⚠️ 修改这里：初始设为1（顶部）

        // 确保TMP文本设置正确
        tmpText.verticalAlignment = VerticalAlignmentOptions.Top;
        tmpText.overflowMode = TextOverflowModes.Truncate;
    }

    void Update()
    {
        // 淡入处理（保持不变）
        if (isFadingIn)
        {
            fadeTimer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(fadeTimer / fadeInDuration);

            if (fadeTimer >= fadeInDuration)
            {
                isFadingIn = false;
                fadeTimer = 0f;
            }
            return; // 淡入期间不滚动
        }

        // 滚动处理(从上往下)
        if (isScrolling)
        {
            // 修改这里：向下滚动（减少verticalNormalizedPosition）
            scrollRect.verticalNormalizedPosition -= Time.deltaTime * scrollSpeed / scrollRect.content.rect.height;

            // 检查是否需要开始淡出(接近底部时)
            if (scrollRect.verticalNormalizedPosition <= fadeOutDuration * scrollSpeed / scrollRect.content.rect.height)
            {
                isFadingOut = true;
            }

            // 检查是否滚动到底部
            if (scrollRect.verticalNormalizedPosition <= 0f)
            {
                scrollRect.verticalNormalizedPosition = 0f;
                isScrolling = false;
            }
        }

        // 淡出处理（保持不变）
        if (isFadingOut)
        {
            fadeTimer += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(fadeTimer / fadeOutDuration);

            if (fadeTimer >= fadeOutDuration)
            {
                isFadingOut = false;
                // 淡出完成后的逻辑
            }
        }
    }
}