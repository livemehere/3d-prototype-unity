using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Timers
    
    [Header("Game")]
    public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finishText;

    [Header("Movement")] 
    public float speed = 0f;
    public float moveSpeed = 2f;
    public float sprintSpeed = 10f;
    public float rotationSmoothTime = 0.2f;
    public float speedSmoothTime = 0.1f;

    [Header("Jump")]
    public float coyoteTime = 0.2f;
    public float jumpHeight = 2f;
    public int jumpCount = 1;
    public int jumpCounter;
    private float coyoteTimer;

    [Header("Force")]
    public float gravity = -15.0f;
    public float verticalVelocity;
    public float fallTimeout = 0.15f;
    private float fallTimer;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool wasGrounded;
    [SerializeField] private bool isSprinting;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cameraTransform;

    private Vector2 input;
    
    private Animator animator;
    private CharacterController controller;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        score = 0;
        finishText.gameObject.SetActive(false);
    }

    private void Update()
    {
        var targetSpeed = input.magnitude * (isSprinting ? 2f : 1f);
        animator.SetFloat("speed", targetSpeed, 0.1f, Time.deltaTime);

        // animator.SetFloat("verticalSpeed", isGrounded ? 0 : rb.linearVelocity.y);

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.25f, groundLayer,  QueryTriggerInteraction.Ignore);
        animator.SetBool("isGrounded", isGrounded);
        
        JumpAndGravity();
        Move();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item")) SetScore(1);

        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            finishText.text = "Lose";
            finishText.gameObject.SetActive(true);
        }
    }

    private void SetScore(int value)
    {
        score += value;
        scoreText.text = "Score: " + score;

        if (score >= 9)
        {
            finishText.gameObject.SetActive(true);
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var e in enemies) Destroy(e);
        }
    }

    private void OnMove(InputValue value)
    {
        input = value.Get<Vector2>().normalized;
    }

    private void OnSprint(InputValue value)
    {
        var sprintValue = value.Get<float>();

        isSprinting = sprintValue > 0.5f;
    }

    private void ApplyJump()
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        animator.SetBool("isJumping", true);
    }

    private void OnJump()
    {
        if (jumpCounter == jumpCount)
        {
           // first jump 
           if (coyoteTimer > 0f)
           {
               ApplyJump();
               coyoteTimer = 0f;
               jumpCounter -= 1;
           }
        } else if (jumpCounter > 0)
        {
            // remain jump
            ApplyJump();
            jumpCounter -= 1;
        }
    }

    private void Move()
    {
        // user input
        var forward = cameraTransform.forward;
        var right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        var move = forward * input.y + right * input.x;

        // turn 
        if (move != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
        }

        // move
        var targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        if (move == Vector3.zero)
        {
            targetSpeed = 0f;
        }
        speed = Mathf.Lerp(speed, targetSpeed, speedSmoothTime);
        controller.Move( (move * (speed * Time.deltaTime)) + new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            // prevent reset count just jumped
            if (wasGrounded)
            {
                jumpCounter = jumpCount;
            }
            
            coyoteTimer = coyoteTime;
            fallTimer = fallTimeout;
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);

           if (verticalVelocity < 0f)
           {
               verticalVelocity = -2.0f;
           }
           
           wasGrounded = false;
        }
        else
        {
            wasGrounded = true;
            if (coyoteTimer > 0f)
            {
                coyoteTimer -= Time.deltaTime;
            }
            
            // for slop
            if (fallTimer > 0f)
            {
                fallTimer -= Time.deltaTime;
            }
            else
            {
                animator.SetBool("isFalling", true);
            }
        }
        

        
        
        // apply gravity
        verticalVelocity += gravity * Time.deltaTime; 
        
    }
}