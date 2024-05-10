using UnityEngine;
using UnityEngine.UI;

public class ScrollbarController : MonoBehaviour
{
    public ScrollRect scrollRect; // Use ScrollRect instead of Scrollbar
    public float scrollSpeed = 100f; // Adjust this value to change the scrolling speed

    public ScrollRect pageCollection;
    public ScrollRect pageInspect;

    void Start()
    {
        scrollRect = pageCollection;
    }
    void Update()
    {
        if (InventoryManager.Instance.pagesCollection.activeSelf)
        {
            scrollRect = pageCollection;
        }
        if (InventoryManager.Instance.pageInspection.activeSelf)
        {
            scrollRect = pageInspect;
        }

        // Calculate the scroll amount based on the actual size of the content
        float scrollAmount = scrollSpeed * Time.deltaTime / scrollRect.content.rect.height;

        // Scroll with up/down arrow keys
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition + scrollAmount, 0f, 1f);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition - scrollAmount, 0f, 1f);
        }
    }
}