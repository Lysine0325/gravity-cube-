using UnityEngine;

public class ClickInteractable : MonoBehaviour
{
    [Header("高亮设置")]
    public Outline outline;
    public Color highlightColor = Color.yellow;

    [Header("提示UI预制体")]
    public GameObject clickPromptPrefab;  // 拖入预制体文件，而非场景实例

    private GameObject currentPrompt;     // 当前生成的UI实例

    void Start()
    {
        outline.OutlineColor = highlightColor;
        outline.enabled = false;
    }

    void OnMouseEnter()
    {
        if (StateModuleA.IsSystemActive)
        {
            currentPrompt = Instantiate(clickPromptPrefab);
            WorldSpaceFollow follow = currentPrompt.GetComponent<WorldSpaceFollow>();
            follow.target = transform;
            follow.screenOffset = new Vector3(0, 50, 0); // 使用正确变量名
        }
    }

    void OnMouseExit()
    {
        outline.enabled = false;
        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
            currentPrompt = null;
        }
    }

    void OnMouseDown()
    {
        if (StateModuleA.IsSystemActive) OpenChest();
    }

    void OpenChest()
    {
        Debug.Log("宝箱已打开！获得金币 x10");
    }
}