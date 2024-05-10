using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    public Sprite[] availablePlayerAppearances;
    public GameObject[] arrow;
    public RuntimeAnimatorController[] animations;

    void Start()
    {
        PlayerController.Instance.player.GetComponent<SpriteRenderer>().enabled = false;
        PlayerController.Instance.player.GetComponent<Animator>().runtimeAnimatorController = animations[0];

        InventoryManager.Instance.ShowMouse();
    }
    public void ChoosePlayer(int playerIndex)
    {
        AudioController.Instance.PlaySFX(3);

        PlayerController.Instance.player.GetComponent<SpriteRenderer>().sprite = availablePlayerAppearances[playerIndex];
        PlayerController.Instance.player.GetComponent<Animator>().runtimeAnimatorController = animations[playerIndex];
        PlayerController.Instance.player.GetComponent<SpriteRenderer>().enabled = true;

        Tutorial.Instance.visualNovel.SetActive(true);
        Tutorial.Instance.selectPlayerCustomization.SetActive(false);
        Tutorial.Instance.allowedToDisplayNextLine = true;

        InventoryManager.Instance.HideMouse();
    }

    public void OnHover(int playerIndex)
    {
        arrow[playerIndex].SetActive(true);
    }
    public void OnHoverExit(int playerIndex)
    {
        arrow[playerIndex].SetActive(false);
    }
}
