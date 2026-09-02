using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxePickup : MonoBehaviour
{

    private PlayerInventory playerInventory; // Stores a reference to the Player's inventory (script) so the Pickaxe can be added
    private QuestUI questUI; // Stores a reference to the UI so the Pickaxe pickup message can be displayed

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerInventory = playerObject.GetComponent<PlayerInventory>(); // Gets the Player's inventory (script) from the Player GameObject
        }

        GameObject textObject = GameObject.Find("Text (TMP)");

        if (textObject != null)
        {
            questUI = textObject.GetComponent<QuestUI>(); // Gets the QuestUI component from the UI text
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player")) // Only allows the Player to collect the Pickaxe
        {
            playerInventory.hasPickaxe = true; // Gives the Player the Pickaxe

            if (questUI != null) // Checks that the UI reference exists before displaying the message
            {
                questUI.ShowMessage("You picked up the Pickaxe. Stand near a wall and press F to break it down.", 4f);
            }

            Destroy(gameObject); // Removes the Pickaxe after it has been collected
        }
    }
}
