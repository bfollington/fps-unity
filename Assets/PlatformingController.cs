using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlatformingController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float sprintSpeed = 9f;
    public float acceleration = 8f;
    public float airControl = 0.6f;

    [Header("Jump")]
    public float jumpForce = 5f;
    public float gravity = -20f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.2f;
    public int airJumps = 1;

    [Header("Look")]
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;
    public bool lockCursor = true;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation;
    private float coyoteTimeRemaining;
    private float jumpBufferTimeRemaining;
    private int remainingAirJumps;
    private bool wasGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        remainingAirJumps = airJumps;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleLook();
    }

    void HandleMovement()
    {
        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Get input direction in world space
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        input = Vector3.ClampMagnitude(input, 1f);
        Vector3 direction = transform.TransformDirection(input);

        // Apply different movement for ground/air
        if (controller.isGrounded)
        {
            velocity.x = Mathf.Lerp(velocity.x, direction.x * targetSpeed, acceleration * Time.deltaTime);
            velocity.z = Mathf.Lerp(velocity.z, direction.z * targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            velocity.x += direction.x * targetSpeed * airControl * Time.deltaTime;
            velocity.z += direction.z * targetSpeed * airControl * Time.deltaTime;

            // Cap air speed
            Vector2 horizontalVel = new Vector2(velocity.x, velocity.z);
            if (horizontalVel.magnitude > targetSpeed)
            {
                horizontalVel = horizontalVel.normalized * targetSpeed;
                velocity.x = horizontalVel.x;
                velocity.z = horizontalVel.y;
            }
        }
    }

    void HandleJump()
    {
        bool isGrounded = controller.isGrounded;

        // Update coyote time
        if (wasGrounded && !isGrounded)
        {
            coyoteTimeRemaining = coyoteTime;
        }
        else if (!wasGrounded && isGrounded)
        {
            remainingAirJumps = airJumps;
        }
        wasGrounded = isGrounded;

        if (coyoteTimeRemaining > 0f)
            coyoteTimeRemaining -= Time.deltaTime;

        // Update jump buffer
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimeRemaining = jumpBufferTime;

        if (jumpBufferTimeRemaining > 0f)
            jumpBufferTimeRemaining -= Time.deltaTime;

        // Apply jump
        if (jumpBufferTimeRemaining > 0f && (isGrounded || coyoteTimeRemaining > 0f || remainingAirJumps > 0))
        {
            velocity.y = jumpForce;
            jumpBufferTimeRemaining = 0f;
            coyoteTimeRemaining = 0f;

            if (!isGrounded && coyoteTimeRemaining <= 0f)
                remainingAirJumps--;
        }

        // Apply gravity
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;

            // Higher fall gravity
            if (velocity.y < 0)
                velocity.y += gravity * 0.5f * Time.deltaTime;

            // Cut jump short if button released
            if (!Input.GetKey(KeyCode.Space) && velocity.y > 0)
                velocity.y *= 0.5f;
        }
        else if (velocity.y < 0)
        {
            velocity.y = gravity * Time.deltaTime; // Small downward force when grounded
        }

        // Apply movement
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        if (!lockCursor) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
