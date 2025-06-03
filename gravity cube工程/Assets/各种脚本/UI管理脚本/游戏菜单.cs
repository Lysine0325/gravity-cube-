using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class 游戏菜单 : MonoBehaviour
{
    [Header("菜单设置")]
    public GameObject 菜单画布; // 拖入菜单Canvas对象
    public Button 继续按钮;    // 拖入继续游戏按钮
    public Button 首页按钮;    // 拖入返回首页按钮
    public Button 重玩按钮;    // 拖入重玩本关按钮

    private bool 游戏已暂停 = false;
    private string 当前场景名;

    void Start()
    {
        // 初始隐藏菜单
        菜单画布.SetActive(false);

        // 绑定按钮事件
        继续按钮.onClick.AddListener(继续游戏);
        首页按钮.onClick.AddListener(返回首页);
        重玩按钮.onClick.AddListener(重玩本关);

        // 记录当前场景
        当前场景名 = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        // 检测ESC键打开菜单（可根据需要修改按键）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            切换菜单状态();
        }
    }

    // 外部调用的菜单开关方法（可绑定到菜单按钮）
    public void 切换菜单状态()
    {
        游戏已暂停 = !游戏已暂停;
        菜单画布.SetActive(游戏已暂停);
        Time.timeScale = 游戏已暂停 ? 0f : 1f; // 暂停/恢复游戏时间[6,7](@ref)
    }

    // 继续游戏
    public void 继续游戏()
    {
        游戏已暂停 = false;
        菜单画布.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏时间[6](@ref)
    }

    // 返回首页
    public void 返回首页()
    {
        Time.timeScale = 1f; // 必须先恢复时间再切换场景[6](@ref)
        SceneManager.LoadScene("LoginScene"); // 加载登录场景[1,3](@ref)
    }

    // 重玩本关
    public void 重玩本关()
    {
        Time.timeScale = 1f; // 恢复游戏时间
        SceneManager.LoadScene(当前场景名); // 重新加载当前场景[3,4](@ref)
    }
}