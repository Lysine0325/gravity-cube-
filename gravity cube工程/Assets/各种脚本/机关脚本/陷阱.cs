using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class 陷阱 : MonoBehaviour
{
    [Header("玩家设置")]
    public string 玩家标签 = "Player";

    [Header("UI设置")]
    public GameObject 死亡界面;
    public Button TMP重玩按钮; // 修改为Button类型

    private bool 已触发 = false;

    void Start()
    {
        if (死亡界面 != null)
            死亡界面.SetActive(false);

        if (TMP重玩按钮 != null)
        {
            TMP重玩按钮.onClick.AddListener(重置关卡);
        }
        else
        {
            Debug.LogError("TMP按钮未绑定");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        处理触发检测(other);
    }

    void OnTriggerStay(Collider other)
    {
        处理触发检测(other);
    }

    void 处理触发检测(Collider other)
    {
        // 检查标签和是否已触发
        if (!other.CompareTag(玩家标签) || 已触发)
            return;

        // 获取防护罩组件
        防护罩 防护罩脚本 = other.GetComponent<防护罩>();

        // 如果处于无敌状态，跳过死亡
        if (防护罩脚本 != null && 防护罩脚本.是否免疫死亡())
        {
            Debug.Log("角色处于无敌状态，防护罩生效，免疫死亡");
            return;
        }

        // 标记已触发并执行死亡
        已触发 = true;
        触发死亡流程(other.gameObject);
    }




    void 触发死亡流程(GameObject 玩家对象)
    {
        var 玩家刚体 = 玩家对象.GetComponent<Rigidbody>();
        if (玩家刚体 != null)
            玩家刚体.isKinematic = true;

        if (死亡界面 != null)
            死亡界面.SetActive(true);

        Time.timeScale = 0f;
    }

    void 重置关卡()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        var collider = GetComponent<Collider>();
        if (collider is BoxCollider)
            Gizmos.DrawCube(transform.position, (collider as BoxCollider).size);
    }
}
