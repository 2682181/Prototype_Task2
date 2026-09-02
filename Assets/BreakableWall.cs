using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    public float interactionDistance = 1.2f; // Sets how close the Player needs to be to break the wall

    private Transform player; // Stores a reference to the Player so its distance from the wall can be checked

    private PlayerInventory playerInventory; // Stores a reference to the Player's inventory to check for the Pickaxe

    private Collider2D wallCollider; // Stores the wall's collider so the closest point to the Player can be found

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;

            playerInventory = playerObject.GetComponent<PlayerInventory>(); // Gets the Player's inventory from the Player GameObject
        }

        wallCollider = GetComponent<Collider2D>(); // Gets the collider attached to this wall
    }

    void Update()
    {
        if (player == null || playerInventory == null || wallCollider == null) // Stops the code if a required reference is missing
        {
            return;
        }

        if (!playerInventory.hasPickaxe) // Only allows the wall to be broken if the Player has the Pickaxe
        {
            return;
        }

        Vector2 closestPoint = wallCollider.ClosestPoint(player.position); // Finds the part of the wall closest to the Playe

        float distance = Vector2.Distance(player.position, closestPoint); // Calculates the Player's actual distance from the wall

        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.F)) // Checks that the Player is close enough and has pressed F
        {
            {
                playerInventory.hasPickaxe = false; // Uses up the one-use Pickaxe

                Destroy(gameObject); // Removes the wall so the Player can pass through it
            }
        }
    }
}
