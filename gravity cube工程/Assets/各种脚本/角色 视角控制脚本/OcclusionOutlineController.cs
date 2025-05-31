using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Renderer))]
public class OcclusionOutlineController : MonoBehaviour
{
    private Transform target;              // 本对象
    private Outline outline;              // 自身或子对象上的 Outline
    public LayerMask obstacleMask = ~0;   // 检测所有物体（默认）可自定义

    void Start()
    {
        target = transform;

        // 自动寻找 Outline 组件（自身或子物体）
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = GetComponentInChildren<Outline>();

        if (outline == null)
        {
            Debug.LogWarning($"[AutoOcclusionOutline] 未找到 Outline 组件：{name}");
        }
    }

    void Update()
    {
        if (outline == null || Camera.main == null) return;

        Vector3 dir = target.position - Camera.main.transform.position;
        Ray ray = new Ray(Camera.main.transform.position, dir.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude, obstacleMask))
        {
            // 有遮挡物
            outline.enabled = true;
        }
        else
        {
            //无遮挡
            outline.enabled = false;
        }
    }
}
