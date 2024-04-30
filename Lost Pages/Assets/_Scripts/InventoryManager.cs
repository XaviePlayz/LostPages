using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    #region Singleton

    private static InventoryManager _instance;
    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(InventoryManager).Name;
                    _instance = obj.AddComponent<InventoryManager>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public GameObject inventoryCanvas;
    public bool hasAccessToInventory;
    public Color currentViewButtonColor;
    public Color canBeViewButtonColor;

    public Image viewPagesButton;
    public Image viewDialoguesButton;

    public GameObject pagesCollection;
    public GameObject dialoguesCollection;

    public Scrollbar pageScrollbar;
    public Scrollbar dialogueScrollbar;

    void Start()
    {
        inventoryCanvas.SetActive(false);
        hasAccessToInventory = false;
        ViewPages();
    }

    void Update()
    {
        if (hasAccessToInventory)
        {
            if (inventoryCanvas != null && Input.GetKeyDown(KeyCode.Tab))
            {
                inventoryCanvas.SetActive(true);
                pageScrollbar.value = 1;
                dialogueScrollbar.value = 1;
            }

            if (inventoryCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseInventory();
            }
        }
        else
        {
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        inventoryCanvas.SetActive(false);
    }

    public void ViewPages()
    {
        viewPagesButton.color = currentViewButtonColor;
        viewDialoguesButton.color = canBeViewButtonColor;

        pagesCollection.SetActive(true);
        dialoguesCollection.SetActive(false);

        pageScrollbar.value = 1;
        dialogueScrollbar.value = 1;
    }

    public void ViewDialogues()
    {
        viewPagesButton.color = canBeViewButtonColor;
        viewDialoguesButton.color = currentViewButtonColor;

        pagesCollection.SetActive(false);
        dialoguesCollection.SetActive(true);

        pageScrollbar.value = 1;
        dialogueScrollbar.value = 1;
    }
}
