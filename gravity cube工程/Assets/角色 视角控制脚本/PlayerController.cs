using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.8f;
    public Transform cameraTransform;
    public float rotationSpeed = 10f; // ת���ٶ�

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v);
        Vector3 moveDir = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0) * inputDir;

        // ʵ���ƶ�
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // ������ת�߼�
        if (moveDir.magnitude > 0.1f)//moveDir.magnitude ���ƶ������ġ����ȡ����ж��Ƿ����ƶ�
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);//Quaternion.LookRotation() ��������һ������ָ���������ת
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
