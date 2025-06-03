using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    private Button startButton;

    void Start()
    {
        // 获取按钮组件
        startButton = GetComponent<Button>();

        // 绑定点击事件
        startButton.onClick.AddListener(OnStartButtonClick);
    }

    void OnStartButtonClick()
    {
        // 加载菜单场景
        SceneManager.LoadScene("开始剧情");
    }
}