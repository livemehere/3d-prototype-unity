using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finishText;

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cameraTransform;
    
    private Rigidbody rb;
    private Vector2 input;

    void SetScore(int value)
    {
        score += value;
        scoreText.text = "Score: " + score;
        
        if (score >= 9)
        {
            finishText.gameObject.SetActive(true);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(GameObject e in enemies)
            {
                Destroy(e);
            }
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;
        
        finishText.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            0.1f,
          groundLayer
            );

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        forward.y = 0;
        right.y = 0;
        
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * input.y + right * input.x;

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            rb.MoveRotation(targetRotation); 
        }
        rb.MovePosition(rb.position + move * (speed * Time.fixedDeltaTime));
    }

    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>().normalized;
    }

    void OnJump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            SetScore(1);
        }

        if (other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            finishText.text = "Lose";
            finishText.gameObject.SetActive(true); 
        }
    }
}

