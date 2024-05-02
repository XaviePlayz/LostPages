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

    void Start()
    {
        startTutorialDialogue.SetActive(false);
        tutorialStarted = false;
        tutorialSequenceEnded = false;
        tutorialSentenceCount = 0;
    }
    private void Update()
    {
        if (tutorialStarted == true && firstPage != null)
        {
            firstPage.SetActive(true);
        }

        if (requiredToOpenInventory && InventoryManager.Instance.inventoryCanvas.activeSelf)
        {
            DialogueManager.Instance.EndDialogue();
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
        if (tutorialStarted && tutorialSentenceCount == 7)
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
