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

    public bool tutorialStarted = false;

    void Start()
    {
        startTutorialDialogue.SetActive(false);
    }
    private void Update()
    {
        if (tutorialStarted == true && firstPage != null)
        {
            firstPage.SetActive(true);
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
