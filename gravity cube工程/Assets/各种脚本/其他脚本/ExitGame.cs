using UnityEngine;
using UnityEngine.UI; // 引入UI命名空间

public class ExitGame : MonoBehaviour
{
    [Header("退出按钮设置")]
    public Button exitButton; // 在UI中绑定的退出按钮

    void Start()
    {
        // 确保退出按钮被绑定
        if (exitButton != null)
        {
            // 为按钮添加点击事件监听
            exitButton.onClick.AddListener(ExitApplication);
        }
        else
        {
            Debug.LogWarning("退出按钮没有绑定！");
        }
    }

    // 点击按钮时退出游戏
    private void ExitApplication()
    {
        Debug.Log("退出游戏...");
        // 执行退出游戏操作
        Application.Quit();

        // 如果是在编辑器中测试游戏，退出编辑器播放模式
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
