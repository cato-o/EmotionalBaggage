using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private bool isAlive = true;
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float speedIncreaseRate = 0;
    [SerializeField]
    private float horizSpeedMultiplier = 2;

    public float jumpForce = 200;
    public float gravityModifier = 1.5f;
    public bool isOnGround = true;

    float horizontalInput;
    Rigidbody rb;

    [SerializeField]
    private TMP_Text infoText;

    // using physics based movement

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;
    }

    // if player is alive, constantly move forward and also move horizontally based on keyboard input
    void FixedUpdate()
    {
        if(!isAlive) return;
        Vector3 forwardMove = transform.forward * speed * Time.fixedDeltaTime;
        Vector3 horizontalMove = transform.right * horizontalInput * horizSpeedMultiplier * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + forwardMove + horizontalMove);
    }

    void Update()
    {
        // get left/right and A/D keyboard input
        horizontalInput = Input.GetAxis("Horizontal");

        // jump if press space
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
        }

        // check player falls off side and restarts
        if (transform.position.x < -3 || transform.position.x > 3)
        {
            infoText.text = "Game Over";
            isAlive = false;
            Invoke("RestartGame", 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isOnGround = true;

        // check player is on ground and not jumping
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }

        // check if player collides with obstacle and restarts
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            infoText.text = "Game Over";
            isAlive = false;
            Invoke("RestartGame", 2);
        }
    }

    // resets game to start
    public void RestartGame()
    {
        Debug.Log("game over");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
