using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public GameObject startTutorialDialogue;
    public GameObject showTutorialDialogueCanvas;
    public GameObject novelBackground;

    public GameObject firstPage;

    public bool tutorialStarted;
    public bool tutorialSequenceEnded;
    public int tutorialSentenceCount;

    public int firstTimeOpeningInventoryCount;
    public bool requiredToOpenInventory;
    public bool TutorialComplete;

    void Start()
    {
        startTutorialDialogue.SetActive(false);
        tutorialStarted = false;
        tutorialSequenceEnded = false;
        tutorialSentenceCount = 0;
        TutorialComplete = false;
    }
    private void Update()
    {
        if (tutorialStarted == true && firstPage != null)
        {
            firstPage.SetActive(true);
        }

        if (requiredToOpenInventory && Input.GetKeyDown(KeyCode.Tab) && !InventoryManager.Instance.inventoryAlreadyOpened)
        {
            requiredToOpenInventory = false;
            InventoryManager.Instance.ResetScrollBars();
            InventoryManager.Instance.inventoryCanvas.SetActive(true);
            InventoryManager.Instance.inventoryAlreadyOpened = true;
            DialogueManager.Instance.DisplayNextLine();
        }

        if (tutorialStarted && tutorialSentenceCount == 8)
        {
            tutorialSequenceEnded = true;
        }
    }

    public void CheckForAllowedInputDuringTutorial()
    {
        if (tutorialStarted && tutorialSentenceCount == 2)
        {
            PlayerController.Instance.allowedToWalk = true;
        }
        if (tutorialStarted && tutorialSentenceCount == 5)
        {
            PlayerController.Instance.allowedToJump = true;
        }
    }

    public void CheckForAllowedInputDuringInventoryExplanation()
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
    }

    public IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1);
        if (startTutorialDialogue.activeSelf)
        {
            showTutorialDialogueCanvas.SetActive(true);
            novelBackground.SetActive(false);
            DialogueManager.Instance.PlayFoundDialogue();
            tutorialStarted = true;
        }
    }
}
