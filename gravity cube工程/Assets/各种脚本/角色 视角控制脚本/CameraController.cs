using UnityEngine;

/// <summary>
/// 升级版相机控制器：
/// - 鼠标左键拖动控制水平和垂直旋转
/// - 鼠标滚轮和小键盘加减号缩放视角
/// - 相机始终看着中心目标
/// - 小键盘上的 1, 2, 3, 5 键控制相机的旋转
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("目标与初始设置")]
    public Transform target;                        // 镜头始终围绕的中心
    public Vector3 baseOffset = new Vector3(0, 4, -8); // 相机起始相对位置

    [Header("控制参数")]
    public float rotationSpeed = 90f;               // O/P 键盘旋转速度
    public float dragSpeed = 20f;                    // 鼠标左键旋转灵敏度
    public float zoomSpeed = 25f;                    // 缩放灵敏度（建议小一些）
    public float minZoom = 0.5f;                    // 最小缩放因子
    public float maxZoom = 1f;                    // 最大缩放因子

    [Range(0.5f, 2.0f)]
    public float zoomFactor = 0.7f;                   // 当前缩放系数

    [Header("上下旋转限制")]
    public float minVerticalAngle = -30f;            // 最小垂直角度
    public float maxVerticalAngle = 50f;             // 最大垂直角度

    private float currentAngle = 0f;                // 当前 Y 轴角度
    private float currentVerticalAngle = 0f;        // 当前 X 轴（垂直）角度
    private Vector2 lastMousePos;

    //与过度动画有关
    private Transform currentTarget;
    private bool isTransitioning = false;
    private float transitionTimer = 0f;
    private float transitionDuration = 0.5f; // 控制过渡持续时间
    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        if (target == null)
            Debug.LogError("请将 target 设置为相机观察的中心点！");

        UpdateCameraPosition();
    }

    void Update()
    {
        HandleKeyRotation();
        HandleMouseDrag();
        HandleZoom();

        if (target != currentTarget)
        {
            // 目标改变 → 开始平滑过渡
            currentTarget = target;
            isTransitioning = true;
            transitionTimer = 0f;

            // 起始位置记录
            startPos = transform.position;
        }

        UpdateCameraPosition();
    }

    /// <summary>
    /// O / P 键控制水平旋转
    /// 小键盘上的 1、3 键控制水平旋转
    /// 小键盘上的 2、5 键控制垂直旋转
    /// </summary>
    void HandleKeyRotation()
    {
        float direction = 0f;
        float verticalDirection = 0f;

        // O/P 键控制水平旋转
        if (Input.GetKey(KeyCode.O)) direction = -1f;
        else if (Input.GetKey(KeyCode.P)) direction = 1f;

        // 小键盘 1, 3 控制水平旋转
        if (Input.GetKey(KeyCode.Keypad1)) direction = -1f;  // 向左旋转
        else if (Input.GetKey(KeyCode.Keypad3)) direction = 1f; // 向右旋转

        // 小键盘 2, 5 控制垂直旋转
        if (Input.GetKey(KeyCode.Keypad2)) verticalDirection = -1f; // 向下旋转
        else if (Input.GetKey(KeyCode.Keypad5)) verticalDirection = 1f; // 向上旋转

        // 更新水平旋转角度
        currentAngle += direction * rotationSpeed * Time.deltaTime;

        // 垂直旋转，限制垂直角度，且速度较慢
        currentVerticalAngle += verticalDirection * rotationSpeed * 0.5f * Time.deltaTime; // 缩小垂直旋转速度
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
    }

    /// <summary>
    /// 鼠标左键控制水平和垂直旋转
    /// </summary>
    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
            lastMousePos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            // 水平旋转
            currentAngle += delta.x * dragSpeed * Time.deltaTime;

            // 垂直旋转，限制垂直角度
            currentVerticalAngle -= delta.y * dragSpeed * Time.deltaTime;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
        }
    }

    /// <summary>
    /// 鼠标滚轮和小键盘加减号控制整体偏移缩放
    /// </summary>
    void HandleZoom()
    {
        // 鼠标滚轮控制缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomFactor -= scroll * zoomSpeed * Time.deltaTime;

        // 小键盘加号减号控制缩放
        if (Input.GetKey(KeyCode.KeypadPlus)) zoomFactor -= 0.03f * zoomSpeed * Time.deltaTime; // 放大
        if (Input.GetKey(KeyCode.KeypadMinus)) zoomFactor += 0.03f * zoomSpeed * Time.deltaTime; // 缩小

        zoomFactor = Mathf.Clamp(zoomFactor, minZoom, maxZoom);
    }

    /// <summary>
    /// 根据角度与缩放更新相机位置
    /// </summary>
    void UpdateCameraPosition()
    {
        // 旋转基础 offset，并根据缩放因子放大/缩小
        Vector3 rotatedOffset = Quaternion.Euler(currentVerticalAngle, currentAngle, 0) * baseOffset;
        Vector3 zoomedOffset = rotatedOffset * zoomFactor;
        Vector3 desiredPos = target.position + zoomedOffset;

        //如果是物体切换
        if (isTransitioning)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);
            transform.position = Vector3.Lerp(startPos, desiredPos, t);

            if (t >= 1f) isTransitioning = false;
        }
        else
        {
            transform.position = desiredPos;
        }

        transform.LookAt(target.position);
    }
}
