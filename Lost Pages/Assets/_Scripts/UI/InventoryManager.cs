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

    [Header("Inventory Canvas")]
    public GameObject inventoryCanvas;

    [Header("Booleans")]
    public bool hasAccessToInventory;
    public bool inventoryAlreadyOpened;
    public bool allowedToCloseInventory;
    public bool allowedToViewSettings;
    public bool allowedToNavigate;

    [Header("Buttons")]
    public Image viewPagesButton;
    public Image viewSettingsButton;
    public TextMeshProUGUI viewPagesButtonText;
    public TextMeshProUGUI viewSettingsButtonText;

    [Header("Inventory View")]
    public GameObject pagesCollection;
    public GameObject settingsMenu;

    [Header("Inventory Scrollbars")]
    public Scrollbar pageScrollbar;
    public Scrollbar inspectPageScrollbar;

    [Header("Page Inspection")]
    public GameObject pageInspection;
    public GameObject additionalpageInspectionItem1;
    public GameObject additionalpageInspectionItem2;
    public int SelectedPage;

    [TextArea(3, 10)]
    public string[] pageLines;
    public TextMeshProUGUI pageLinesText;

    [Header("Page Inspection [Content Settings]")]
    public RectTransform pageRectTransform;
    public float[] newYPosition;
    public float[] newHeight;

    [Header("All Collectable Pages")]
    public GameObject[] pages;

    [Header("Page Chapters")]
    [TextArea(1, 3)]
    public string[] pageContentLine;
    public TextMeshProUGUI pageContent;

    [Header("Collected Pages")]
    public int currentPage = 0;

    void Start()
    {
        hasAccessToInventory = false;
        inventoryAlreadyOpened = false;
        allowedToCloseInventory = true;
        allowedToViewSettings = true;
        allowedToNavigate = false;
        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
            pageContent.text = "";
            pageLinesText.text = "";
            viewPagesButtonText.text = "LOCKED";
            pageInspection.SetActive(false);
            additionalpageInspectionItem1.SetActive(false);
            additionalpageInspectionItem2.SetActive(false);
            viewPagesButton.GetComponent<Button>().interactable = false;
            viewPagesButtonText.color = GameOptionsManager.Instance.canBeViewedTextButtonColor;
            viewSettingsButtonText.color = GameOptionsManager.Instance.currentViewButtonColor;
        }
    }

    void Update()
    {
        if (hasAccessToInventory)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !inventoryAlreadyOpened && !Tutorial.Instance.requiredToOpenInventory)
            {
                Tutorial.Instance.showTutorialDialogueCanvas.SetActive(false);

                AudioController.Instance.PlaySFX(0);

                ResetScrollBars();
                inventoryCanvas.SetActive(true);
                inventoryAlreadyOpened = true;

                ViewPages();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !inventoryAlreadyOpened)
        {
            AudioController.Instance.PlaySFX(0);

            Tutorial.Instance.showTutorialDialogueCanvas.SetActive(false);
            Tutorial.Instance.allowedToDisplayNextLine = false;
            ResetScrollBars();
            inventoryCanvas.SetActive(true);
            ViewSettings();
            inventoryAlreadyOpened = true;
        }
        else if (inventoryAlreadyOpened && Input.GetKeyDown(KeyCode.Escape))
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
        
        if ((inventoryAlreadyOpened && Input.GetKeyDown(KeyCode.Escape) && GameOptionsManager.Instance.areYouSureMenu.activeSelf))
        {
            GameOptionsManager.Instance.ReturnToGame();
        }

        if (inventoryCanvas.activeSelf && !pageInspection.activeSelf && allowedToNavigate)
        {
            if (Input.GetKeyDown(KeyCode.E) && pagesCollection.activeSelf)
            {
                ViewSettings();
                AudioController.Instance.PlaySFX(3);
            }
            if (Input.GetKeyDown(KeyCode.Q) && !pagesCollection.activeSelf)
            {
                ViewPages();
                AudioController.Instance.PlaySFX(3);
            }
        }
    }

    public void CloseInventory()
    {
        AudioVolumeController.Instance.ConfirmSettings();

        if (allowedToCloseInventory)
        {
            AudioController.Instance.PlaySFX(1);

            ResetScrollBars();
            inventoryCanvas.SetActive(false);
            inventoryAlreadyOpened = false;
            Tutorial.Instance.allowedToDisplayNextLine = true;
            if (DialogueManager.Instance.isDialogueActive)
            {
                Tutorial.Instance.showTutorialDialogueCanvas.SetActive(true);
            }
        }
    }

    public void ViewPages()
    {
        if (allowedToNavigate)
        {
            AudioVolumeController.Instance.SetMusicVolume(AudioVolumeController.Instance.musicGameVolume);

            ResetScrollBars();
            viewPagesButton.color = GameOptionsManager.Instance.currentViewButtonColor;
            viewSettingsButton.color = GameOptionsManager.Instance.canBeViewedButtonColor;
            viewPagesButtonText.color = GameOptionsManager.Instance.currentViewButtonColor;
            viewSettingsButtonText.color = GameOptionsManager.Instance.canBeViewedTextButtonColor;

            pagesCollection.SetActive(true);
            settingsMenu.SetActive(false);
        }       
    }

    public void ViewSettings()
    {
        AudioVolumeController.Instance.OpenSettings();
        if (allowedToViewSettings && !GameOptionsManager.Instance.areYouSureMenu.activeSelf)
        {
            pageContent.text = "";

            ResetScrollBars();
            viewPagesButton.color = GameOptionsManager.Instance.canBeViewedButtonColor;
            viewSettingsButton.color = GameOptionsManager.Instance.currentViewButtonColor;
            viewPagesButtonText.color = GameOptionsManager.Instance.canBeViewedTextButtonColor;
            viewSettingsButtonText.color = GameOptionsManager.Instance.currentViewButtonColor;

            pagesCollection.SetActive(false);
            settingsMenu.SetActive(true);
        }       
    }
    public void ResetScrollBars()
    {
        pageScrollbar.value = 1;
    }

    public void ViewSelectedPage(int viewSelectedPage)
    {
        AudioController.Instance.PlayMusic(2);

        AudioVolumeController.Instance.lowPassFilter.cutoffFrequency = AudioVolumeController.Instance.muffledCutoffFrequency;
        ScrollbarController.Instance.mouseScrollSensitivity = 0f;
        AudioController.Instance.PlaySFX(2);

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
            Tutorial.Instance.requiredToOpenInventory = false;
            Tutorial.Instance.allowedToDisplayNextLine = true;
            allowedToCloseInventory = true;
            allowedToViewSettings = true;
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
        if (!Tutorial.Instance.backgroundDarkenerTutorial.activeSelf)
        {
            AudioController.Instance.PlayMusic(1);

            AudioVolumeController.Instance.lowPassFilter.cutoffFrequency = AudioVolumeController.Instance.normalFrequency;
            ScrollbarController.Instance.mouseScrollSensitivity = 0.5f;
            AudioController.Instance.PlaySFX(2);

            pageScrollbar.interactable = true;
            viewPagesButton.GetComponent<Button>().interactable = true;
            viewSettingsButton.GetComponent<Button>().interactable = true;

            inspectPageScrollbar.value = 1;
            pageInspection.SetActive(false);
            additionalpageInspectionItem1.SetActive(false);
            additionalpageInspectionItem2.SetActive(false);

            pageLinesText.text = "";
        }       
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
            viewPagesButtonText.color = GameOptionsManager.Instance.currentViewButtonColor;
        }
    }
    public void HoverExitPagesButton()
    {
        if (settingsMenu.activeSelf && viewPagesButton.GetComponent<Button>().interactable == true && allowedToNavigate)
        {
            viewPagesButtonText.color = GameOptionsManager.Instance.canBeViewedTextButtonColor;
        }
    }
    public void HoverEnterSettingsButton()
    {
        if (pagesCollection.activeSelf && viewSettingsButton.GetComponent<Button>().interactable == true)
        {
            viewSettingsButtonText.color = GameOptionsManager.Instance.currentViewButtonColor;
        }
    }

    public void HoverExitSettingsButton()
    {
        if (pagesCollection.activeSelf && viewSettingsButton.GetComponent<Button>().interactable == true)
        {
            viewSettingsButtonText.color = GameOptionsManager.Instance.canBeViewedTextButtonColor;
        }
    }
}
