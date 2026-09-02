using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantInteraction : MonoBehaviour
{
    private Transform player;

    public TMPro.TextMeshProUGUI interactionText;

    public PlayerMovement playerMovement;

    bool isTalking = false;

    int dialogueStep = 0;

    private QuestTracker questTracker;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; //Finds the persistent player so distance to merchant can be measured
        questTracker = FindObjectOfType<QuestTracker>(); //Finds the QuestTracker so merchant can read the quest state
    }


    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position); //calculates distance between player and merchant

        //Dialogue without Queststate while talking to merchant (line by line)
        if (isTalking && Input.GetKeyDown(KeyCode.X))
        {
            if (dialogueStep == 1)
            {
                dialogueStep = 2;
                interactionText.text = "I lost a very special key in a cave system.\nPress X to continue";
            }
            else if (dialogueStep == 2)
            {
                dialogueStep = 3;
                interactionText.text = "Here's a torch, go in there and find it for me.\nPress Y to accept";
            }
            else if (dialogueStep == 4)
            {
                dialogueStep = 5;
                interactionText.text = "Thank you explorer!\nPress X to continue";
            }
            else if (dialogueStep == 5)
            {
                dialogueStep = 6;
                interactionText.text = "Quest Completed!";
            }
            else if (dialogueStep == 6)
            {
                isTalking = false;
                playerMovement.canMove = true;
            }
        }

        //Dialogue based on distance
        if (questTracker.questCompleted)
        {
            if (distance <= 2.8f && !isTalking)
            {
                interactionText.text = "Press 'X' to talk to the Merchant";

                if (Input.GetKeyDown(KeyCode.X))
                {
                    isTalking = true; //Player is currently talking with merchant
                    playerMovement.canMove = false; //stops playermovment when talking to merchant
                    dialogueStep = 4;

                    interactionText.text = "Hey explorer, do you bring good news?\nPress X to give the key";
                }
            }
            else if (!isTalking)
            {
                interactionText.text = "Return to the Merchant";
            }
        }
        //Keeps the quest message until quest completed
        else if (questTracker.questAccepted)
        {
            if (!isTalking)
            {
                interactionText.text = "Quest in progress: Find the key in the cave";
            }
        }
        //Dialogue before quest picked up
        else if (distance <= 2.8f && !isTalking)
        {
            interactionText.text = "Press 'X' to talk to the Merchant";

            if (Input.GetKeyDown(KeyCode.X))
            {
                isTalking = true; //Player is currently talking with merchant
                playerMovement.canMove = false; //Stops player movement while player talks
                dialogueStep = 1;

                interactionText.text = "Hey there explorer, I need your help.\nPress X to continue";
            }
        }

        //Allows the player to accept the quest with Y when we reach that level
        if (isTalking && dialogueStep == 3 && Input.GetKeyDown(KeyCode.Y))
        {
            isTalking = false; //Player not talking with Merchant
            playerMovement.canMove = true; //Player can move (to do quest)
            questTracker.questAccepted = true; //Quest has been given out
        }
    } 
}
