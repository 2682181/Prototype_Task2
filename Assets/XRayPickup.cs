using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRayPickup : MonoBehaviour
{
    private PlayerMovement playerMovement; // Stores a reference to PlayerMovement so the Player can be frozen during the X-ray effect

    private QuestUI questUI; // Stores a reference to the UI so X-ray messages can be displayed

    private CaveLighting caveLighting; // Stores a reference to CaveLighting so the darkness can be removed during X-ray

    private Camera mainCamera; // Stores a reference to the Main Camera so its zoom can be changed

    private bool activated = false; // Prevents the X-ray from being activated more than once

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerMovement = playerObject.GetComponent<PlayerMovement>(); // Gets PlayerMovement from the Player GameObject
        }

        GameObject textObject = GameObject.Find("Text (TMP)");

        if (textObject != null)
        {
            questUI = textObject.GetComponent<QuestUI>(); // Gets QuestUI from the UI text
        }

        caveLighting = FindObjectOfType<CaveLighting>(); // Finds the CaveLighting system used to control the cave visibility

        mainCamera = Camera.main; // Gets the Main Camera
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) // Stops the X-ray effect from being triggered again
        {
            return;
        }

        if (other.CompareTag("Player")) // Only allows the Player to collect the X-ray goggles
        {
            activated = true; // Marks the goggles as used

            StartCoroutine(UseXRay()); // Starts the timed X-ray sequence
        }
    }

    IEnumerator UseXRay()
    {
        playerMovement.canMove = false; // Stops the Player moving while the X-ray sequence is happening

        if (questUI != null)
        {
            questUI.ShowMessage("You picked up X ray goggles, try them on.",6f); // Displays instructions before the X-ray activates
        }
    

        yield return new WaitForSeconds(3f); // Waits before activating the X-ray so the Player can read the message

        if (questUI != null)
        {
            questUI.HideText(); // Hides the text so it does not distract from the full-map view
        }

        float originalCameraSize = mainCamera.orthographicSize; // Saves the normal camera size so it can be restored later

        if (caveLighting != null)
        {
            caveLighting.xRayActive = true; // Removes the darkness so the entire cave can be seen
        }

        mainCamera.orthographicSize = 23f; // Zooms the camera out to show the larger Cave layout

        yield return new WaitForSeconds(2.7f);

        if (caveLighting != null)
        {
            caveLighting.xRayActive = false; // Restores the normal cave darkness
        }

        mainCamera.orthographicSize = originalCameraSize; // Returns the camera to its normal zoom

        if (questUI != null)
        {
            questUI.ShowText(); // Makes the UI visible again

            questUI.ShowMessage("\"My eyes hurt\"", 1.8f); // Shows the Player's reaction after the X-ray effect
        }

        playerMovement.canMove = true; // Allows the Player to move again

        Destroy(gameObject); // Removes the X-ray goggles after they have been used
    }
}