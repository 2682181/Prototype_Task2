using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    private QuestTracker questTracker; //Stores QuestTracker script so the UI can access quest state

    private TextMeshProUGUI interactionText; //Stores the text component on screen

    private string temporaryMessage = ""; //Stores temporary message such as a pickup instruction

    private float temporaryMessageTime = 0f; //Stores how long the message should stay on screen for


    void Start()
    {
        interactionText = GetComponent<TextMeshProUGUI>(); //Gets the TextMeshPro Component attached to the gameobject

        questTracker = FindObjectOfType<QuestTracker>(); //Finds the existing QuestTracker so the UI can read the quest state
    }

    void Update()
    {
        if (temporaryMessageTime > 0f) //Checks if a temporary message is currently active or not
        {
            temporaryMessageTime -= Time.deltaTime; //Counts down display time

            interactionText.text = temporaryMessage; //Displays the temporary message
            return;
        }

        if (questTracker.questCompleted) //Checks if key is found
        {
            interactionText.text = "Key found! Return to the merchant.";
        }
        else if (questTracker.questAccepted) //Checks if Quest is picked up (but not completed)
        {
            interactionText.text = "Quest in progress: Find the key in the caves.";
        }
        else //Displays the starting message before quest is accepted
        {
            interactionText.text = "Go to the merchant";
        }
    }

    public void ShowMessage(string message, float duration)
    {
        temporaryMessage = message; //Stores message to be temporarly shown

        temporaryMessageTime = duration; //Stores time temporary message to be shown for
    }

    public void HideText() //Hides the UI text
    {
        interactionText.enabled = false;
    }

    public void ShowText() //Makes the UI text visible again
    {
        interactionText.enabled = true;
    }
}

