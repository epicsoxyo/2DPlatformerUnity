/*
a simple, but unsophisticated way of attaching a camera
to a player. the camera simply follows the player without
accounting for walls.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    /*
    a transform variable to store the position of the player.
    this is public so the player's object can be chosen from the inspector window.
    */
    public Transform attachedPlayer;
    // stores the camera the script is attached to
    Camera thisCamera;

    // on start, get the camera the script is attached to and stick it in the variable
    void Start () {
        thisCamera = GetComponent<Camera>();
    }

    // each frame
    void Update () {
        // get player's position
        Vector3 player = attachedPlayer.transform.position;
        // change the camera's position to that of the player's (without changing z)
        Vector3 newCamPos = new Vector3(player.x, player.y, transform.position.z);
        transform.position = newCamPos;
    }

}
