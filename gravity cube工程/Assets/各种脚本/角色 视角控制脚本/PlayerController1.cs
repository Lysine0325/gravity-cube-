using UnityEngine;
using TMPro; // Don't forget to import TextMeshPro namespace

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
    public float ladderLateralSpeed = 2f;
    [Header("音效")]
    public AudioSource walkAudioSource; // 角色的音效源（需要拖入）
    public AudioClip walkSound; // 走路音效

    private bool isWalking = false; // 控制音效播放的状态

    // TextMeshPro UI for climbing instructions
    [Header("UI Text")]
    public TextMeshProUGUI climbingInstructionsText; // Drag the TMP text component here

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (climbingInstructionsText != null)
        {
            climbingInstructionsText.enabled = false; // Hide the text at start
        }
    }

    void Update()
    {
        if (isClimbing)
        {
            HandleClimbing();
            // Show climbing instructions
            if (climbingInstructionsText != null)
            {
                climbingInstructionsText.text = "按 空格键 脱离爬梯";
                climbingInstructionsText.enabled = true;
            }
        }
        else
        {
            HandleWalking();
            // Hide climbing instructions when not climbing
            if (climbingInstructionsText != null)
            {
                climbingInstructionsText.enabled = false;
            }
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
        float horizontal = Input.GetAxis("Horizontal"); // 新增：获取水平输入

        // 新增：计算左右移动方向
        Vector3 lateralDirection = Vector3.zero;
        if (currentLadder != null)
        {
            // 使用梯子的右方向作为水平移动基准
            lateralDirection = currentLadder.right;
        }

        // 组合移动向量：上下移动 + 左右移动
        Vector3 move = (climbDirection * vertical * climbSpeed) +
                      (-lateralDirection * horizontal * ladderLateralSpeed);

        controller.Move(move * Time.deltaTime);

        // 传递速度给动画 - 同时考虑垂直和水平移动
        float combinedSpeed = Mathf.Max(Mathf.Abs(vertical), Mathf.Abs(horizontal));
        animator.SetFloat("Speed", combinedSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ExitLadder();
        }
    }

    // 触碰梯子启动攀爬的逻辑
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            EnterLadder(other.transform);
        }
    }

    // 确认结束攀爬的过程
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

        Vector3 pos = transform.position;
        transform.position = new Vector3(ladder.position.x, pos.y, ladder.position.z);
        velocity = Vector3.zero;

        // 让角色面向梯子
        transform.forward = -ladder.forward;

        animator.SetBool("IsClimbing", true);
    }

    void ExitLadder()
    {
        isClimbing = false;
        currentLadder = null;
        animator.SetBool("IsClimbing", false);

        if (climbingInstructionsText != null)
        {
            climbingInstructionsText.enabled = false;
        }
    }
}

