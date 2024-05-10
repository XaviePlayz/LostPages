using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDialogue : MonoBehaviour
{
    #region Singleton

    private static PlayDialogue _instance;
    public static PlayDialogue Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayDialogue>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PlayDialogue).Name;
                    _instance = obj.AddComponent<PlayDialogue>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public DialogueTrigger[] dialogueDatas;

    public void PlayNewDialogue(int dialogueToPlay)
    {
        DialogueManager.Instance.currentDialogueTrigger = dialogueDatas[dialogueToPlay];

        dialogueDatas[dialogueToPlay].PlayDialogue();
    }
}
