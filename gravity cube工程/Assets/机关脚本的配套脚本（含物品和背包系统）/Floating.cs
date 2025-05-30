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
        startPos = transform.position;
    }

    void Update()
    {
        // 旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // 上下浮动
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);
    }
}