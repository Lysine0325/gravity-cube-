using UnityEngine;

public class Floating : MonoBehaviour
{
    [Header("旋转参数")]
    public float rotationSpeed = 50f;

    [Header("浮动参数")]
    public float floatAmplitude = 0.15f;  // 上下浮动幅度
    public float floatFrequency = 1f;     // 浮动频率

    private Vector3 startPos;

    void Start()
    {
        // 记录初始相对父级的位置
        startPos = transform.localPosition;
    }

    void Update()
    {
        // 旋转，绕着物体自身的Y轴进行旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        // 上下浮动
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        // 使用 localPosition 保证物体相对于父物体的位置
        transform.localPosition = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);
    }
}