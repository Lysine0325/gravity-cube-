using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController1 : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float gravity = -9.8f;

    private CharacterController controller;
    private Vector3 velocity;

    //爬梯
    [Header("爬梯参数")]
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private Vector3 climbDirection = Vector3.up;
    private Transform currentLadder;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isClimbing)
        {
            HandleClimbing();
        }
        else
        {
            HandleWalking();
        }
    }

    void HandleWalking()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(h, 0, v);
        Vector3 move = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * input;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleClimbing()
    {
        velocity = Vector3.zero;

        float vertical = Input.GetAxis("Vertical");
        Vector3 move = climbDirection * vertical * climbSpeed;

        controller.Move(move * Time.deltaTime);

        //  可选：按下空格退出爬梯
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExitLadder();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            EnterLadder(other.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            ExitLadder();
        }
    }

    void EnterLadder(Transform ladder)
    {
        isClimbing = true;
        currentLadder = ladder;
        climbDirection = ladder.up;

        //  简洁贴近梯子，只重设 X/Z，不乱动 Y
        Vector3 pos = transform.position;
        transform.position = new Vector3(ladder.position.x, pos.y, ladder.position.z);
        velocity = Vector3.zero;
    }

    void ExitLadder()
    {
        isClimbing = false;
        currentLadder = null;
    }
}
