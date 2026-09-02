using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad; //Stores which scene should load when the Player enters the trigger

    public string spawnPointName; //Stores where the player should spawn in the new scene

   

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //Makes sure only the player can activate the scene transition
        {
            PlayerMovement.spawnPointName = spawnPointName; //Gives PlayerMovement Script the name of the
                                                            //spawnpoint so that it can set the Player position there
            SceneManager.LoadScene(sceneToLoad); //Loads the selected scene
        }
    }
}
