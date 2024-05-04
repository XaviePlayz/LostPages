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
    public bool inventoryAlreadyOpened;
    public bool allowedToCloseInventory;
    public bool allowedToView;

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
        inventoryAlreadyOpened = false;
        allowedToCloseInventory = false;
        allowedToView = false;
        ViewPages();
    }

    void Update()
    {
        if (hasAccessToInventory)
        {
            if (inventoryCanvas != null && Input.GetKeyDown(KeyCode.Tab) && !inventoryAlreadyOpened && !DialogueManager.Instance.isDialogueActive)
            {
                ResetScrollBars();
                inventoryCanvas.SetActive(true);
                inventoryAlreadyOpened = true;
            }
        }
        else
        {
            CloseInventory();
        }

        if (inventoryCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        if (allowedToCloseInventory)
        {
            ResetScrollBars();
            inventoryCanvas.SetActive(false);
            inventoryAlreadyOpened = false;
        }
    }

    public void ViewPages()
    {
        ResetScrollBars();
        viewPagesButton.color = currentViewButtonColor;
        viewDialoguesButton.color = canBeViewButtonColor;

        pagesCollection.SetActive(true);
        dialoguesCollection.SetActive(false);
    }

    public void ViewDialogues()
    {
        if (allowedToView)
        {
            ResetScrollBars();
            viewPagesButton.color = canBeViewButtonColor;
            viewDialoguesButton.color = currentViewButtonColor;

            pagesCollection.SetActive(false);
            dialoguesCollection.SetActive(true);
        }       
    }
    public void ResetScrollBars()
    {
        pageScrollbar.value = 1;
        dialogueScrollbar.value = 1;
    }

    public void ViewPageOne()
    {
        allowedToCloseInventory = true;
        allowedToView = true;
        Tutorial.Instance.requiredToOpenInventory = false;
        DialogueManager.Instance.DisplayNextLine();
    }
}
