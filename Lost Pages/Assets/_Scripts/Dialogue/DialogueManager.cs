using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    #region Singleton

    private static DialogueManager _instance;
    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogueManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(DialogueManager).Name;
                    _instance = obj.AddComponent<DialogueManager>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public GameObject visualNovelCanvas;
    public GameObject introDialogue;

    public GameObject clickToContinueMouse;

    public TextMeshProUGUI dialogueText;
    public Image characterShowcaseImage;
    public TextMeshProUGUI characterNameText;

    private Queue<string> dialogueQueue;
    private Queue<Sprite> CharacterShowcaseQueue;
    private Queue<string> CharacterNameQueue;

    public bool isDialogueActive;
    private bool isPressToContinue;
    private bool isAutoDisplaying;
    private bool isFirstLine;

    private DialogueTrigger currentDialogueTrigger;
    private DialogueTrigger findAvailableDialogueTrigger;

    void Awake()
    {
        dialogueQueue = new Queue<string>();

        CharacterShowcaseQueue = new Queue<Sprite>();
        characterShowcaseImage.gameObject.SetActive(false);

        CharacterNameQueue = new Queue<string>();
    }

    void Start()
    {
        visualNovelCanvas.SetActive(true);
        findAvailableDialogueTrigger = FindObjectOfType<DialogueTrigger>();
        PlayFoundDialogue();
    }

    public void StartDialogue(DialogueTrigger dialogueTrigger, DialogueData dialogueData, bool pressToContinue)
    {
        if (!isDialogueActive && dialogueTrigger.isTriggerable)
        {
            isDialogueActive = true;

            // Clear previous dialogue
            dialogueQueue.Clear();
            CharacterShowcaseQueue.Clear();
            CharacterNameQueue.Clear();

            // Enqueue new lines of dialogue
            foreach (string line in dialogueData.lines)
            {
                dialogueQueue.Enqueue(line);
            }

            foreach (Sprite characterShowcase in dialogueData.Character_Showcases)
            {
                CharacterShowcaseQueue.Enqueue(characterShowcase);
            }

            foreach (string characterName in dialogueData.CharacterNameline)
            {
                CharacterNameQueue.Enqueue(characterName);
            }

            // Set the boolean for "Press E" dialogue
            isPressToContinue = pressToContinue;

            // Set the current DialogueTrigger
            currentDialogueTrigger = dialogueTrigger;

            // Freeze player position if needed
            if (pressToContinue)
            {
                isFirstLine = true;
            }
            else
            {
                // Start displaying dialogue
                StartCoroutine(DisplayLines());
            }
        }
    }

    public void Update()
    {
        // Check for first line of dialogue
        if (isFirstLine)
        {
            isFirstLine = false;
            string line = dialogueQueue.Dequeue();
            dialogueText.text = line;

            string Characterline = CharacterNameQueue.Dequeue();
            characterNameText.text = Characterline;

            Sprite characterShowcase = CharacterShowcaseQueue.Dequeue();
            characterShowcaseImage.sprite = characterShowcase;

            characterShowcaseImage.gameObject.SetActive(true);
        }

        if (InventoryManager.Instance.inventoryAlreadyOpened)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        // Check for key press to advance dialogue for "Press LEFT MOUSE CLICK" dialogue
        if (isDialogueActive && isPressToContinue && Input.GetKeyDown(KeyCode.Mouse0))
        {

            if (!isAutoDisplaying)
            {
                DisplayNextLine();
            }
        }
    }

    public void DisplayNextLine()
    {
        if (!Tutorial.Instance.requiredToOpenInventory)
        {
            if (dialogueQueue.Count > 0)
            {
                string line = dialogueQueue.Dequeue();
                dialogueText.text = line;

                Sprite characterShowcase = CharacterShowcaseQueue.Dequeue();
                characterShowcaseImage.sprite = characterShowcase;

                string Characterline = CharacterNameQueue.Dequeue();
                characterNameText.text = Characterline;

                if (Tutorial.Instance.tutorialStarted)
                {
                    Tutorial.Instance.tutorialSentenceCount++;
                    Tutorial.Instance.CheckForAllowedInputDuringTutorial();
                }

                if (InventoryManager.Instance.hasAccessToInventory)
                {
                    Tutorial.Instance.firstTimeOpeningInventoryCount++;
                    Tutorial.Instance.CheckForAllowedInputDuringInventoryExplanation();
                }
            }
            else
            {
                EndDialogue();
            }
        }        
    }

    public void EndDialogue()
    {
        isDialogueActive = false;

        dialogueQueue.Clear();
        CharacterShowcaseQueue.Clear();
        dialogueText.text = "";
        characterShowcaseImage.sprite = null;
        characterShowcaseImage.gameObject.SetActive(false);
        characterNameText.text = "";

        if (InventoryManager.Instance.hasAccessToInventory)
        {
            Tutorial.Instance.requiredToOpenInventory = false;
            DialogueManager.Instance.clickToContinueMouse.SetActive(true);
        }

        RemoveCanvas();
    }

    void RemoveCanvas()
    {
        visualNovelCanvas.SetActive(false);
        if (introDialogue.activeSelf)
        {
            introDialogue.SetActive(false);
            Tutorial.Instance.startTutorialDialogue.SetActive(true);
            StartCoroutine(Tutorial.Instance.StartTutorial());
        }
    }

    private IEnumerator DisplayLines()
    {
        while (dialogueQueue.Count > 0)
        {
            string line = dialogueQueue.Dequeue();
            dialogueText.text = line;

            Sprite characterShowcase = CharacterShowcaseQueue.Dequeue();
            characterShowcaseImage.sprite = characterShowcase;

            string Characterline = CharacterNameQueue.Dequeue();
            characterNameText.text = Characterline;

            if (isPressToContinue)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            }
            else
            {
                yield return new WaitForSeconds(currentDialogueTrigger.timeToContinue);
            }
        }

        EndDialogue();
    }

    public void PlayFoundDialogue()
    {
        findAvailableDialogueTrigger = FindObjectOfType<DialogueTrigger>();
        findAvailableDialogueTrigger.PlayDialogue();
    }
}