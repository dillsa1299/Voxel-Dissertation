using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script controls movement of the players model, as well as its graphics and collision physics

    Code in this document adapted from the following source:
    https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
*/

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float moveSpeed = 24f;
    float originalMoveSpeed;
    Vector3 velocity;

    void Start()
    {
        originalMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("left shift")) //Player sprints whilst holding shift
        {
            moveSpeed = originalMoveSpeed * 3f;
        }
        else
        {
            moveSpeed = originalMoveSpeed;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * moveSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        if(Input.GetKey(KeyCode.Space)) //Go up
        {
            velocity.y = moveSpeed;
        }
        else if(Input.GetKey(KeyCode.C)) //Go down
        {
            velocity.y = -moveSpeed;
        }
        else
        {
            velocity.y = 0;
        }
        controller.Move(velocity * Time.deltaTime);
    }

    public void teleportPlayer(Vector3 vectorLocation)
    {
        controller.enabled = false; //Need to disable character controller to teleport player
        transform.position = vectorLocation;
        controller.enabled = true;
    }
}
