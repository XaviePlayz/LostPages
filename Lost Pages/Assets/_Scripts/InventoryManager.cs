using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryCanvas;
    void Start()
    {
        inventoryCanvas.SetActive(false);
    }

    void Update()
    {
        if (inventoryCanvas != null && Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryCanvas.SetActive(true);
        }

        if (inventoryCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    public void CloseInventory()
    {
        inventoryCanvas.SetActive(false);
    }
}
