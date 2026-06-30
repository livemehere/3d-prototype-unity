using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private int UPPER_LAYER = 1;

    [Header("Game")] public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finishText;

    [Header("Movement")] public float speed;
    public float moveSpeed = 2f;
    public float sprintSpeed = 10f;
    public float rotationSmoothTime = 0.2f;
    public float speedSmoothTime = 0.1f;

    [Header("Jump")] public float coyoteTime = 0.2f;
    public float jumpHeight = 2f;
    public int jumpCount = 1;
    private int jumpCounter;
    private float coyoteTimer;

    [Header("Gravity")] public float gravity = -15.0f;
    public float fallTimeout = 0.15f;
    private float verticalVelocity;
    private float fallTimer;
    private bool isGrounded;
    private bool wasGrounded;

    // drag and drop
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cameraTransform;

    [Header("Aim")] [SerializeField] private LayerMask aimLayer;
    [SerializeField] private Transform aimTarget;
    [SerializeField] private float shootRange = 999f;
    private bool isAiming;

    [Header("Weapon")] public GameObject startWeaponPrefab;
    public Transform rightHandSocket;
    private Weapon currentWeapon;

    // user input state
    private Vector2 input;
    private bool isSprinting;

    // search from self
    private Animator animator;
    private CharacterController controller;

    private void EquipWeapon(GameObject weaponPrefab)
    {
        if (currentWeapon)
        {
            Destroy(currentWeapon.gameObject);
        }

        GameObject weaponObj = Instantiate(
            weaponPrefab,
            rightHandSocket.position,
            rightHandSocket.rotation,
            rightHandSocket
        );
        currentWeapon = weaponObj.GetComponent<Weapon>();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        score = 0;
        finishText.gameObject.SetActive(false);
        EquipWeapon(startWeaponPrefab);
    }

    private void Update()
    {
        GroundCheck();
        GravityAndVerticalState();
        Move();

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange, aimLayer))
        {
            aimTarget.position = hit.point;
        }
        else
        {
            aimTarget.position = ray.origin + ray.direction * shootRange;
        }
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
                animator.SetTrigger("jumpTrigger");
            }
        }
        else if (jumpCounter > 0)
        {
            // remain jump
            ApplyJump();
            jumpCounter -= 1;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.25f, groundLayer, QueryTriggerInteraction.Ignore);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void Move()
    {
        // animate blending
        var animateTargetSpeed = input.magnitude * (isSprinting ? 2f : 1f);
        animator.SetFloat("speed", animateTargetSpeed, 0.1f, Time.deltaTime);

        // user input
        var forward = cameraTransform.forward;
        var right = cameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        var move = forward * input.y + right * input.x;

        // turn 
        if (isAiming)
        {
            Vector3 aimDir = aimTarget.position - transform.position;
            aimDir.y = 0f;
            aimDir.Normalize();

            if (aimDir != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(aimDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSmoothTime
                );
            }
        }
        else if (move != Vector3.zero)
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
        controller.Move((move * (speed * Time.deltaTime)) + new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    private void GravityAndVerticalState()
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

    private void OnAim(InputValue value)
    {
        var isPressed = value.Get<float>() > 0.5f;
        animator.SetBool("isAiming", isPressed);
        isAiming = isPressed;
    }

    private void OnAttack()
    {
        if (currentWeapon)
        {
            animator.Play("Armature|Pistol_Shoot", UPPER_LAYER, 0);
            currentWeapon.Fire(aimTarget.transform.position);
        }
    }
}