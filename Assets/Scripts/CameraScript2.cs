/*
a slightly better camera script; this camera lags behind
the player slightly for a smoother feel. this still does
not account for walls.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript2 : MonoBehaviour
{
    // a public variable so the player can be chosen in the inspector window
    public Transform attachedPlayer;
    // to store the camera the script is attached to
    Camera thisCamera;
    // the amount the camera lags behind the player
    public float blendAmount = 0.05f;

    // on game start, grab the camera and store it in the variable we created
    void Start () 
    {   
        thisCamera = GetComponent<Camera>();
    }
    
    // each frame
    void Update () 
    {
        // grab the player's position and store it in player
        Vector3 playerPos = attachedPlayer.transform.position;

        // calculate the position of the camera
        Vector3 newCamPos = playerPos * blendAmount + transform.position * (1.0f - blendAmount);

        // set the camera's position to the new position we calculated
        transform.position = new Vector3( newCamPos.x, newCamPos.y, transform.position.z);
    }

}
