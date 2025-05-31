using UnityEngine;
using TMPro;

public class StarCollector : MonoBehaviour
{
    public int totalStars = 0;

    public TextMeshProUGUI starText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Star"))
        {
            Destroy(other.gameObject);
            totalStars++;
            UpdateUI();

        }
    }

    void UpdateUI()
    {
        starText.text = $"Stars: {totalStars}";
        
    }
}