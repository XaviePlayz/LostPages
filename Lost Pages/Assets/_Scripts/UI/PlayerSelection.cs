using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    #region Singleton

    private static PlayerSelection _instance;
    public static PlayerSelection Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerSelection>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(PlayerSelection).Name;
                    _instance = obj.AddComponent<PlayerSelection>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public Sprite[] availablePlayerAppearances;
    public GameObject[] arrow;
    public RuntimeAnimatorController[] animations;
    public bool characterSelected;

    void Start()
    {
        characterSelected = false;
        PlayerController.Instance.player.GetComponent<SpriteRenderer>().enabled = false;
        PlayerController.Instance.player.GetComponent<Animator>().runtimeAnimatorController = animations[0];

        GameOptionsManager.Instance.ShowMouse();
    }
    public void ChoosePlayer(int playerIndex)
    {
        characterSelected = true;
        AudioController.Instance.PlaySFX(3);

        PlayerController.Instance.player.GetComponent<SpriteRenderer>().sprite = availablePlayerAppearances[playerIndex];
        PlayerController.Instance.player.GetComponent<Animator>().runtimeAnimatorController = animations[playerIndex];
        PlayerController.Instance.player.GetComponent<SpriteRenderer>().enabled = true;

        Tutorial.Instance.visualNovel.SetActive(true);
        Tutorial.Instance.selectPlayerCustomization.SetActive(false);
        Tutorial.Instance.allowedToDisplayNextLine = true;

        GameOptionsManager.Instance.HideMouse();
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
