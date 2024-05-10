using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBackground : MonoBehaviour
{
    #region Singleton

    private static AnimatedBackground _instance;
    public static AnimatedBackground Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AnimatedBackground>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(AnimatedBackground).Name;
                    _instance = obj.AddComponent<AnimatedBackground>();
                }
            }
            return _instance;
        }
    }

    #endregion

    public GameObject fallingPages;

    public GameObject[] arrows;

    void Start()
    {
        StartCoroutine(FallingPages());
        foreach (var arrow in arrows)
        {
            arrow.SetActive(false);
        }
    }

    IEnumerator FallingPages()
    {
        int randomChance = Random.Range(1, 4);

        if (randomChance == 1)
        {
            fallingPages.SetActive(true);
            yield return new WaitForSeconds(5);
            fallingPages.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(5);
        }

        StartCoroutine(FallingPages());
    }

    public void OnHoverEnter(int button)
    {
        arrows[button].SetActive(true);
    }

    public void OnHoverExit(int button)
    {
        arrows[button].SetActive(false);
    }
}