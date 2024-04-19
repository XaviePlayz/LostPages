using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public TextMeshProUGUI dialogueText;
    public Image characterShowcaseImage;
    public TextMeshProUGUI characterNameText;

    private Queue<string> dialogueQueue;
    private Queue<Sprite> CharacterShowcaseQueue;
    private Queue<string> CharacterNameQueue;

    private bool isDialogueActive;
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
        if (dialogueQueue.Count > 0)
        {
            string line = dialogueQueue.Dequeue();
            dialogueText.text = line;

            Sprite characterShowcase = CharacterShowcaseQueue.Dequeue();
            characterShowcaseImage.sprite = characterShowcase;

            string Characterline = CharacterNameQueue.Dequeue();
            characterNameText.text = Characterline;
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;

        dialogueQueue.Clear();
        CharacterShowcaseQueue.Clear();
        dialogueText.text = "";
        characterShowcaseImage.sprite = null;
        characterShowcaseImage.gameObject.SetActive(false);
        characterNameText.text = "";
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
        findAvailableDialogueTrigger.PlayDialogue();
    }
}