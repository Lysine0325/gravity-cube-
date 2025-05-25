using UnityEngine;

public class WorldSpaceFollow : MonoBehaviour
{
    [Header("绑定设置")]
    public Transform target;      // 需要跟随的3D物体
    public Vector3 screenOffset = new Vector3(0, 50, 0); // 屏幕空间偏移

    private RectTransform rectTransform;
    private Camera mainCamera;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (target == null || mainCamera == null) return;

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.position);

        // 应用偏移并更新位置
        rectTransform.position = screenPoint + screenOffset;

        // 处理物体在屏幕外的情况
        if (screenPoint.z < 0)
        {
            rectTransform.gameObject.SetActive(false);
        }
        else
        {
            rectTransform.gameObject.SetActive(true);
        }
    }
}