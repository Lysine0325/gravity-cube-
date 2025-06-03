using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PressurePlateGroup : MonoBehaviour
{
    [Tooltip("绑定到同一机关的所有压力板")]
    public List<压力板> pressurePlates = new List<压力板>();

    [Tooltip("当组内任意压力板激活时触发")]
    public UnityEvent OnGroupActivated;

    [Tooltip("当组内所有压力板均松开时触发")]
    public UnityEvent OnGroupDeactivated;

    private int activePlateCount = 0;

    void Start()
    {
        foreach (var plate in pressurePlates)
        {
            // 监听每个压力板的激活/取消事件
            plate.OnActivate.AddListener(HandlePlateActivated);
            plate.OnDeactivate.AddListener(HandlePlateDeactivated);
        }
    }

    private void HandlePlateActivated()
    {
        activePlateCount++;
        if (activePlateCount == 1) // 首次激活时触发
        {
            OnGroupActivated.Invoke();
        }
    }

    private void HandlePlateDeactivated()
    {
        activePlateCount--;
        if (activePlateCount == 0) // 全部松开时触发
        {
            OnGroupDeactivated.Invoke();
        }
    }

    void OnDestroy()
    {
        // 清理监听
        foreach (var plate in pressurePlates)
        {
            plate.OnActivate.RemoveListener(HandlePlateActivated);
            plate.OnDeactivate.RemoveListener(HandlePlateDeactivated);
        }
    }
}