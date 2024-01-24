using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // for managing text mesh objects
using UnityEngine.SceneManagement; // for managing scenes

public class PlayerController : MonoBehaviour
{
    // MOVEMENT VARS
    Rigidbody2D rigidBody; // to store player physics
    public float speed = 5.0f; // for adjusting the speed in Unity's inspector

    //JUMP VARS
    Vector2 boxExtents; // to store size of player collider box
    public float jumpForce = 8.0f; // " jump force "
    public float airControlForce = 10.0f; // " air control force "
    public float airControlMax = 1.5f; // " maximum air control "

    // ANIMATION VARS
    Animator animator; // for storing the animation controller for the player

    // AUDIO VARS
    public AudioSource coinSound;

    // UI VARS
    public TextMeshProUGUI uiText; // to store the UI text element to be manipulated
    int totalCoins; // to store the total coins in the game
    int coinsCollected; // to store the total coins the player has collected

    // LEVEL CONTROL VARS
    string currentLevel;
    string nextLevel;


    void Start () 
    {
        // get the name of the current level and work out the name of the next level
        currentLevel = SceneManager.GetActiveScene().name;
        if (currentLevel == "Level1")
            nextLevel = "Level2";
        else if (currentLevel == "Level2")
            nextLevel = "Finished";

        // get player's physics
        rigidBody = GetComponent<Rigidbody2D>();

        // get the player's extents (half of the player's bounds)
        boxExtents = GetComponent<BoxCollider2D>().bounds.extents;

        // get the player's animation controller
        animator = GetComponent<Animator>();

        // set the player's coin count to 0 + get total number of coins in level
        coinsCollected = 0;
        totalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    // each frame
    void Update () 
    {
        // if the direction of motion has the opposite sign to the way the sprite is currently facing
        if (rigidBody.velocity.x * transform.localScale.x < 0.0f)
            // flip the sprite in the x direction
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // get the velocity of the player and feed it to the animator
        float speedX = Mathf.Abs(rigidBody.velocity.x);
        animator.SetFloat("speedX", speedX);

        float speedY = Mathf.Abs(rigidBody.velocity.y);
        animator.SetFloat("speedY", speedY);

        // get a random float between 0 and 200. If the number is below 1, play the blink animation.
        float blinkVal = Random.Range(0.0f, 500.0f);
        if (blinkVal < 1.0f)
            animator.SetTrigger("blinkTrigger");

        // generate the UI string from the player's collected coins, then set the UI text to that string.
        string uiString = "x " + coinsCollected + "/" + totalCoins;
        uiText.text = uiString;

    }

    // since we're dealing with physics, use FixedUpdate.
    void FixedUpdate()
    {
        // get the user's horizontal input
        float h = Input.GetAxis("Horizontal");

        // stores the position of the bottom middle of the player's hitbox
        Vector2 bottom = new Vector2
        (
            transform.position.x,
            transform.position.y - boxExtents.y
        );
        
        // stores the x-length and y-length of an imaginary hitbox below the player
        Vector2 hitBoxSize = new Vector2
        (
            boxExtents.x * 1.95f,
            0.05f
        );

        /*
        RayCastHit2D draws a line between two objects.
        BoxCast checks the distance between two boxes.
        */
        RaycastHit2D result = Physics2D.BoxCast
        (
            bottom, // location of hitbox
            hitBoxSize, // size of hitbox
            0.0f,// angle of hitbox rotation (0)
            new Vector2(0.0f, -1.0f), // direction of player during collision (down)
            0.0f, // distance from location of hitbox
            1 << LayerMask.NameToLayer("Ground") // layermask for collision layer
        );
        /*
        The last argument is a little more complex. NameToLayer returns the number
        layer "Ground" is on, but this is just an int, not a layer mask.

        Layer masks are stored on specific bits, e.g. layer 8 is on 10000000.
        To obtain this, we use 1 <<; the bitwise shift operator. This shifts 1 across
        x amount of bits depending on the number returned by NameToLayer.
        */

        /*
        result.collider is the collider hit by the raycast,if applicable.
        we check there is a collider, because if there isn't one then we're in the air!

        result.normal.y is the y component of the ray normal to the surface of collision.
        (this should be a reflection off of the surface of the ground.)
        we check if this is substantial, because this would only be the case if the ray is
        being shot at the ground and it's right next to the player.
        */
        bool grounded =  result.collider != null && result.normal.y >  0.9f;

        if (grounded)
        {
            // if the player inputs a jump (up direction), add a force to the player.
            if (Input.GetAxis("Jump") > 0.0f)
                rigidBody.AddForce
                (
                    new Vector2(0.0f, jumpForce), // direction of force
                    ForceMode2D.Impulse // type of force
                );
            // else just stick to changing horizontal velocity based on user input.
            else
                rigidBody.velocity = new Vector2
                (
                    speed * h, // speed * horizontal input from player
                    rigidBody.velocity.y // don't change vertical velocity
                );
        }
        // if in the air, we want to minimize horizontal control
        else
        {
            // get the player's current horizontal velocity
            float vx = rigidBody.velocity.x;

            /*
            if the horizontal velocity * the horizontal input is below the threshold,
            apply a gentle force equal to the player's input * airControlForce to the
            player.
            */
            if (h * vx < airControlMax)
                rigidBody.AddForce(new Vector2(h * airControlForce, 0));
        }

    }

    // function to kill the player
    IEnumerator DoDeath()
    {
        // stop moving
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

        // disable the renderer (hides the player's sprite)
        GetComponent<Renderer>().enabled = false;

        // reload the level in 2 seconds
        yield return new WaitForSeconds(2); // wait 2s
        SceneManager.LoadScene(currentLevel); // reload level
    }

    // function to load the next level
    IEnumerator LoadNextLevel()
    {
        if ( nextLevel != "Finished")
        {
            GetComponent<Renderer>().enabled = false; // hide player
            yield return new WaitForSeconds(2); // wait 2s
            SceneManager.LoadScene(nextLevel); // load the next scene
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Coin")
        {
            Destroy(collider.gameObject);
            coinSound.Play();
            coinsCollected++;
        }

        if (collider.gameObject.tag == "Enemy")
        {
            // kill the player
            StartCoroutine(DoDeath());
        }

        if (collider.gameObject.tag == "LevelEnd")
        {
            collider.gameObject.SetActive(false);
            StartCoroutine(LoadNextLevel());
        }
    }

}