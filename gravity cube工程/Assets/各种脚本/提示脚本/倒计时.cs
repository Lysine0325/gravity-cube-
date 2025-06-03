using UnityEngine;
using TMPro;
using System.Collections;

public class 倒计时 : MonoBehaviour
{
    [Header("倒计时设置")]
    [Tooltip("倒计时的总时间（秒）")]
    public float countdownTime = 90f;  // 默认1分30秒
    private float currentTime;         // 当前倒计时剩余时间
    private bool isCountingDown = false;  // 是否在倒计时中

    [Header("UI 设置")]
    [Tooltip("倒计时显示文本组件")]
    public TextMeshProUGUI countdownText;
    [Tooltip("岩浆开始上涨的文本组件")]
    public TextMeshProUGUI lavaRisingText;  // 新增的TextMeshProUGUI组件

    [Header("闪烁效果设置")]
    [Tooltip("倒计时文字闪烁的颜色")]
    public Color countdownColor = Color.red;  // 默认红色
    [Tooltip("闪烁效果的频率，值越大闪烁越快")]
    public float blinkSpeed = 1f;

    private Color defaultColor;  // 默认颜色

    private void Start()
    {
        // 初始化当前倒计时时间
        currentTime = countdownTime;

        // 默认颜色为白色
        defaultColor = countdownText.color;

        // 确保文本最初不可见
        if (countdownText != null)
        {
            countdownText.enabled = false;
        }

        if (lavaRisingText != null)
        {
            lavaRisingText.enabled = false;  // 确保“岩浆开始上涨！”文本最初不可见
        }
    }

    void Update()
    {
        // 如果倒计时正在进行
        if (isCountingDown)
        {
            // 减少剩余时间
            currentTime -= Time.deltaTime;

            // 更新时间显示
            UpdateCountdownText();

            // 如果时间结束
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isCountingDown = false;  // 停止倒计时
                // 可以在此触发其他事件，比如倒计时结束后的逻辑
                Debug.Log("倒计时结束！");
            }
        }

        // 如果倒计时正在显示，应用闪烁效果
        if (isCountingDown && countdownText != null)
        {
            BlinkTextColor(countdownText);  // 为倒计时文字添加闪烁效果
        }

        // 为“岩浆开始上涨！”文字添加闪烁效果
        if (lavaRisingText != null && lavaRisingText.enabled)
        {
            BlinkTextColor(lavaRisingText);  // 为岩浆文字添加闪烁效果
        }
    }

    // 触发倒计时开始的函数接口
    public void StartCountdown()
    {
        if (!isCountingDown)
        {
            StartCoroutine(ShowLavaRisingMessageAndStartCountdown());  // 开始显示岩浆文本并启动倒计时
        }
    }

    // 协程处理岩浆开始上涨的淡入淡出效果，并应用闪烁
    private IEnumerator ShowLavaRisingMessageAndStartCountdown()
    {
        // 显示岩浆开始上涨的文字，并淡入
        if (lavaRisingText != null)
        {
            lavaRisingText.enabled = true;
            float duration = 1f;  // 淡入淡出持续时间
            float timeElapsed = 0f;

            // 淡入效果
            while (timeElapsed < duration)
            {
                lavaRisingText.color = new Color(lavaRisingText.color.r, lavaRisingText.color.g, lavaRisingText.color.b, Mathf.Lerp(0f, 1f, timeElapsed / duration));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            lavaRisingText.color = new Color(lavaRisingText.color.r, lavaRisingText.color.g, lavaRisingText.color.b, 1f);  // 确保完全显示

            // 保持显示一段时间
            yield return new WaitForSeconds(1f);

            // 淡出效果
            timeElapsed = 0f;
            while (timeElapsed < duration)
            {
                lavaRisingText.color = new Color(lavaRisingText.color.r, lavaRisingText.color.g, lavaRisingText.color.b, Mathf.Lerp(1f, 0f, timeElapsed / duration));
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            lavaRisingText.color = new Color(lavaRisingText.color.r, lavaRisingText.color.g, lavaRisingText.color.b, 0f);  // 确保完全隐藏
            lavaRisingText.enabled = false;

            // 开始倒计时
            StartCountdownTimer();
        }
    }

    // 开始倒计时
    private void StartCountdownTimer()
    {
        currentTime = countdownTime;  // 重置倒计时
        isCountingDown = true;
        if (countdownText != null)
        {
            countdownText.enabled = true;  // 显示倒计时文本
        }
    }

    // 更新时间显示
    private void UpdateCountdownText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        countdownText.text = $"神庙崩塌中：{minutes}分{seconds:D2}秒";  // 显示倒计时格式
    }

    // 闪烁文字颜色
    private void BlinkTextColor(TextMeshProUGUI text)
    {
        float lerpValue = Mathf.PingPong(Time.time * blinkSpeed, 1f);  // 通过时间控制闪烁
        text.color = Color.Lerp(defaultColor, countdownColor, lerpValue);  // 在白色和当前颜色之间平滑过渡
    }
}
