using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    public bool canMove = true;

    private Rigidbody2D rb;

    private Vector2 movement;

    public static string spawnPointName;

    private static PlayerMovement instance; //Keeps track of main character so duplicates can be safely removed


    void Start() 
    {
        if (instance != null && instance != this) //Checks for duplicate of Player GameObject to delete
        {
            Destroy(gameObject);
            return;
        }

        instance = this; //Sets this player as the main Player instance

        rb = GetComponent<Rigidbody2D>();

        DontDestroyOnLoad(gameObject); //Player wont be deleted when new scenes are loaded

        SceneManager.sceneLoaded += OnSceneLoaded; //Run the method when a new scene is loaded
        
    }

  
    void Update()
    {
        if (!canMove) //Locks Player Movment if game requires it
        {
            movement = Vector2.zero; //Clears all movement prior to stop, prevents sliding
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontal, vertical); //combines horizontal and vertical input to one movement direction

    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime); //Moves player based on movement direction and speed
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) //Called only when a new scene loads so player is put at right location
    {
        if (spawnPointName != "") //checks to find if a spawnPointName has been located/defined in the new scene
        {
            GameObject spawnPoint = GameObject.Find(spawnPointName); //Finds the spawnpoint in the new scene

            if (spawnPoint != null) //Stops error if location aint found
            {
                transform.position = spawnPoint.transform.position; //Places player at spawnpoints location
            }
        }
    }

}
