using UnityEngine;
using UnityEngine.UI;

public class 探险手册 : MonoBehaviour
{
    [Header("图片设置")]
    public Sprite[] 手册图片;          // 在Inspector中拖入所有需要展示的图片
    public Image 图片显示区域;         // 用于显示图片的UI Image组件

    [Header("按钮设置")]
    public Button 上一页按钮;
    public Button 下一页按钮;
    public Button 关闭按钮;
    public Button 手册按钮;            // 用于打开手册的按钮

    [Header("画布设置")]
    public Canvas 手册画布;           // 手册的画布

    [Header("音效设置")]
    public AudioSource 翻页音效;      // 用于播放翻页音效

    [Header("选项设置")]
    public bool 默认打开手册 = false; // 是否在场景启动时打开手册

    private int 当前页码 = 0;

    void Start()
    {
        // 初始化按钮事件
        上一页按钮.onClick.AddListener(上一页);
        下一页按钮.onClick.AddListener(下一页);
        关闭按钮.onClick.AddListener(关闭手册);
        手册按钮.onClick.AddListener(打开手册);

        // 根据设置初始化显示状态
        if (默认打开手册)
        {
            打开手册();
        }
        else
        {
            关闭手册();
        }

        // 确保初始显示正确
        更新显示();
    }

    // 打开手册（从第一页开始）
    void 打开手册()
    {
        当前页码 = 0;
        手册画布.gameObject.SetActive(true);
        手册按钮.gameObject.SetActive(false); // 打开手册时隐藏手册按钮
        更新显示();
    }

    // 关闭手册
    void 关闭手册()
    {
        手册画布.gameObject.SetActive(false);
        手册按钮.gameObject.SetActive(true); // 关闭手册时显示手册按钮
    }

    void 更新显示()
    {
        // 显示当前图片
        if (手册图片.Length > 0 && 当前页码 < 手册图片.Length)
        {
            图片显示区域.sprite = 手册图片[当前页码];
        }

        // 更新按钮状态
        上一页按钮.gameObject.SetActive(当前页码 > 0);
        下一页按钮.gameObject.SetActive(当前页码 < 手册图片.Length - 1);
    }

    void 上一页()
    {
        if (当前页码 > 0)
        {
            当前页码--;
            更新显示();
            播放翻页音效();
        }
    }

    void 下一页()
    {
        if (当前页码 < 手册图片.Length - 1)
        {
            当前页码++;
            更新显示();
            播放翻页音效();
        }
    }

    void 播放翻页音效()
    {
        if (翻页音效 != null && 翻页音效.clip != null)
        {
            翻页音效.Play();
        }
        else if (翻页音效 != null && 翻页音效.clip == null)
        {
            Debug.LogWarning("翻页音效未分配音频文件");
        }
    }
}