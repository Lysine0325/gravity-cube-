using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // 新增命名空间

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
    private bool hasFinished = false; // 新增：标识流程已完成

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        canvasGroup = GetComponent<CanvasGroup>();
        tmpText = scrollRect.content.GetComponentInChildren<TextMeshProUGUI>();

        canvasGroup.alpha = 0f;
        scrollRect.verticalNormalizedPosition = 1f; // 从顶部开始

        tmpText.verticalAlignment = VerticalAlignmentOptions.Top;
        tmpText.overflowMode = TextOverflowModes.Truncate;
    }

    void Update()
    {
        // 淡入处理
        if (isFadingIn)
        {
            fadeTimer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(fadeTimer / fadeInDuration);

            if (fadeTimer >= fadeInDuration)
            {
                isFadingIn = false;
                fadeTimer = 0f;
            }
            return;
        }

        // 滚动处理
        if (isScrolling)
        {
            scrollRect.verticalNormalizedPosition -= Time.deltaTime * scrollSpeed / scrollRect.content.rect.height;

            // 接近底部时触发淡出
            if (scrollRect.verticalNormalizedPosition <= fadeOutDuration * scrollSpeed / scrollRect.content.rect.height)
            {
                isFadingOut = true;
            }

            // 滚动到底部时停止滚动
            if (scrollRect.verticalNormalizedPosition <= 0f)
            {
                scrollRect.verticalNormalizedPosition = 0f;
                isScrolling = false;
            }
        }

        // 淡出处理
        if (isFadingOut && !hasFinished)
        {
            fadeTimer += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(fadeTimer / fadeOutDuration);

            // 淡出完成后跳转场景
            if (fadeTimer >= fadeOutDuration)
            {
                hasFinished = true;
                LoadSelectScene();
            }
        }
    }

    // 新增：场景跳转方法
    private void LoadSelectScene()
    {
        SceneManager.LoadScene("SelectScene");
    }
}