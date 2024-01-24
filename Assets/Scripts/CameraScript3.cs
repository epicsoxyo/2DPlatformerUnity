/*
a more sophisticated camera script.
the camera only moves if the player is a certain distance
from the edge of the camera box - i.e., if the player is about
to leave the frame, the camera moves to catch up with them.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript3 : MonoBehaviour
{
    // a public variable so the player can be chosen in the inspector window
    public Transform attachedPlayer;
    // to store the camera the script is attached to
    Camera thisCamera;
    // the amount the camera lags behind the player
    public float blendAmount = 0.05f;
    // stores the size of the "hitbox" around the player to determine whether
    // they are at the end of the world
    public float boxSizeX = 2.0f;
    public float boxSizeY = 2.0f;

    // on game start, grab the camera and store it in the variable we created
    void Start () 
    {   
        thisCamera = GetComponent<Camera>();
    }
    
    // each frame
    void Update () 
    {
        // get the player's position and the camera's position
        Vector3 playerPos = attachedPlayer.transform.position;
        Vector3 cameraPos = transform.position;

        // store the camera's x and y coordinates in separate variables
        float camX, camY;
        camX = cameraPos.x;
        camY = cameraPos.y;

        // to store a virtual "hitbox" around the player
        float box_x0, box_x1, box_y0, box_y1;
        box_x0 = playerPos.x - boxSizeX ;
        box_x1 = playerPos.x + boxSizeX ;
        box_y0 = playerPos.y - boxSizeY ;
        box_y1 = playerPos.y + boxSizeY ;

        // grabs the coordinates of the bottom left + top right of the camera box.
        // note: camera box is of "size" (1, 1, 1), and for 2D games z = 0.
        Vector3 bottomLeft = thisCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = thisCamera.ViewportToWorldPoint(new Vector3(1, 1, 0 ));

        float screenX0, screenX1, screenY0, screenY1;

        // store the x values of the two corners in separate variables
        screenX0 = bottomLeft.x;
        screenX1 = topRight.x;

        // if the "hitbox" exceeds the camera's boundaries, set the camera's
        // x coordinates to follow the player such that they appear to not move
        // on the screen.
        if (box_x0 < screenX0)
            camX = playerPos.x + 0.5f * (screenX1 - screenX0) - boxSizeX; 
        else if (box_x1 > screenX1)
            camX = playerPos.x - 0.5f * (screenX1 - screenX0) + boxSizeX;

        // repeat for the y coordinates.
        screenY0 = bottomLeft.y;
        screenY1 = topRight.y;

        if (box_y0 < screenY0)
            camY = playerPos.y + 0.5f * (screenY1 - screenY0) - boxSizeY;
        else if (box_y1 > screenY1)
            camY = playerPos.y - 0.5f * (screenY1 - screenY0) + boxSizeY;

        // finally, move the camera to the new coordinates.
        transform.position = new Vector3(camX, camY, cameraPos.z);
    }
}