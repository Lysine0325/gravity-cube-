using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController1 : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;
    public float gravity = -9.8f;
    public Animator animator;  // 拖入你“模型”上的 Animator

    private CharacterController controller;
    private Vector3 velocity;

    //爬梯
    [Header("爬梯参数")]
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private Vector3 climbDirection = Vector3.up;
    private Transform currentLadder;

    [Header("音效")]
    public AudioSource walkAudioSource; // 角色的音效源（需要拖入）
    public AudioClip walkSound; // 走路音效

    private bool isWalking = false; // 控制音效播放的状态

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

        // 控制角色朝向
        if (move != Vector3.zero)
            transform.forward = new Vector3(move.x, 0, move.z);

        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else
            velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        //计算速度大小传给 Animator
        float speed = new Vector3(move.x, 0, move.z).magnitude;
        animator.SetFloat("Speed", speed);

        // 只有在角色移动时播放音效
        if (speed > 0 && !isWalking)
        {
            isWalking = true;
            if (walkAudioSource != null && walkSound != null)
            {
                walkAudioSource.clip = walkSound;
                walkAudioSource.Play();
            }
        }
        else if (speed == 0 && isWalking)
        {
            isWalking = false;
            if (walkAudioSource != null)
            {
                walkAudioSource.Stop();
            }
        }
    }

    //攀爬过程的逻辑，此时只有上下（ws）有相关的判断逻辑
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

        animator.SetFloat("Speed", Mathf.Abs(vertical));

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExitLadder();
        }
    }

    //触碰梯子启动攀爬的逻辑
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))//检测是否是Ladder标签
        {
            EnterLadder(other.transform);
        }
    }

    //确认结束攀爬的过程
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            ExitLadder();
        }
    }

    //启动攀爬之后的基本设置，要把ladder的参数传进去
    void EnterLadder(Transform ladder)
    {
        isClimbing = true;
        currentLadder = ladder;
        climbDirection = ladder.up;

        Vector3 pos = transform.position;
        transform.position = new Vector3(ladder.position.x, pos.y, ladder.position.z);
        velocity = Vector3.zero;

        animator.SetBool("IsClimbing", true); // 
    }

    //结束攀爬的设置
    void ExitLadder()
    {
        isClimbing = false;
        currentLadder = null;

        animator.SetBool("IsClimbing", false);
    }
}
