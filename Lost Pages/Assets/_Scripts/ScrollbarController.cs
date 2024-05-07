using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollbarController : MonoBehaviour
{
    public Scrollbar scrollbar;

    void Start()
    {
        scrollbar = InventoryManager.Instance.pageScrollbar;
    }
    void Update()
    {
        if (InventoryManager.Instance.pagesCollection.activeSelf)
        {
            scrollbar = InventoryManager.Instance.pageScrollbar;
        }
        if (InventoryManager.Instance.settingsMenu.activeSelf)
        {
            scrollbar = InventoryManager.Instance.settingsScrollbar;
        }
        if (InventoryManager.Instance.pageInspection.activeSelf)
        {
            scrollbar = InventoryManager.Instance.inspectPageScrollbar;
        }


        // Scroll with mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            scrollbar.value += scroll * 0.1f;
        }

        // Scroll with up/down arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
        {
            scrollbar.value += 0.001f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            scrollbar.value -= 0.001f;
        }
    }
}
