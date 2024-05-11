using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

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
    public GameObject badEndingDialogue;
    public GameObject goodEndingDialogue;

    public GameObject clickToContinueMouse;

    public TextMeshProUGUI dialogueText;
    public Image characterShowcaseImage;
    public TextMeshProUGUI characterNameText;

    public GameObject characterNameHolder;
    public GameObject dialogueHolder;

    private Queue<string> dialogueQueue;
    private Queue<Sprite> CharacterShowcaseQueue;
    private Queue<string> CharacterNameQueue;
    private Queue<Color> DialogueBoxColorAppearanceQueue;

    public int totalSentenceCount;

    public bool isDialogueActive;
    private bool isPressToContinue;
    private bool isAutoDisplaying;
    private bool isFirstLine;
    private bool introFinished;

    public string url;

    public DialogueTrigger currentDialogueTrigger;

    void Awake()
    {
        dialogueQueue = new Queue<string>();

        CharacterShowcaseQueue = new Queue<Sprite>();
        characterShowcaseImage.gameObject.SetActive(false);

        CharacterNameQueue = new Queue<string>();
        DialogueBoxColorAppearanceQueue = new Queue<Color>();
    }

    void Start()
    {
        visualNovelCanvas.SetActive(true);
        badEndingDialogue.SetActive(false);
        goodEndingDialogue.SetActive(false);

        PlayDialogue.Instance.PlayNewDialogue(0);
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
            DialogueBoxColorAppearanceQueue.Clear();

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

            foreach (Color dialogueBoxColorAppearance in dialogueData.DialogueBoxColorAppearance)
            {
                DialogueBoxColorAppearanceQueue.Enqueue(dialogueBoxColorAppearance);
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

            Color DialogueBox = DialogueBoxColorAppearanceQueue.Dequeue();
            dialogueHolder.GetComponent<Image>().color = DialogueBox;
            characterNameHolder.GetComponent<Image>().color = DialogueBox;

            Sprite characterShowcase = CharacterShowcaseQueue.Dequeue();
            characterShowcaseImage.sprite = characterShowcase;

            characterShowcaseImage.gameObject.SetActive(true);
        }

        if (characterNameText.text == "")
        {
            characterNameHolder.SetActive(false);
        }
        else
        {
            characterNameHolder.SetActive(true);
        }

        if (InventoryManager.Instance.inventoryAlreadyOpened)
        {
            InventoryManager.Instance.ShowMouse();
        }
        else
        {
            if (!Tutorial.Instance.selectPlayerCustomization.activeSelf)
            {
                InventoryManager.Instance.HideMouse();
            }
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
            if (Tutorial.Instance.allowedToDisplayNextLine)
            {
                AudioController.Instance.PlaySFX(3);

                totalSentenceCount++;
                if (dialogueQueue.Count > 0)
                {
                    string line = dialogueQueue.Dequeue();
                    dialogueText.text = line;

                    Sprite characterShowcase = CharacterShowcaseQueue.Dequeue();
                    characterShowcaseImage.sprite = characterShowcase;

                    string Characterline = CharacterNameQueue.Dequeue();
                    characterNameText.text = Characterline;

                    Color DialogueBox = DialogueBoxColorAppearanceQueue.Dequeue();
                    dialogueHolder.GetComponent<Image>().color = DialogueBox;
                    characterNameHolder.GetComponent<Image>().color = DialogueBox;

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

        if (PlayerController.Instance.hasReachedTheEnd)
        {
            Application.OpenURL(url);
            AudioVolumeController.Instance.GoToMainMenu();
        }

        if (!introFinished)
        {
            introFinished = true;
            AudioController.Instance.PlayMusic(1);
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

            Color DialogueBox = DialogueBoxColorAppearanceQueue.Dequeue();
            dialogueHolder.GetComponent<Image>().color = DialogueBox;
            characterNameHolder.GetComponent<Image>().color = DialogueBox;

            if (isPressToContinue)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0));
            }
            else
            {
                yield return new WaitForSeconds(currentDialogueTrigger.timeToContinue);
            }
        }

        EndDialogue();
    }
}