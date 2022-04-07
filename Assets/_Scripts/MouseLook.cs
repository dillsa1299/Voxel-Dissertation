using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    This script is used to move the players camera with the mouse

    Code in this document adapted from the following source:
    https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys
*/

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody; //Player object

    public float xRotation = 0f;

    public MainController controller;

    void Update()
    {
        if (controller.isPlaying)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
            //Multiply by time so that movement is consistent across inconsistent frame timings

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Limits rotation to 180 degrees

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //Rotate up and down
            playerBody.Rotate(Vector3.up * mouseX); //Rotate left and right
        }
    }
}
