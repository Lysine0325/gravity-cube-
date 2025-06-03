using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform target;  // 目标点 (相对父级物体的终点位置)
    public float moveSpeed = 1f;  // 平台的移动速度
    public AudioSource movingAudioSource;  // 用于播放位移音效的音频源
    public AudioClip movingSound;  // 位移时播放的音效

    private Vector3 startPosition;  // 起始位置 (相对父级物体的起点)
    private bool isActive = false;  // 是否激活平移
    private Vector3 endPosition;  // 终点位置
    private bool isMoving = false;  // 是否正在移动

    private void Start()
    {
        // 记录平台的初始位置
        startPosition = transform.localPosition;
        endPosition = target.localPosition;

        // 确保音效初始状态是停止的
        if (movingAudioSource != null)
        {
            movingAudioSource.Stop();
        }
    }

    private void Update()
    {
        if (isActive)
        {
            // 如果平台处于激活状态，平移到终点
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, endPosition, moveSpeed * Time.deltaTime);

            // 判断是否正在移动，如果是，播放音效
            if (!isMoving)
            {
                isMoving = true;
                PlayMovingSound();
            }
        }
        else
        {
            // 如果平台处于非激活状态，平移回起点
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPosition, moveSpeed * Time.deltaTime);

            // 判断是否正在移动，如果是，播放音效
            if (!isMoving)
            {
                isMoving = true;
                PlayMovingSound();
            }
        }

        // 如果平台到达起点或终点，停止音效
        if (transform.localPosition == startPosition || transform.localPosition == endPosition)
        {
            if (isMoving)
            {
                isMoving = false;
                StopMovingSound();
            }
        }
    }

    // 激活平移
    public void Activate()
    {
        isActive = true;
    }

    // 停用平移
    public void Deactivate()
    {
        isActive = false;
    }

    // 播放位移音效
    private void PlayMovingSound()
    {
        if (movingAudioSource != null && movingSound != null)
        {
            if (!movingAudioSource.isPlaying)
            {
                movingAudioSource.clip = movingSound;
                movingAudioSource.loop = true;  // 设置音效循环播放
                movingAudioSource.Play();
            }
        }
    }

    // 停止位移音效
    private void StopMovingSound()
    {
        if (movingAudioSource != null)
        {
            movingAudioSource.Stop();
        }
    }
}
