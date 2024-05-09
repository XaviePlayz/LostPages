using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedBackground : MonoBehaviour
{
    public GameObject fallingPages;

    void Start()
    {
        StartCoroutine(FallingPages());
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
}