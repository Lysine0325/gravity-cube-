using UnityEngine;

/// <summary>
/// 升级版相机控制器：
/// - 鼠标左键拖动控制水平旋转
/// - 鼠标滚轮缩放整体 offset，实现自然推拉
/// - 相机始终看着中心目标
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("目标与初始设置")]
    public Transform target;                        // 镜头始终围绕的中心
    public Vector3 baseOffset = new Vector3(0, 5, -10); // 相机起始相对位置

    [Header("控制参数")]
    public float rotationSpeed = 45f;               // O/P 键盘旋转速度
    public float dragSpeed = 20f;                    // 鼠标左键旋转灵敏度
    public float zoomSpeed = 25f;                    // 缩放灵敏度（建议小一些）
    public float minZoom = 1f;                    // 最小缩放因子
    public float maxZoom = 2.0f;                    // 最大缩放因子

    [Range(0.5f, 2.0f)]
    public float zoomFactor = 1.5f;                   // 当前缩放系数


    private float currentAngle = 0f;                // 当前 Y 轴角度 
    private Vector2 lastMousePos;

    void Start()
    {
        if (target == null)
            Debug.LogError("请将 target 设置为相机观察的中心点！");

        UpdateCameraPosition();
    }

    void Update()
    {
        HandleKeyRotation();
        HandleMouseHorizontalDrag();
        HandleZoom();
        UpdateCameraPosition();

    }

    /// <summary>
    /// O / P 键控制水平旋转
    /// </summary>
    void HandleKeyRotation()
    {
        float direction = 0f;
        if (Input.GetKey(KeyCode.O)) direction = -1f;
        else if (Input.GetKey(KeyCode.P)) direction = 1f;

        currentAngle += direction * rotationSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 鼠标左键控制水平旋转
    /// </summary>
    void HandleMouseHorizontalDrag()
    {
        if (Input.GetMouseButtonDown(0))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            currentAngle += delta.x * dragSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// 鼠标滚轮缩放整体偏移
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomFactor -= scroll * zoomSpeed * Time.deltaTime;
        zoomFactor = Mathf.Clamp(zoomFactor, minZoom, maxZoom);
    }

    /// <summary>
    /// 根据角度与缩放更新相机位置
    /// </summary>
    void UpdateCameraPosition()
    {
        // 旋转基础 offset，并根据缩放因子放大/缩小
        Vector3 rotatedOffset = Quaternion.Euler(0, currentAngle, 0) * baseOffset;
        Vector3 zoomedOffset = rotatedOffset * zoomFactor;

        transform.position = target.position + zoomedOffset;
        transform.LookAt(target.position);



    }

}
