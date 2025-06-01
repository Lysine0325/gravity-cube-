using UnityEngine;
using TMPro;
using cakeslice;

public class 引力器 : MonoBehaviour
{
    [Header("基础设置")]
    public bool 拥有引力器 = false;
    public float 控制范围 = 5f;
    public float 移动速度 = 3f;
    public KeyCode 交互键 = KeyCode.J;
    public string 可控Tag = "能触机关的";
    public float 最近高亮距离 = 5f;

    [Header("UI 提示")]
    public TextMeshProUGUI 提示文字;

    [Header("激光设置")]
    public LineRenderer 激光线;  // 激光线
    public float 激光线宽度 = 0.05f;  // 激光宽度
    public float 扭曲幅度 = 0.2f;  // 激光扭曲幅度
    public float 扭曲速度 = 5f;  // 激光扭曲速度
    public Color 激光颜色 = new Color(0.5f, 0.8f, 1f);  // 浅蓝色激光

    private GameObject 当前目标物体;
    private GameObject 上一个高亮物体;
    private bool 正在控制 = false;
    private PlayerController1 玩家控制器;
    private CameraController 相机控制器;

    void Start()
    {
        玩家控制器 = GetComponent<PlayerController1>();
        相机控制器 = Camera.main.GetComponent<CameraController>();

        // 如果没有手动设置激光线，自动添加 LineRenderer 组件
        if (激光线 == null)
        {
            激光线 = gameObject.AddComponent<LineRenderer>();
            激光线.startWidth = 激光线.endWidth = 激光线宽度;
            激光线.material = new Material(Shader.Find("Sprites/Default"));
            激光线.startColor = 激光线.endColor = 激光颜色;  // 浅蓝色激光
            激光线.positionCount = 2;  // 设置两个点：起点（角色）和终点（目标物体）
            激光线.enabled = false;  // 默认禁用激光线
        }
    }

    void Update()
    {
        if (!拥有引力器)
        {
            清除提示();
            关闭高亮();
            当前目标物体 = null;
            激光线.enabled = false;  // 没有引力器时，禁用激光线
            return;
        }

        if (!正在控制)
        {
            查找并高亮最近目标();

            if (当前目标物体 != null)
            {
                设置提示("按 J 控制该物体");

                if (Input.GetKeyDown(交互键))
                {
                    开始控制();
                }
            }
            else
            {
                清除提示();
            }
        }
        else
        {
            控制物体移动();
            更新激光线();  // 更新激光线位置，加入不规则扭曲效果

            if (Input.GetKeyDown(交互键))
            {
                停止控制();
            }
        }
    }
    public bool 正在控制状态()
    {
        return 正在控制;
    }

    void 查找并高亮最近目标()
    {
        GameObject[] 所有目标 = GameObject.FindGameObjectsWithTag(可控Tag);
        float 最近距离 = 最近高亮距离;
        GameObject 最近目标 = null;

        foreach (var 物体 in 所有目标)
        {
            float dist = Vector3.Distance(transform.position, 物体.transform.position);
            if (dist <= 最近高亮距离 && dist < 最近距离)
            {
                最近距离 = dist;
                最近目标 = 物体;
            }
        }

        当前目标物体 = 最近目标;

        foreach (var 物体 in 所有目标)
        {
            var outline = 物体.GetComponent<Outline>();
            if (outline == null) continue;

            // 高亮最近目标，关闭其他目标
            bool 应高亮 = (物体 == 最近目标);
            outline.enabled = 应高亮;
        }

        上一个高亮物体 = 最近目标;
    }

    void 开始控制()
    {
        正在控制 = true;
        玩家控制器.enabled = false;

        设置提示("控制中：WASD移动，J 退出");

        相机控制器.target = 当前目标物体.transform;
        激光线.enabled = true;  // 启用激光线
    }

    void 停止控制()
    {
        正在控制 = false;
        玩家控制器.enabled = true;

        当前目标物体 = null;
        清除提示();
        相机控制器.target = this.transform;
        激光线.enabled = false;  // 停止控制时禁用激光线
    }

    void 控制物体移动()
    {
        // 获取输入的移动方向
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;  // 保持水平面
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;  // 保持水平面

        // 计算最终的移动方向
        Vector3 finalMove = cameraRight.normalized * h + cameraForward.normalized * v;

        // 如果当前目标物体存在，则移动它
        if (当前目标物体 != null)
        {
            当前目标物体.transform.Translate(finalMove * 移动速度 * Time.deltaTime, Space.World);

            // 让角色朝向被控制物体
            Vector3 targetPosition = 当前目标物体.transform.position;
            targetPosition.y = transform.position.y;  // 保持角色的Y轴不变
            transform.LookAt(targetPosition);  // 让角色面向目标
        }
    }

    // 更新激光线
    void 更新激光线()
    {
        if (当前目标物体 != null)
        {
            Vector3 startPos = transform.position;  // 角色位置
            Vector3 endPos = 当前目标物体.transform.position;  // 被控制物体位置

            激光线.SetPosition(0, startPos);  // 设置起点（角色位置）
            激光线.SetPosition(1, endPos);  // 设置终点（目标物体位置）

            // 添加不规则噪声模拟扭曲效果（电流效果）
            int pointCount = 20;  // 激光线的点数
            激光线.positionCount = pointCount;
            for (int i = 0; i < pointCount; i++)
            {
                float t = (float)i / (pointCount - 1);
                float offset = Mathf.Sin(Time.time * 扭曲速度 + i * Random.Range(0.5f, 1f)) * 扭曲幅度;  // 引入随机化的偏移
                Vector3 position = Vector3.Lerp(startPos, endPos, t);
                position += new Vector3(0, offset, 0);  // 在Y轴方向上添加噪声偏移，模拟扭曲
                激光线.SetPosition(i, position);
            }
        }
    }

    void 设置提示(string 文本)
    {
        if (提示文字 != null)
        {
            提示文字.text = 文本;
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

    void 关闭高亮()
    {
        GameObject[] 所有目标 = GameObject.FindGameObjectsWithTag(可控Tag);
        foreach (var 物体 in 所有目标)
        {
            var outline = 物体.GetComponent<Outline>();
            if (outline != null) outline.enabled = false;
        }

        上一个高亮物体 = null;
    }
}
