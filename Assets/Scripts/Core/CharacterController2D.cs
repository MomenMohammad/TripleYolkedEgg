using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    [Header("Player Settings")]
[SerializeField, Tooltip("Player number (1 or 2)")]
    protected int playerNumber = 1;

    public int PlayerNumber => playerNumber;
    public bool IsInvincible => isInvincible;
    public float Facing => sr.flipX ? -1f : 1f;

    [Header("Data")]
[SerializeField, Tooltip("Data asset for this character")]
    protected CharacterData characterData;

    [Header("Movement Tweaks")]
    [SerializeField] protected float acceleration = 60f;
    [SerializeField] protected float friction = 25f;
    [SerializeField] protected float airAcceleration = 40f;
    [SerializeField] protected float airFriction = 10f;
    [SerializeField] protected float gravityScaleFalling = 5f;
    [SerializeField] protected float gravityScaleRising = 3f;
    [SerializeField] protected float fastFallMultiplier = 2.5f;
    
    [Header("Responsiveness")]
    [SerializeField] protected float coyoteTime = 0.15f;
    [SerializeField] protected float jumpBufferTime = 0.15f;

    [Header("Visual Juice")]
    [SerializeField] protected float maxTiltAngle = 10f;
    [SerializeField] protected float tiltSpeed = 15f;
    [SerializeField] protected float visualScaleMult = 1f;

    [Header("Core References")]
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckRadius = 0.2f;

    [Header("Combat")]
    [SerializeField] protected Hitbox hitbox;
    protected bool isAttacking = false;

    [Header("Stats (Cached from Data)")]
    protected float moveSpeed;
    protected float jumpForce;
    protected float weight;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer sr;

    protected Vector2 moveInput;
    protected bool isGrounded;
    protected bool canDoubleJump = true;
    protected bool isFastFalling = false;
    
    private float coyoteCounter;
    private float jumpBufferCounter;
    private Vector3 originalScale;
    private float currentTilt;

    [Header("Dash Settings")]
    [SerializeField] protected float dashForce = 20f;
    [SerializeField] protected float dashDuration = 0.2f;
    [SerializeField] protected float dashCooldown = 0.8f;
    protected bool isDashing = false;
    protected bool canDash = true;
    protected bool isInvincible = false;

    // Animation Hashes
    protected static readonly int SpeedHash = Animator.StringToHash("Speed");
    protected static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int HitHash = Animator.StringToHash("Hit");
    protected static readonly int DashHash = Animator.StringToHash("Dash");

    public void SetPlayerNumber(int number)
    {
        playerNumber = number;
    }

    protected virtual void Awake()
{
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalScale = transform.localScale;

        if (characterData != null)
        {
            InitializeStats();
            Debug.Log($"{gameObject.name} Initialized: Speed={moveSpeed}, Weight={weight}");
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} CharacterData is NULL!");
            moveSpeed = 5f;
            jumpForce = 10f;
            weight = 1f;
        }
    }

    protected void InitializeStats()
    {
        moveSpeed = characterData.speed;
        jumpForce = characterData.jumpForce;
        weight = characterData.weight;
    }

    protected virtual void Update()
    {
        CheckGround();
        HandleResponsiveness();
        HandleInput();
        ApplyAnimationState();
        ApplyJuice();
    }

    private void HandleInput()
    {
        if (Keyboard.current != null)
        {
            float x = 0;
            if (playerNumber == 1)
            {
                if (Keyboard.current.dKey.isPressed) x += 1;
                if (Keyboard.current.aKey.isPressed) x -= 1;
                if (Keyboard.current.wKey.wasPressedThisFrame) InputJump();
                if (Keyboard.current.gKey.wasPressedThisFrame) InputLightAttack();
                if (Keyboard.current.kKey.wasPressedThisFrame) InputSpecialAttack();
                if (Keyboard.current.jKey.wasPressedThisFrame) InputDodge();
            }
            else
            {
                if (Keyboard.current.rightArrowKey.isPressed) x += 1;
                if (Keyboard.current.leftArrowKey.isPressed) x -= 1;
                if (Keyboard.current.upArrowKey.wasPressedThisFrame) InputJump();
                if (Keyboard.current.numpad1Key.wasPressedThisFrame) InputLightAttack();
                if (Keyboard.current.numpad2Key.wasPressedThisFrame) InputSpecialAttack();
                if (Keyboard.current.numpad3Key.wasPressedThisFrame) InputDodge();
            }

            if (x != 0) moveInput = new Vector2(x, 0);
            else moveInput = Vector2.zero;
        }
    }

    private void ApplyJuice()
    {
        if (sr == null) return;

        // Smooth Tilt based on velocity
        float targetTilt = -moveInput.x * maxTiltAngle;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
        sr.transform.localRotation = Quaternion.Euler(0, 0, currentTilt);

        // Standard flipX logic
        if (moveInput.x > 0.1f) sr.flipX = false;
        else if (moveInput.x < -0.1f) sr.flipX = true;

        // Ensure scale is normal
        sr.transform.localScale = Vector3.one * visualScaleMult;
    }

    private void HandleResponsiveness()
    {
        if (isGrounded) coyoteCounter = coyoteTime;
        else coyoteCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
            if (coyoteCounter > 0) ExecuteJump();
        }
    }

    private void OnGUI()
    {
        if (Camera.main == null) return;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(screenPos.x - 50, Screen.height - screenPos.y - 70, 200, 50), 
            $"P{playerNumber}: G={isGrounded}");
    }

    protected virtual void FixedUpdate()
    {
        if (!isAttacking && !isDashing)
        {
            ApplyMovement();
        }
        
        if (!isDashing)
        {
            ApplyGravityScale();
        }
        else
        {
            rb.gravityScale = 0; // No gravity during dash
        }
    }

    protected virtual void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && !wasGrounded)
        {
            StartCoroutine(LandSquash());
        }

        if (isGrounded)
        {
            canDoubleJump = true;
            isFastFalling = false;
        }
    }

    private IEnumerator LandSquash()
    {
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    protected virtual void ApplyMovement()
    {
        float targetSpeed = moveInput.x * moveSpeed;
        float currentAccel = isGrounded ? acceleration : airAcceleration;
        float currentFriction = isGrounded ? friction : airFriction;

        if (isGrounded && Mathf.Abs(moveInput.x) > 0.01f && Mathf.Sign(moveInput.x) != Mathf.Sign(rb.linearVelocity.x) && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            currentAccel *= 3f; 
        }

        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? currentAccel : currentFriction;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right);

        if (!isGrounded && moveInput.y < -0.5f && rb.linearVelocity.y < 0)
        {
            isFastFalling = true;
        }

        if (isFastFalling)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y - (Physics2D.gravity.y * -1f * fastFallMultiplier * Time.fixedDeltaTime));
        }
    }

    protected virtual void ApplyGravityScale()
    {
        if (rb.linearVelocity.y < -0.1f) rb.gravityScale = gravityScaleFalling;
        else rb.gravityScale = gravityScaleRising;
    }

    protected virtual void ApplyAnimationState()
    {
        if (animator == null) return;
        animator.SetBool(IsGroundedHash, isGrounded);
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public virtual void InputJump()
    {
        if (isAttacking) return;
        
        if (coyoteCounter > 0)
        {
            ExecuteJump();
        }
        else if (canDoubleJump)
        {
            ExecuteJump();
            canDoubleJump = false; // Consume air jump
        }
    }

    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpBufferCounter = 0;
        coyoteCounter = 0;
        isGrounded = false; // Force grounded to false immediately
        StartCoroutine(JumpStretch());
    }

    private IEnumerator JumpStretch()
    {
        transform.localScale = new Vector3(originalScale.x * 0.8f, originalScale.y * 1.3f, originalScale.z);
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    public virtual void InputLightAttack() 
    {
        if (isAttacking) return;
        AttackData data = FindAttack(AttackType.Light);
        if (data != null) StartCoroutine(AttackSequence(data));
    }

    public virtual void InputSpecialAttack() 
{
        if (isAttacking) return;
        AttackData data = FindAttack(AttackType.Special);
        if (data != null) StartCoroutine(AttackSequence(data));
    }

    public virtual void InputDodge() 
    {
        if (canDash && !isDashing && !isAttacking)
        {
            StartCoroutine(DashSequence());
        }
    }

    protected virtual IEnumerator DashSequence()
    {
        canDash = false;
        isDashing = true;
        isInvincible = true;
        
        animator.SetTrigger(DashHash);
        
        // Dash direction based on input or facing
        float dashDir = moveInput.x != 0 ? Mathf.Sign(moveInput.x) : Facing;
        
        // Visual feedback
StartCoroutine(DashVisualEffect());

        float timer = 0;
        while (timer < dashDuration)
        {
            rb.linearVelocity = new Vector2(dashDir * dashForce, 0f);
            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        isInvincible = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private IEnumerator DashVisualEffect()
    {
        Color originalColor = sr.color;
        sr.color = new Color(1f, 1f, 1f, 0.5f); // Fade out
        
        while (isDashing)
        {
            // Simple pulse or ghosting could go here
            yield return new WaitForSeconds(0.05f);
        }
        
        sr.color = originalColor;
    }

    protected AttackData FindAttack(AttackType type)
{
        if (characterData == null) return null;
        return characterData.attacks.Find(a => a.type == type);
    }

    protected AttackData FindAttack(string namePart)
    {
        if (characterData == null) return null;
        return characterData.attacks.Find(a => a.attackName.Contains(namePart));
    }

    protected virtual IEnumerator AttackSequence(AttackData attack)
    {
        if (attack == null)
        {
            Debug.LogWarning($"{gameObject.name} attempted to start a null attack sequence.");
            yield break;
        }

        isAttacking = true;
animator.SetTrigger(AttackHash);
        
        Debug.Log($"{gameObject.name} starting attack: {attack.attackName}");

        yield return new WaitForSeconds(attack.startupFrames / 60f);

        if (hitbox != null)
        {
            hitbox.Initialize(attack, gameObject);
            hitbox.Activate();
            yield return new WaitForSeconds(attack.activeFrames / 60f);
            hitbox.Deactivate();
        }

        yield return new WaitForSeconds(attack.recoveryFrames / 60f);
        isAttacking = false;
    }

    public void ApplyHitstun(int frames)
    {
        if (frames <= 0) return;
        StopAllCoroutines(); 
        isAttacking = true; 
        StartCoroutine(HitstunSequence(frames));
    }

    private IEnumerator HitstunSequence(int frames)
    {
        animator.SetTrigger(HitHash); 
        yield return new WaitForSeconds(frames / 60f);
        isAttacking = false;
    }
}