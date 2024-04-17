using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public DialogueData dialogue;
    public bool isTriggerable = true;
    public bool pressToContinue;
    public int timeToContinue;

    [Header("Interacted")]
    public bool hasInteracted = false;
    public bool hasInteractedAfterDialogue = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasInteracted && IsTriggerable())
        {
            DialogueManager.Instance.StartDialogue(this, dialogue, pressToContinue);
            hasInteracted = true;
        }
    }

    public void PlayDialogue()
    {
        if (!hasInteracted && IsTriggerable())
        {
            DialogueManager.Instance.StartDialogue(this, dialogue, pressToContinue);
            hasInteracted = true;
        }
    }

    public void SetTriggerable(bool value)
    {
        isTriggerable = value;
    }

    public bool IsTriggerable()
    {
        return isTriggerable;
    }
}