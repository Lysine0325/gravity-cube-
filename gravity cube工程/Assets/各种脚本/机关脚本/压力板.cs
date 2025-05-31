using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class 压力板 : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("需要触发机关的物体标签（空表示所有物体）")]
    public List<string> triggerTags = new List<string> { "Player", "能触机关的" };
    [Tooltip("触发类型：需要一定重量触发 或 任意物体触发")]
    public TriggerType triggerType = TriggerType.AnyObject;

    [Header("Controlled Mechanisms")]
    [Tooltip("直接控制的平移平台（可选）")]
    public 平移平台[] platformTargets;
    [Tooltip("通用触发事件（可连接其他机关）")]
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    [Header("Visual Feedback")]
    [Tooltip("按下时的下沉距离")]
    public float pressDownDistance = 0.1f;
    [Tooltip("按下时的颜色变化")]
    public Color pressedColor = Color.green;
    public float colorChangeSpeed = 5f;

    [Header("Audio Feedback")]
    public AudioClip pressSound;
    public AudioClip releaseSound;

    private HashSet<Collider> activePressers = new HashSet<Collider>();
    private Vector3 originalPosition;
    private Renderer plateRenderer;
    private Color originalColor;
    private AudioSource audioSource;
    private bool isActivated = false;

    public enum TriggerType { AnyObject, WeightBased }

    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        originalPosition = transform.position;
        plateRenderer = GetComponentInChildren<Renderer>();
        originalColor = plateRenderer.material.color;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        UpdateVisualState();
    }

    void UpdateVisualState()
    {
        // 平滑颜色过渡
        Color targetColor = isActivated ? pressedColor : originalColor;
        plateRenderer.material.color = Color.Lerp(
            plateRenderer.material.color,
            targetColor,
            colorChangeSpeed * Time.deltaTime
        );

        // 平滑位置移动
        Vector3 targetPosition = originalPosition + (isActivated ? Vector3.down * pressDownDistance : Vector3.zero);
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            colorChangeSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsValidTrigger(other))
        {
            activePressers.Add(other);
            UpdateActivationState();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (activePressers.Contains(other))
        {
            activePressers.Remove(other);
            UpdateActivationState();
        }
    }

    bool IsValidTrigger(Collider other)
    {
        // 标签检查
        if (triggerTags.Count > 0 && !triggerTags.Contains(other.tag))
            return false;

        // 重量类型检查
        if (triggerType == TriggerType.WeightBased)
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb == null || rb.mass < 0.1f)
                return false;
        }

        return true;
    }

    void UpdateActivationState()
    {
        bool shouldActivate = activePressers.Count > 0;

        if (shouldActivate != isActivated)
        {
            isActivated = shouldActivate;
            PlaySoundEffect(shouldActivate);
            NotifyMechanisms(shouldActivate);
        }
    }

    void PlaySoundEffect(bool activating)
    {
        if (audioSource == null) return;

        AudioClip clip = activating ? pressSound : releaseSound;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void NotifyMechanisms(bool activating)
    {
        // 控制平移平台
        foreach (var platform in platformTargets)
        {
            if (activating)
                platform.Activate();
            else
                platform.Deactivate();
        }

        // 触发通用事件
        if (activating)
            OnActivate.Invoke();
        else
            OnDeactivate.Invoke();
    }

    void OnDrawGizmos()
    {
        BoxCollider collider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(collider.center, collider.size);
    }
}