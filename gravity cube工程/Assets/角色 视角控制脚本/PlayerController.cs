using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("摄像机设置")]
    public Transform cameraTransform;
    public float deadZoneThreshold = 0.1f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 死区过滤
        if (Mathf.Abs(h) < deadZoneThreshold) h = 0;
        if (Mathf.Abs(v) < deadZoneThreshold) v = 0;

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            // 根据摄像机方向移动
            Vector3 moveDir = cameraTransform.forward * inputDir.z + cameraTransform.right * inputDir.x;
            moveDir.y = 0;
            moveDir.Normalize();

            controller.Move(moveDir * moveSpeed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 稳定贴地
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
