using UnityEngine;
using cakeslice;
using TMPro;

public class 惯性锁 : MonoBehaviour
{
    [Header("惯性锁设置")]
    [Tooltip("是否拥有惯性锁能力")]
    public bool 拥有惯性锁 = false;

    [Header("操作设置")]
    [Tooltip("使用惯性锁的按键")]
    public KeyCode 使用键 = KeyCode.Mouse1;

    [Header("UI 设置")]
    [Tooltip("提示文字组件")]
    public TextMeshProUGUI 提示文字;

    private Transform 当前目标平台;
    private Outline 当前描边;
    private Outline 悬停描边;
    private GameObject 当前悬停物体;
    private bool 惯性锁生效中 = false;

    void Update()
    {
        if (!拥有惯性锁)
        {
            清除提示();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (!惯性锁生效中 && hitObj.CompareTag("惯性锁平台"))
            {
                if (hitObj != 当前悬停物体)
                {
                    移除悬停描边();
                    添加悬停描边(hitObj);
                    设置提示("右键锁定该平台");
                }
            }
            else if (!惯性锁生效中) // Only clear the tip when the lock is not active
            {
                移除悬停描边();
                清除提示();
            }

            当前悬停物体 = hitObj;
        }
        else
        {
            移除悬停描边();
            if (!惯性锁生效中) // Clear tip only when lock is not active
            {
                清除提示();
            }
            当前悬停物体 = null;
        }

        if (Input.GetKeyDown(使用键))
        {
            if (!惯性锁生效中)
            {
                if (当前悬停物体 != null && 当前悬停物体.CompareTag("惯性锁平台"))
                {
                    var 平移 = 当前悬停物体.GetComponent<平移平台>();
                    var 旋转 = 当前悬停物体.GetComponent<旋转平台>();

                    if ((平移 != null && 平移.movementMode == MovementMode.Auto) ||
                        (旋转 != null && 旋转.rotationMode == RotationMode.Auto))
                    {
                        当前目标平台 = 当前悬停物体.transform;
                        锁定平台(当前悬停物体, 平移, 旋转);
                    }
                }
            }
            else
            {
                解锁平台();
            }
        }
    }

    void 锁定平台(GameObject 平台对象, 平移平台 平移, 旋转平台 旋转)
    {
        if (平移 != null)
        {
            if (平移.movementMode == MovementMode.Auto)
                平移.暂停移动();
            else
                平移.Deactivate();
        }
        else if (旋转 != null)
        {
            旋转.Deactivate();
        }

        当前描边 = 平台对象.GetComponent<Outline>();
        if (当前描边 == null)
        {
            当前描边 = 平台对象.AddComponent<Outline>();
        }

        当前描边.color = 2; // 紫色
        当前描边.eraseRenderer = false;

        惯性锁生效中 = true;
        设置提示("再次右键取消惯性锁功能");

        移除悬停描边(); // 不再显示绿色描边
    }

    void 解锁平台()
    {
        if (当前目标平台 == null) return;

        var 平移 = 当前目标平台.GetComponent<平移平台>();
        var 旋转 = 当前目标平台.GetComponent<旋转平台>();

        if (平移 != null)
        {
            if (平移.movementMode == MovementMode.Auto)
                平移.恢复移动();
            else
                平移.Activate();
        }
        else if (旋转 != null)
        {
            旋转.Activate();
        }

        if (当前描边 != null)
        {
            Destroy(当前描边);
        }

        当前目标平台 = null;
        当前描边 = null;
        惯性锁生效中 = false;

        清除提示(); // 清除提示
    }


    void 添加悬停描边(GameObject 平台对象)
    {
        悬停描边 = 平台对象.GetComponent<Outline>();
        if (悬停描边 == null)
        {
            悬停描边 = 平台对象.AddComponent<Outline>();
        }
        悬停描边.color = 0; // 浅绿色（cakeslice 默认色之一）
        悬停描边.eraseRenderer = false;

    }

    void 移除悬停描边()
    {
        if (悬停描边 != null && 悬停描边 != 当前描边)
        {
            Destroy(悬停描边);
        }
        悬停描边 = null;
    }

    void 设置提示(string 内容)
    {
        if (提示文字 != null)
        {
            提示文字.text = 内容;
            提示文字.enabled = true;
        }
    }

    void 清除提示()
    {
        if (提示文字 != null)
        {
            提示文字.text = "";
            提示文字.enabled = false;
        }
    }
}
