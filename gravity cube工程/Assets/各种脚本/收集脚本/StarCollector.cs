using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarCollector : MonoBehaviour
{
    public int totalStars = 0;

    public TextMeshProUGUI starText;

    [Header("星星 UI")]
    public Image[] starImages; // 拖入3个Image对象
    public Sprite starOnSprite;  // 已点亮图片
    public Sprite starOffSprite; // 未点亮图片

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            other.enabled = false;
            Destroy(other.gameObject);
            totalStars = Mathf.Clamp(totalStars + 1, 0, starImages.Length);
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // 更新文字
        if (starText != null)
            starText.text = $"Stars: {totalStars}";

        // 更新星星图片
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                starImages[i].sprite = (i < totalStars) ? starOnSprite : starOffSprite;
            }
        }
    }
}
