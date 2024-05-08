using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private bool allowedToNavigate;

    public Color currentViewButtonColor;
    public Color canBeViewedButtonColor;
    public Color canBeViewedTextButtonColor;

    public Image viewPagesButton;
    public Image viewSettingsButton;
    public TextMeshProUGUI viewPagesButtonText;
    public TextMeshProUGUI viewSettingsButtonText;

    public GameObject pagesCollection;
    public GameObject settingsMenu;

    public Scrollbar pageScrollbar;
    public Scrollbar settingsScrollbar;
    public Scrollbar inspectPageScrollbar;

    [Header("PageContent")]
    public GameObject pageInspection;
    public GameObject additionalpageInspectionItem1;
    public GameObject additionalpageInspectionItem2;


    public TextMeshProUGUI pageContent;
    public GameObject[] pages;

    [TextArea(3, 10)]
    public string[] pageLines;
    public TextMeshProUGUI pageLinesText;
    public RectTransform pageRectTransform;

    [TextArea(1, 3)]
    public string[] pageContentLine;
    public float[] newYPosition;
    public float[] newHeight;

    public int SelectedPage;

    [Header("Collected Pages")]
    public int currentPage = 0;

    void Start()
    {
        inventoryCanvas.SetActive(false);
        hasAccessToInventory = false;
        inventoryAlreadyOpened = false;
        allowedToCloseInventory = false;
        allowedToView = false;
        allowedToNavigate = false;
        pageContent.text = "";
        pageLinesText.text = "";
        pageInspection.SetActive(false);
        additionalpageInspectionItem1.SetActive(false);
        additionalpageInspectionItem2.SetActive(false);

        viewPagesButtonText.color = currentViewButtonColor;
        viewSettingsButtonText.color = canBeViewedTextButtonColor;

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
                ViewPages();
                inventoryAlreadyOpened = true;
                allowedToNavigate = true;
            }
        }
        else
        {
            CloseInventory();
        }

        //if (inventoryCanvas != null && Input.GetKeyDown(KeyCode.Escape) && !inventoryAlreadyOpened)
        //{
        //    Tutorial.Instance.visualNovel.GetComponent<Canvas>().sortingOrder = 0;
        //    ResetScrollBars();
        //    inventoryCanvas.SetActive(true);
        //    ViewSettings();
        //    inventoryAlreadyOpened = true;
        //}

        if (inventoryCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if (pageInspection.activeSelf)
            {
                ClosePage();
            }
            else
            {
                CloseInventory();
            }
        }

        if (inventoryCanvas.activeSelf && !pageInspection.activeSelf && allowedToNavigate && allowedToNavigate)
        {
            if (Input.GetKeyDown(KeyCode.E) && pagesCollection.activeSelf)
            {
                ViewSettings();
            }
            if (Input.GetKeyDown(KeyCode.Q) && !pagesCollection.activeSelf)
            {
                ViewPages();
            }
            if (Input.GetKeyDown(KeyCode.Q) && settingsMenu.activeSelf)
            {
                ViewPages();
            }
            if (Input.GetKeyDown(KeyCode.E) && !settingsMenu.activeSelf)
            {
                ViewSettings();
            }
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

        //Tutorial.Instance.showTutorialDialogueCanvas.GetComponent<Canvas>().sortingOrder = 5;
    }

    public void ViewPages()
    {
        if (allowedToNavigate)
        {
            ResetScrollBars();
            viewPagesButton.color = currentViewButtonColor;
            viewSettingsButton.color = canBeViewedButtonColor;
            viewPagesButtonText.color = currentViewButtonColor;
            viewSettingsButtonText.color = canBeViewedTextButtonColor;

            pagesCollection.SetActive(true);
            settingsMenu.SetActive(false);
        }       
    }

    public void ViewSettings()
    {
        if (allowedToView)
        {
            ResetScrollBars();
            viewPagesButton.color = canBeViewedButtonColor;
            viewSettingsButton.color = currentViewButtonColor;
            viewPagesButtonText.color = canBeViewedTextButtonColor;
            viewSettingsButtonText.color = currentViewButtonColor;

            pagesCollection.SetActive(false);
            settingsMenu.SetActive(true);
        }       
    }
    public void ResetScrollBars()
    {
        pageScrollbar.value = 1;
        settingsScrollbar.value = 1;
    }

    public void ViewSelectedPage(int viewSelectedPage)
    {
        pageScrollbar.interactable = false;
        viewPagesButton.GetComponent<Button>().interactable = false;
        viewSettingsButton.GetComponent<Button>().interactable = false;

        if (pageContent.text != "")
        {
            pageContent.text = "";
        }

        pageInspection.SetActive(true);
        additionalpageInspectionItem1.SetActive(true);
        additionalpageInspectionItem2.SetActive(true);

        SelectedPage = viewSelectedPage;
        if (!Tutorial.Instance.TutorialComplete)
        {
            Tutorial.Instance.TutorialComplete = true;
            allowedToCloseInventory = true;
            allowedToView = true;
            Tutorial.Instance.requiredToOpenInventory = false;
            DialogueManager.Instance.DisplayNextLine();
        }
        pageLinesText.text = pageLines[SelectedPage];

        Vector2 position = pageRectTransform.anchoredPosition;
        position.y = newYPosition[SelectedPage];
        pageRectTransform.anchoredPosition = position;

        Vector2 size = pageRectTransform.sizeDelta;
        size.y = newHeight[SelectedPage];
        pageRectTransform.sizeDelta = size;
    }

    public void ClosePage()
    {
        pageScrollbar.interactable = true;
        viewPagesButton.GetComponent<Button>().interactable = true;
        viewSettingsButton.GetComponent<Button>().interactable = true;

        inspectPageScrollbar.value = 1;
        pageInspection.SetActive(false);
        additionalpageInspectionItem1.SetActive(false);
        additionalpageInspectionItem2.SetActive(false);

        pageLinesText.text = "";
    }

    public void PageCollected()
    {
        if (currentPage <= pages.Length)
        {
            pages[currentPage].GetComponent<Button>().interactable = true;
            currentPage++;
        }
        else
        {
            Debug.Log("All pages have been collected.");
        }
    }
    public void UpdatePageContentOnHoverEnter(int viewingPage)
    {
        if (pages[viewingPage].GetComponent<Button>().interactable)
        {
            pageContent.text = pageContentLine[viewingPage];
        }
    }

    public void UpdatePageContentOnHoverExit(int viewingPage)
    {
        pageContent.text = "";
    }

    public void HoverEnterPagesButton()
    {
        if (settingsMenu.activeSelf && viewPagesButton.GetComponent<Button>().interactable == true && allowedToNavigate)
        {
            viewPagesButtonText.color = currentViewButtonColor;
        }
    }
    public void HoverExitPagesButton()
    {
        if (settingsMenu.activeSelf && viewPagesButton.GetComponent<Button>().interactable == true && allowedToNavigate)
        {
            viewPagesButtonText.color = canBeViewedTextButtonColor;
        }
    }
    public void HoverEnterSettingsButton()
    {
        if (pagesCollection.activeSelf && viewSettingsButton.GetComponent<Button>().interactable == true)
        {
            viewSettingsButtonText.color = currentViewButtonColor;
        }
    }

    public void HoverExitSettingsButton()
    {
        if (pagesCollection.activeSelf && viewSettingsButton.GetComponent<Button>().interactable == true)
        {
            viewSettingsButtonText.color = canBeViewedTextButtonColor;
        }
    }
}
