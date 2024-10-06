using UnityEngine;
using System.IO.Ports;

public class Character : CharacterStats
{
    private Rigidbody2D rb;
    public float jumpForce = 5f; // Adjust this value as needed
    public float moveSpeed = 5f;  // Adjust this value for horizontal movement
    private bool isGrounded;

    // Arduino
    private readonly SerialPort data_Stream = new("COM5", 9600);
    private string value;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // try
        // {
        //     if (!data_Stream.IsOpen)
        //     {
        //         data_Stream.Open();
        //         Debug.Log("Serial port opened successfully.");
        //     }
        // }
        // catch (System.Exception e)
        // {
        //     Debug.LogError("Error opening serial port: " + e.Message);
        // }
    }

    void Update()
    {
        // Check for serial data from Arduino
        if (data_Stream.IsOpen && data_Stream.BytesToRead > 0)
        {
            value = data_Stream.ReadLine().Trim();
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
        if (data_Stream != null && data_Stream.IsOpen)
        {
            data_Stream.Close();
        }
    }
}
