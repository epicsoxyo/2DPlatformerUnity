using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sawController : MonoBehaviour
{
    // stores the saw's rotational speed
    public float speed = 300;

    void Update ()
    {
        // angle of rotation is defined as the rotational speed times time
        float angle = speed * Time.deltaTime;
        // rotate the sprite about the z axis by the calculated angle.
        transform.Rotate
        (
            Vector3.forward, // axis of rotation (0, 0, 1)
            angle // the angle to set the sprite to
        );
    }

}
