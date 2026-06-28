using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finishText;

    [SerializeField] private Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cameraTransform;

    private Vector2 input;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;
        isGrounded = false;

        finishText.gameObject.SetActive(false);
    }

    private void Update()
    {
        Debug.Log(rb.linearVelocity.y);
        animator.SetFloat("speed", input.magnitude);
        animator.SetFloat("verticalSpeed", isGrounded ? 0 : rb.linearVelocity.y);

        animator.SetBool("isGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            0.1f,
            groundLayer
        );

        var forward = cameraTransform.forward;
        var right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        var move = forward * input.y + right * input.x;

        if (move.sqrMagnitude > 0.001f)
        {
            var targetRotation = Quaternion.LookRotation(move);
            rb.MoveRotation(targetRotation);
        }

        rb.MovePosition(rb.position + move * (speed * Time.fixedDeltaTime));

        // falling gravity
        rb.AddForce(
            Physics.gravity * 1.5f,
            ForceMode.Acceleration
        );
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

    private void OnJump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        animator.SetTrigger("jumpTrigger");
    }
}