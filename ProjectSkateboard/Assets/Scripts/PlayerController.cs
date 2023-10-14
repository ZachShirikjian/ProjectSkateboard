using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("The current player settings.")] private PlayerSettings playerSettings;
    [SerializeField, Tooltip("The layer for objects that can be jumped on.")] private LayerMask groundLayer;

    private Vector2 movement;
    private bool isGrounded;
    private Transform groundCheck;

    private Rigidbody2D rb2D;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Jump.performed += _ => Jump();
    }

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck");
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void FixedUpdate()
    {
        //Move the player based on player input
        movement = playerControls.Player.Move.ReadValue<Vector2>();
        Vector2 moveForce = new Vector2(movement.x * (isGrounded? playerSettings.moveSpeed : playerSettings.airSpeed), 0f);
        rb2D.AddForce(moveForce);

        //If the player is not trying to move, start to decelerate
        if(movement.x == 0f && rb2D.velocity.magnitude > 0f)
        {
            Vector2 decelerationForce = -rb2D.velocity.normalized * playerSettings.deceleration;
            rb2D.AddForce(decelerationForce);
        }

        //Keep the player at their maximum speed
        if (rb2D.velocity.magnitude > playerSettings.maxSpeed)
            rb2D.velocity = rb2D.velocity.normalized * playerSettings.maxSpeed;
    }

    /// <summary>
    /// Allows the player to jump if grounded.
    /// </summary>
    private void Jump()
    {
        if (isGrounded)
            rb2D.AddForce(Vector3.up * playerSettings.jumpForce, ForceMode2D.Impulse);
    }

}
