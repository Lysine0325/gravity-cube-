using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class PlayerTriggerEvent : MonoBehaviour
{
    [Header("触发设置")]
    public bool 只触发一次 = true;
    public UnityEvent 触发事件;

    private bool 已触发 = false;

    void OnTriggerEnter(Collider other)
    {
        if (已触发 && 只触发一次) return;

        if (other.CompareTag("Player"))
        {
            触发事件?.Invoke();
            已触发 = true;
        }
    }
}
