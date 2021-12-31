using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float movementInputDirection;
    public float movementSpeed = 10.0f;
    public float groundCheckRadius = 0.1f;

    public int amountOfJumpLeft;
    public int amountOfJump=1;

    private bool IsFacingRight=true;
    private bool IsWalking;
    private bool canJump;
    public bool IsGrounded;

    private Rigidbody2D rb;

    public LayerMask whatIsGround;

    private Animator anim;

    public Transform groundedCheck;

    public float JumpForce = 16.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJump = amountOfJumpLeft;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurrounding();
    }
    private void CheckIfCanJump()
    {
        if (IsGrounded && rb.velocity.y <= 0)
        {
            amountOfJumpLeft = amountOfJump;
            
        }

        if(amountOfJumpLeft<=0)
        {
            canJump = false;
        }
        else
        {
            canJump = true;
        }
    }
    private void CheckSurrounding()
    {
        IsGrounded = Physics2D.OverlapCircle(groundedCheck.position, groundCheckRadius, whatIsGround);
    }
    private void CheckMovementDirection()
    {
        if (IsFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!IsFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (rb.velocity.x != 0)
        {
            IsWalking = true;
        }
        else
        {
            IsWalking = false;
        }
    }
    private void UpdateAnimation()
    {
        anim.SetBool("IsWalking",IsWalking);
        anim.SetBool("IsGrounded", IsGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    private void Jump()
    {
        if (canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            amountOfJumpLeft--;
        }
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(movementSpeed*movementInputDirection,rb.velocity.y);
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundedCheck.position, groundCheckRadius);
    }
}
