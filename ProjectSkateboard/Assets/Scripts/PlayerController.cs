using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("The current player settings.")] private PlayerSettings playerSettings;
    [SerializeField, Tooltip("The layer for objects that can be jumped on.")] private LayerMask groundLayer;
    [SerializeField, Tooltip("The master list of possible combos.")] private Combo[] comboList;

    public static Action OnPaused;
    public static Action<Combo> OnComboSuccess;
    public static Action OnPlayerGrounded;
    public static Action<float> OnHypeTimeActivated;

    [Header("Debug Settings")]
    [SerializeField] private bool debugCombo;
    [SerializeField] private Combo debugComboInfo;

    private enum PlayerMode { GROUND, AIR, GRIND };

    private SpriteRenderer playerSpriteRenderer;
    private Sprite idleSprite;

    private Vector2 movement;
    private bool isMovingRight;
    private bool isGrounded;
    private bool onRail;
    private Transform groundCheck;
    private PlayerMode playerMode = PlayerMode.GROUND;
    private bool rotatingOnJump;
    private Vector3 targetRotation = Vector3.zero;

    private List<Combo.ComboKey> currentComboInput = new List<Combo.ComboKey>();
    private Combo currentCombo;
    private float currentComboCooldown;
    private float comboDurationCooldown;
    private bool comboInputActive;
    private bool comboDurationActive;
    private bool hypeTimeReady;

    private float railMomentum;
    private Vector2 railDirection;

    private Rigidbody2D rb2D;
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Jump.performed += _ => Jump();
        playerControls.Player.AirTrick1.performed += OnTrickButton;
        playerControls.Player.AirTrick2.performed += OnTrickButton;
        playerControls.Player.GrindTrick.performed += OnTrickButton;
        playerControls.Player.HypeTime.performed += _ => OnHypeTime();
        playerControls.Player.Pause.performed += _ => OnPaused?.Invoke();
    }

    private void Start()
    {
        playerSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        idleSprite = playerSpriteRenderer?.sprite;
        isMovingRight = true;

        rb2D = GetComponent<Rigidbody2D>();
        groundCheck = transform.Find("GroundCheck");
    }

    private void OnEnable()
    {
        playerControls?.Enable();
        HypeController.HypeMeterFull += ReadyHypeTime;
        HypeController.HypeTimeEnd += UnreadyHypeTime;
    }

    private void OnDisable()
    {
        playerControls?.Disable();
        HypeController.HypeMeterFull -= ReadyHypeTime;
        HypeController.HypeTimeEnd -= UnreadyHypeTime;
    }

    private void Update()
    {
        if (LevelManager.Instance != null && !LevelManager.Instance.IsGameActive())
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.1f, groundLayer);

        isGrounded = colliders.Length > 0;

        bool foundRail = false;

        if (isGrounded)
        {
            foreach (Collider2D collider in colliders)
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
                    if (playerSpriteRenderer.sprite != idleSprite)
                        playerSpriteRenderer.sprite = idleSprite;
                    OnPlayerGrounded?.Invoke();
                    break;
            }
        }

        playerMode = checkPlayerMode;

        if (comboInputActive)
        {
            if (currentComboCooldown < playerSettings.comboInputDelay)
                currentComboCooldown += Time.deltaTime;
            else
                PerformCombo();
        }

        if(comboDurationActive)
            UpdateComboCooldown();

        if (debugCombo)
        {
            currentCombo = debugComboInfo;
            PerformCombo();
            OnPlayerGrounded?.Invoke();
            debugCombo = false;
        }
    }

    private void FixedUpdate()
    {
        if (LevelManager.Instance != null && !LevelManager.Instance.IsGameActive())
            return;

        RotatePlayerOrientation();
        CheckPlayerDirection();

        if (!onRail)
        {
            //Move the player based on player input
            movement = playerControls.Player.Move.ReadValue<Vector2>();
            Vector2 moveForce = new Vector2(movement.x * (isGrounded ? playerSettings.moveSpeed : playerSettings.airSpeed), 0f);
            //Debug.Log("Move Force: " + moveForce);
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
    }

    /// <summary>
    /// Function called when a trick button is performed.
    /// </summary>
    private void OnTrickButton(InputAction.CallbackContext ctx)
    {
        if(playerMode != PlayerMode.GROUND && !comboDurationActive)
        {
            switch (ctx.action.name)
            {
                case "Air Trick 1":
                    currentComboInput.Add(Combo.ComboKey.TRICKONE);
                    break;
                case "Air Trick 2":
                    currentComboInput.Add(Combo.ComboKey.TRICKTWO);
                    break;
                case "Grind Trick":
                    if (playerMode == PlayerMode.GRIND)
                        currentComboInput.Add(Combo.ComboKey.GRINDTRICK);
                    else
                        return;
                    break;
            }

            comboInputActive = true;
            CheckCombo();
        }
    }

    /// <summary>
    /// Activates hype time when it's ready.
    /// </summary>
    private void OnHypeTime()
    {
        if (hypeTimeReady)
        {
            OnHypeTimeActivated?.Invoke(playerSettings.hypeModeTime);
            hypeTimeReady = false;
        }
    }

    private void CheckCombo()
    {
        foreach(Combo combo in comboList)
        {
            //Debug.Log("Checking " + combo.name + "...");

            //Continue if the current combo does not match the length
            if (currentComboInput.Count != combo.comboRequirement.Length)
                continue;

            bool comboMatched = true;
            for(int i = 0; i < combo.comboRequirement.Length; i++)
            {
                if (combo.comboRequirement[i] != currentComboInput[i])
                {
                    comboMatched = false;
                    break;
                }
            }

            if (comboMatched)
            {
                currentCombo = combo;
                currentComboCooldown = 0;
                break;
            }
        }
    }

    /// <summary>
    /// Tries to perform the combo stored.
    /// </summary>
    private void PerformCombo()
    {
        if(currentCombo != null)
        {
            OnComboSuccess?.Invoke(currentCombo);
            if(currentCombo.comboSprite != null)
            {
                playerSpriteRenderer.sprite = currentCombo.comboSprite;
                comboDurationActive = true;
                comboDurationCooldown = currentCombo.comboDuration;
            }
        }

        currentCombo = null;
        currentComboInput.Clear();
        comboInputActive = false;
    }

    /// <summary>
    /// Decrements the combo duration cooldown timer.
    /// </summary>
    private void UpdateComboCooldown()
    {
        comboDurationCooldown -= Time.deltaTime;

        if (comboDurationCooldown <= 0)
        {
            comboDurationActive = false;
            playerSpriteRenderer.sprite = idleSprite;
        }
    }

    /// <summary>
    /// Checks the player's direction and updates the sprite flip accordingly.
    /// </summary>
    private void CheckPlayerDirection()
    {
        if (!onRail)
        {
            if (isMovingRight && rb2D.velocity.normalized.x < 0)
            {
                isMovingRight = false;
                playerSpriteRenderer.flipX = !isMovingRight;
            }

            else if (rb2D.velocity.normalized.x > 0)
            {
                isMovingRight = true;
                playerSpriteRenderer.flipX = !isMovingRight;
            }
        }
        else
        {
            if (isMovingRight && railMomentum < 0)
            {
                isMovingRight = false;
                playerSpriteRenderer.flipX = !isMovingRight;
            }

            else if (railMomentum > 0)
            {
                isMovingRight = true;
                playerSpriteRenderer.flipX = !isMovingRight;
            }
        }
    }

    /// <summary>
    /// Rotates the player according to the surface that they are grounded on.
    /// </summary>
    private void RotatePlayerOrientation()
    {
        Vector2 origin = groundCheck.position;
        Vector2 direction = Vector2.down;
        float distance = 1f;

        // Cast the ray and store the hit information
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
        Debug.DrawRay(origin, direction * distance, Color.yellow);

        if (hit.collider != null)
        {
            // Get the angle of the ground and set it as the rotation for the player
            float groundAngle = Mathf.Atan2(hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -groundAngle);

            //Debug.Log("Rotating Player: Hit On " + hit.collider.name + " | Normal: " + hit.normal + ", Player Rotation: " + transform.eulerAngles);
        }

        CheckRotateInAir();
    }

    /// <summary>
    /// Checks to see if the player needs to rotate in the air.
    /// </summary>
    private void CheckRotateInAir()
    {
        if (rotatingOnJump)
        {
            // Calculate the interpolation factor based on the rotationSpeed
            float rotationFactor = playerSettings.rotationSpeed * Time.fixedDeltaTime;

            // Interpolate between the current rotation and the target rotation
            Vector3 currentEulerAngles = transform.eulerAngles;
            transform.eulerAngles = new Vector3(
                Mathf.LerpAngle(currentEulerAngles.x, targetRotation.x, rotationFactor),
                Mathf.LerpAngle(currentEulerAngles.y, targetRotation.y, rotationFactor),
                Mathf.LerpAngle(currentEulerAngles.z, targetRotation.z, rotationFactor));

            //Debug.Log("Rotating Jump: " + currentEulerAngles + " to " + transform.eulerAngles);

            // Check if we've reached the upright orientation
            if (Vector3.Distance(transform.eulerAngles, targetRotation) < 0.1f)
            {
                transform.eulerAngles = Vector3.zero;
                rotatingOnJump = false;
            }
        }
    }

    /// <summary>
    /// Allows the player to jump if grounded.
    /// </summary>
    private void Jump()
    {
        if (LevelManager.Instance != null && !LevelManager.Instance.IsGameActive())
            return;

        if (isGrounded)
        {
            rb2D.AddForce(transform.up * playerSettings.jumpForce, ForceMode2D.Impulse);
            rotatingOnJump = true;
            GameManager.Instance.AudioManager.PlayOneShot(AudioManager.GameSound.Sound.PlayerJump);
        }
    }

    private void ReadyHypeTime() => hypeTimeReady = true;
    private void UnreadyHypeTime() => hypeTimeReady = false;

    /// <summary>
    /// Function called when the player enters a rail.
    /// </summary>
    private void OnStartRail()
    {
        //Debug.Log("Rail Entered!");
        if (playerSpriteRenderer.sprite != idleSprite)
            playerSpriteRenderer.sprite = idleSprite;

        playerMode = PlayerMode.GRIND;

        float startingMomentum = (Mathf.Abs(rb2D.velocity.x) < playerSettings.railSpeed) ? playerSettings.railSpeed : Mathf.Abs(rb2D.velocity.x);

        railMomentum = (isMovingRight ? 1: -1) * startingMomentum;

        railDirection = transform.right * railMomentum;
        Debug.DrawRay(groundCheck.position, railDirection, Color.red);

        //Start grind combo
        OnComboSuccess?.Invoke(comboList[2]);
    }

    private void RailMovement()
    {
        // Get the rail's normal vector
        Vector2 railNormal = GetRailNormal();

        // Calculate the movement direction based on the rail normal
        Vector2 moveDirection = new Vector2(railNormal.y, -railNormal.x);
        Vector2 moveForce = moveDirection * railMomentum;
        transform.position = new Vector2(transform.position.x, transform.position.y) + moveForce * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Gets the vector of the rail's surface.
    /// </summary>
    /// <returns>The vector of the rail's surface, showing its angle.</returns>
    private Vector2 GetRailNormal()
    {
        Vector2 origin = groundCheck.position;
        Vector2 direction = Vector2.down;
        float distance = 1.5f;

        // Cast the ray and store the hit information
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, groundLayer);
        Debug.DrawRay(origin, direction * distance, Color.blue);

        if (hit.collider != null)
            return hit.normal;

        Debug.Log("Rail Not Found.");

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
