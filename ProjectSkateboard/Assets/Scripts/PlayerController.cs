using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("The current player settings.")] private PlayerSettings playerSettings;
    [SerializeField, Tooltip("The layer for objects that can be jumped on.")] private LayerMask groundLayer;
    [SerializeField, Tooltip("The master list of possible combos.")] private Combo[] comboList;

    public static Action<Combo> OnComboSuccess;
    public static Action OnPlayerGrounded;

    [Header("Debug Settings")]
    [SerializeField] private bool debugCombo;
    [SerializeField] private Combo debugComboInfo;

    private enum PlayerMode { GROUND, AIR, GRIND };

    private Vector2 movement;
    private bool isGrounded;
    private bool onRail;
    private Transform groundCheck;
    private PlayerMode playerMode = PlayerMode.GROUND;

    private float railMomentum;
    private Vector2 railDirection;

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
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, groundLayer);

        isGrounded = colliders.Length > 0;

        bool foundRail = false;

        if (isGrounded)
        {
            foreach(Collider2D collider in colliders)
            {
                if (collider.CompareTag("Rail"))
                {
                    foundRail = true;
                    break;
                }
            }

            onRail = foundRail;
        }

        if (onRail)
        {
            if (playerMode != PlayerMode.GRIND)
                OnStartRail();
        }

        PlayerMode checkPlayerMode = DeterminePlayerMode();

        if(playerMode != checkPlayerMode)
        {
            switch (checkPlayerMode)
            {
                case PlayerMode.GROUND:
                    OnPlayerGrounded?.Invoke();
                    break;
            }
        }

        playerMode = checkPlayerMode;

        if (debugCombo)
        {
            OnComboSuccess?.Invoke(debugComboInfo);
            OnPlayerGrounded?.Invoke();
            debugCombo = false;
        }
    }

    private void FixedUpdate()
    {
        if (!onRail)
        {
            //Move the player based on player input
            movement = playerControls.Player.Move.ReadValue<Vector2>();
            Vector2 moveForce = new Vector2(movement.x * (isGrounded ? playerSettings.moveSpeed : playerSettings.airSpeed), 0f);
            rb2D.AddForce(moveForce);

            //If the player is not trying to move, start to decelerate
            if (movement.x == 0f && rb2D.velocity.magnitude > 0f)
            {
                Vector2 decelerationForce = -rb2D.velocity.normalized * playerSettings.deceleration;
                rb2D.AddForce(decelerationForce);
            }

            //Keep the player at their maximum speed
            if (rb2D.velocity.magnitude > playerSettings.maxSpeed)
                rb2D.velocity = rb2D.velocity.normalized * playerSettings.maxSpeed;
        }
        else
            RailMovement();

        RotatePlayerOrientation();
    }

    private void RotatePlayerOrientation()
    {
        Vector2 origin = groundCheck.position;
        Vector2 direction = Vector2.down;
        float distance = 0.1f;

        // Cast the ray and store the hit information
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
        Debug.DrawRay(origin, direction, Color.yellow);

        if (hit.collider != null)
        {
            // Get the angle of the ground and set it as the rotation for the player
            float groundAngle = Mathf.Atan2(hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -groundAngle);
        }
    }

    /// <summary>
    /// Allows the player to jump if grounded.
    /// </summary>
    private void Jump()
    {
        if (isGrounded)
            rb2D.AddForce(transform.up * playerSettings.jumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Function called when the player enters a rail.
    /// </summary>
    private void OnStartRail()
    {
        Debug.Log("Rail Entered!");
        playerMode = PlayerMode.GRIND;

        railMomentum = rb2D.velocity.x > 0 ? 1 : -1;

        railDirection = transform.right * railMomentum;
        Debug.DrawRay(groundCheck.position, railDirection, Color.red);

        //Start grind combo
        OnComboSuccess?.Invoke(comboList[6]);
    }

    private void RailMovement()
    {
        // Get the rail's normal vector
        Vector2 railNormal = GetRailNormal(); // Implement this function to get the rail's normal vector.

        // Calculate the movement direction based on the rail normal
        Vector2 moveDirection = new Vector2(railNormal.y, -railNormal.x);
        Vector2 moveForce = moveDirection * railMomentum * playerSettings.railSpeed;
        transform.position = new Vector2(transform.position.x, transform.position.y) + moveForce * Time.fixedDeltaTime;
    }

    private Vector2 GetRailNormal()
    {
        Vector2 origin = groundCheck.position;
        Vector2 direction = Vector2.down;
        float distance = 0.1f;

        // Cast the ray and store the hit information
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
        Debug.DrawRay(origin, direction, Color.yellow);

        if (hit.collider != null)
            return hit.normal;

        return Vector2.up;
    }

    /// <summary>
    /// Determines what movement mode the player is in.
    /// </summary>
    /// <returns>The movement mode for the player.</returns>
    private PlayerMode DeterminePlayerMode()
    {
        if (onRail)
            return PlayerMode.GRIND;

        if (!isGrounded)
            return PlayerMode.AIR;

        return PlayerMode.GROUND;
    }

}
