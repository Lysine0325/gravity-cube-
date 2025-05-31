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

    private GameObject 当前目标物体;
    private GameObject 上一个高亮物体;
    private bool 正在控制 = false;
    private PlayerController1 玩家控制器;
    private CameraController 相机控制器;

    void Start()
    {
        玩家控制器 = GetComponent<PlayerController1>();
        相机控制器 = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        if (!拥有引力器)
        {
            清除提示();
            关闭高亮();
            当前目标物体 = null;
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

            if (Input.GetKeyDown(交互键))
            {
                停止控制();
            }
        }
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
    }

    void 停止控制()
    {
        正在控制 = false;
        玩家控制器.enabled = true;

        当前目标物体 = null;
        清除提示();
        相机控制器.target = this.transform;
    }

    void 控制物体移动()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;

        Vector3 finalMove = cameraRight.normalized * h + cameraForward.normalized * v;

        if (当前目标物体 != null)
        {
            当前目标物体.transform.Translate(finalMove * 移动速度 * Time.deltaTime, Space.World);
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
