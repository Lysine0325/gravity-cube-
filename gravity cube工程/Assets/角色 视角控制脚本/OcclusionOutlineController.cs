using UnityEngine;
using cakeslice;

[RequireComponent(typeof(Renderer))]
public class OcclusionOutlineController : MonoBehaviour
{
    private Transform target;              // ������
    private Outline outline;              // ������Ӷ����ϵ� Outline
    public LayerMask obstacleMask = ~0;   // ����������壨Ĭ�ϣ����Զ���

    void Start()
    {
        target = transform;

        // �Զ�Ѱ�� Outline ���������������壩
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = GetComponentInChildren<Outline>();

        if (outline == null)
        {
            Debug.LogWarning($"[AutoOcclusionOutline] δ�ҵ� Outline �����{name}");
        }
    }

    void Update()
    {
        if (outline == null || Camera.main == null) return;

        Vector3 dir = target.position - Camera.main.transform.position;
        Ray ray = new Ray(Camera.main.transform.position, dir.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, dir.magnitude, obstacleMask))
        {
            // ���ڵ���
            outline.enabled = true;
        }
        else
        {
            //���ڵ�
            outline.enabled = false;
        }
    }
}
