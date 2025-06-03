using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 跳转到指定的场景
    public void LoadScene(string sceneName)
    {
        // 检查是否有这个场景
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("场景 " + sceneName + " 不存在！");
        }
    }
}
