using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    #region Singleton

    private static Tutorial _instance;
    public static Tutorial Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Tutorial>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(Tutorial).Name;
                    _instance = obj.AddComponent<Tutorial>();
                }
            }
            return _instance;
        }
    }

    #endregion

    [Header("Visual Novel")]
    public GameObject startTutorialDialogue;
    public GameObject showTutorialDialogueCanvas;
    public GameObject novelBackground;
    public GameObject visualNovel;

    [Header("Player Customization")]
    public GameObject selectPlayerCustomization;

    [Header("Page Inspection")]
    public GameObject backgroundDarkenerTutorial;

    [Header("First Collectible Page")]
    public GameObject firstPage;

    [Header("Booleans")]
    public bool tutorialStarted;
    public bool tutorialSequenceEnded;
    public bool requiredToOpenInventory;
    public bool TutorialComplete;
    public bool allowedToDisplayNextLine;

    [Header("Counter")]
    public int tutorialSentenceCount;
    public int firstTimeOpeningInventoryCount;


    void Start()
    {
        startTutorialDialogue.SetActive(false);
        backgroundDarkenerTutorial.SetActive(false);
        tutorialStarted = false;
        tutorialSequenceEnded = false;
        tutorialSentenceCount = 0;
        TutorialComplete = false;
        allowedToDisplayNextLine = true;
    }
    private void Update()
    {
        if (tutorialStarted == true && firstPage != null)
        {
            firstPage.SetActive(true);
        }

        if (requiredToOpenInventory && Input.GetKeyDown(KeyCode.Tab) && !InventoryManager.Instance.inventoryAlreadyOpened)
        {
            InventoryManager.Instance.viewPagesButtonText.text = "Pages";
            AudioController.Instance.PlaySFX(0);

            InventoryManager.Instance.allowedToNavigate = true;

            allowedToDisplayNextLine = true;
            requiredToOpenInventory = false;
            InventoryManager.Instance.ResetScrollBars();
            InventoryManager.Instance.inventoryCanvas.SetActive(true);
            InventoryManager.Instance.inventoryAlreadyOpened = true;
            DialogueManager.Instance.DisplayNextLine();

            InventoryManager.Instance.allowedToCloseInventory = false;
            InventoryManager.Instance.allowedToViewSettings = false;
            InventoryManager.Instance.viewPagesButton.GetComponent<Button>().interactable = true;

            InventoryManager.Instance.ViewPages();
        }

        if (tutorialStarted && tutorialSentenceCount == 6)
        {
            tutorialSequenceEnded = true;
        }

        if (TutorialComplete)
        {
            InventoryManager.Instance.allowedToViewSettings = true;
        }
    }

    public void CheckForAllowedInputDuringTutorial()
    {
        if (!TutorialComplete)
        {
            if (tutorialStarted && tutorialSentenceCount == 2)
            {
                visualNovel.SetActive(false);
                selectPlayerCustomization.SetActive(true);

                allowedToDisplayNextLine = false;
            }
            if (tutorialStarted && tutorialSentenceCount == 3)
            {
                PlayerController.Instance.allowedToWalk = true;
            }
            if (tutorialStarted && tutorialSentenceCount == 6)
            {
                PlayerController.Instance.allowedToJump = true;
            }
        }
    }

    public void CheckForAllowedInputDuringInventoryExplanation()
    {
        if (!TutorialComplete)
        {
            if (InventoryManager.Instance.hasAccessToInventory && firstTimeOpeningInventoryCount == 1)
            {
                requiredToOpenInventory = true;
                DialogueManager.Instance.clickToContinueMouse.SetActive(false);
            }
            if (InventoryManager.Instance.hasAccessToInventory && firstTimeOpeningInventoryCount == 2)
            {
                requiredToOpenInventory = true;
            }
            if (InventoryManager.Instance.hasAccessToInventory && firstTimeOpeningInventoryCount > 2)
            {
                backgroundDarkenerTutorial.SetActive(true);
                DialogueManager.Instance.clickToContinueMouse.SetActive(true);
            }
        }      
    }

    public IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1);
        if (startTutorialDialogue.activeSelf)
        {
            showTutorialDialogueCanvas.SetActive(true);
            novelBackground.SetActive(false);
            PlayDialogue.Instance.PlayNewDialogue(1);
            tutorialStarted = true;
        }
    }
}
