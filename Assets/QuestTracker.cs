using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{

    public bool questAccepted = false; //Stores if player picked up quest or not

    public bool questCompleted = false; //Stores if player completed quest or not

    private static QuestTracker instance; //Keeps track of the main QuestTracker to prevent duplicates


    void Start()
    {
        if (instance != null && instance != this) //Checks if another QuestTracker exists or not
        {
            Destroy(gameObject);
            return;
        }

        instance = this; //Sets this one to the Main QuestTracker on Start

        DontDestroyOnLoad(gameObject); //Keeps QuestTracker Script prominent between scenes
    }
}
