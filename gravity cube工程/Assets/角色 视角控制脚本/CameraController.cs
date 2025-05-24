using UnityEngine;

/// <summary>
/// �����������������
/// - �������϶�����ˮƽ��ת
/// - �������������� offset��ʵ����Ȼ����
/// - ���ʼ�տ�������Ŀ��
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Ŀ�����ʼ����")]
    public Transform target;                        // ��ͷʼ��Χ�Ƶ�����
    public Vector3 baseOffset = new Vector3(0, 5, -10); // �����ʼ���λ��

    [Header("���Ʋ���")]
    public float rotationSpeed = 45f;               // O/P ������ת�ٶ�
    public float dragSpeed = 20f;                    // ��������ת������
    public float zoomSpeed = 25f;                    // ���������ȣ�����СһЩ��
    public float minZoom = 1f;                    // ��С��������
    public float maxZoom = 2.0f;                    // �����������

    [Range(0.5f, 2.0f)]
    public float zoomFactor = 1.5f;                   // ��ǰ����ϵ��


    private float currentAngle = 0f;                // ��ǰ Y ��Ƕ� 
    private Vector2 lastMousePos;

    void Start()
    {
        if (target == null)
            Debug.LogError("�뽫 target ����Ϊ����۲�����ĵ㣡");

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
    /// O / P ������ˮƽ��ת
    /// </summary>
    void HandleKeyRotation()
    {
        float direction = 0f;
        if (Input.GetKey(KeyCode.O)) direction = -1f;
        else if (Input.GetKey(KeyCode.P)) direction = 1f;

        currentAngle += direction * rotationSpeed * Time.deltaTime;
    }

    /// <summary>
    /// ����������ˮƽ��ת
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
    /// ��������������ƫ��
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoomFactor -= scroll * zoomSpeed * Time.deltaTime;
        zoomFactor = Mathf.Clamp(zoomFactor, minZoom, maxZoom);
    }

    /// <summary>
    /// ���ݽǶ������Ÿ������λ��
    /// </summary>
    void UpdateCameraPosition()
    {
        // ��ת���� offset���������������ӷŴ�/��С
        Vector3 rotatedOffset = Quaternion.Euler(0, currentAngle, 0) * baseOffset;
        Vector3 zoomedOffset = rotatedOffset * zoomFactor;

        transform.position = target.position + zoomedOffset;
        transform.LookAt(target.position);



    }

}
