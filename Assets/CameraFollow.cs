using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform player; // Stores a reference to the Player so the camera knows what to follow
    public float smoothSpeed = 5f; // Controls how smoothly the camera follows the Player

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform; // Gets the Player's Transform so its position can be followed
        }
    }


    void LateUpdate()
    {
        if (player == null) // Prevents errors if the Player reference cannot be found
        {
            return;
        }
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z); // Creates the camera's target position while keeping its Z position unchanged

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime); // Smoothly moves the camera toward the Player
    }
}

