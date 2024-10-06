using UnityEngine;
using System.IO.Ports;

public class Character : CharacterStats
{
    private Rigidbody2D rb;
    public float jumpForce = 5f; // Adjust this value as needed
    public float moveSpeed = 5f;  // Adjust this value for horizontal movement
    private bool isGrounded;    
    private string value;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Check for serial data from Arduino
        if (gameManager.arduino.data_Stream.IsOpen && gameManager.arduino.data_Stream.BytesToRead > 0)
        {
            value = gameManager.arduino.data_Stream.ReadLine().Trim();
            Debug.Log("Received value: " + value); // Debug message

            // Handle jump
            if (value == "JUMP" && isGrounded)
            {
                Jump();
            }
            // Handle left movement
            else if (value == "LEFT")
            {
                MoveHorizontal(-moveSpeed);
            }
            // Handle right movement
            else if (value == "RIGHT")
            {
                MoveHorizontal(moveSpeed);
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TakeDamage(damage.GetValue());
        }

    }

    private void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            isGrounded = false; // Prevent double jumping
        }
    }

    private void MoveHorizontal(float direction)
    {
        rb.velocity = new Vector2(direction, rb.velocity.y);
    }

    // Ground detection
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Ensure the ground has this tag
        {
            isGrounded = true;
        }
    }

    void OnApplicationQuit()
    {
        if (gameManager.arduino.data_Stream != null && gameManager.arduino.data_Stream.IsOpen)
        {
            gameManager.arduino.data_Stream.Close();
        }
    }
}
