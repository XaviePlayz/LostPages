using UnityEngine;
using UnityEngine.UI;

public class ScrollbarController : MonoBehaviour
{
    #region Singleton

    private static ScrollbarController _instance;
    public static ScrollbarController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScrollbarController>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(ScrollbarController).Name;
                    _instance = obj.AddComponent<ScrollbarController>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public ScrollRect scrollRect;
    public float scrollSpeed = 500f;
    public float mouseScrollSensitivity = 0.5f;

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

        // Scroll with up/down arrow keys or W/S keys
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition + scrollAmount, 0f, 1f);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition - scrollAmount, 0f, 1f);
        }

        // Scroll with mouse wheel
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            float fixedMouseScrollAmount = mouseScroll * mouseScrollSensitivity;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollRect.verticalNormalizedPosition + fixedMouseScrollAmount, 0f, 1f);
        }
    }
}