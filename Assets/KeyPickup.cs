using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{

    public QuestTracker questTracker; //Stores a reference of QuestTracker so the key can check the quest state

    void Start()
    {
        questTracker = FindObjectOfType<QuestTracker>(); //Find the QuestTracker and assign it
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && questTracker.questAccepted) //Only allows Player gameobject to
                                                                      //pick up key and again only if quest has been accepted
        {
            questTracker.questCompleted = true; //The objective is complete

            Destroy(gameObject); //Removes key, to show it is 'picked up' by player
        }
    }
}
