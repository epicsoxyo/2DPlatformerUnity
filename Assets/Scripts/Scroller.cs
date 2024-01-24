using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    // stores the number of layers
    int numLayers;
    // stores the layers
    Transform[] layers;
    // an array to store the movement speeds of each layer
    public float [] speeds;
    // to store the camera
    public Camera attachedCamera;
    // stores the position of the camera at the start of the game
    Vector3 initialPosition;

    void Start ()
    {
        // the number of layers is obtained from the array of speeds created in inspector
        numLayers = speeds.Length;
        // set the length of the layers array to match the speeds array
        layers = new Transform[numLayers];
        // set the initial position of all layers before transform
        initialPosition = attachedCamera.transform.position;

        // fill the layers array with the children of the background object
        for (int i = 0; i < numLayers; i++)
        {
            layers[i] = transform.GetChild(i);
        }
    }

    void Update ()
    {
        // calculate the displacement of the camera from its starting position
        Vector3 diff = attachedCamera.transform.position - initialPosition;

        // for each layer
        for ( int i = 0; i < numLayers; i++ )
        {
            // calculate its individual displacement
            Vector3 scaledDiff = diff * speeds[i];
            // transform the layer (hence localPosition; we don't want the entire background)
            layers[i].transform.localPosition = new Vector3(scaledDiff.x, scaledDiff.y, 0.0f);
        }
    }

}
