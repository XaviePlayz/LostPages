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
                inventoryAlreadyOpened = true;
            }
        }
        else
        {
            CloseInventory();
        }

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
        if (inventoryCanvas.activeSelf && !pageInspection.activeSelf)
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
    }

    public void ViewPages()
    {
        ResetScrollBars();
        viewPagesButton.color = currentViewButtonColor;
        viewSettingsButton.color = canBeViewedButtonColor;
        viewPagesButtonText.color = currentViewButtonColor;
        viewSettingsButtonText.color = canBeViewedTextButtonColor;

        pagesCollection.SetActive(true);
        settingsMenu.SetActive(false);
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
    public void UpdatePageContent(int viewingPage)
    {
        if (pages[viewingPage].GetComponent<Button>().interactable)
        {
            if (pageContent.text == "" && viewingPage == 0)
            {
                pageContent.text = "Chapter 1 - Page 1";
            }
            else if (pageContent.text == "" && viewingPage == 1)
            {
                pageContent.text = "Chapter 1 - Page 2";
            }
            else if (pageContent.text == "" && viewingPage == 2)
            {
                pageContent.text = "Chapter 4 - Page 27";
            }
            else if (pageContent.text == "" && viewingPage == 3)
            {
                pageContent.text = "Chapter 3 - Page 80";
            }
            else if (pageContent.text == "" && viewingPage == 4)
            {
                pageContent.text = "Chapter 3 - Page 132";
            }
            else if (pageContent.text == "" && viewingPage == 5)
            {
                pageContent.text = "Chapter 9 - Page 153";
            }
            else
            {
                pageContent.text = "";
            }
        }       
    }

    public void HoverEnterPagesButton()
    {
        if (settingsMenu.activeSelf)
        {
            viewPagesButtonText.color = currentViewButtonColor;
        }
    }
    public void HoverExitPagesButton()
    {
        if (settingsMenu.activeSelf)
        {
            viewPagesButtonText.color = canBeViewedTextButtonColor;
        }
    }
    public void HoverEnterSettingsButton()
    {
        if (pagesCollection.activeSelf)
        {
            viewSettingsButtonText.color = currentViewButtonColor;
        }
    }

    public void HoverExitSettingsButton()
    {
        if (pagesCollection.activeSelf)
        {
            viewSettingsButtonText.color = canBeViewedTextButtonColor;
        }
    }
}
