using UnityEngine;

public class 落水音效: MonoBehaviour
{
    // 音效源组件
    private AudioSource audioSource;

    // 落水音效的音频文件
    public AudioClip waterSplashSound;

    // 判断角色是否在水中
    private bool isInWater = false;

    void Start()
    {
        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();

        // 确保音效源已准备好
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 设置音效源不在播放时的音量
        audioSource.volume = 1.0f;
    }

    void OnTriggerEnter(Collider other)
    {
        // 检测角色是否进入水中
        if (other.CompareTag("Water") && !isInWater)
        {
            // 播放落水音效
            audioSource.PlayOneShot(waterSplashSound);
            isInWater = true; // 标记角色已经进入水中
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 检测角色是否离开水中
        if (other.CompareTag("Water"))
        {
            isInWater = false; // 标记角色离开水中
        }
    }
}
