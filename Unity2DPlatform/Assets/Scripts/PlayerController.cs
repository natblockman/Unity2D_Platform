using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float movementInputDirection;

    

    public float movementSpeed = 10.0f;
    public float groundCheckRadius = 0.1f;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier=0.95f;
    public float variableJumpHeightMultiplier=0.5f;
    public float wallHopForce;
    public float wallJumpForce;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public int amountOfJumpLeft;
    private int facingDirection=1;
    public int amountOfJump=1;

    private bool IsFacingRight=true;
    private bool IsWalking;
    private bool canJump;
    public bool IsTouchingWall;
    public bool IsGrounded;
    public bool IsWallSliding;

    private Rigidbody2D rb;

    public LayerMask whatIsGround;

    private Animator anim;

    public Transform groundedCheck;
    public Transform wallCheck;

    public float JumpForce = 16.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJump = amountOfJumpLeft;
        wallHopDirection.Normalize();
        wallJumpDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimation();
        CheckIfCanJump();
        CheckIfWallSliding();
    }
    private void CheckIfWallSliding()
    {
        if (IsTouchingWall&& !IsGrounded && rb.velocity.y < 0)
        {
          
            IsWallSliding = true;
            
        }
        else
        {
            
            IsWallSliding = false;
            
        }
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurrounding();
    }
    private void CheckIfCanJump()
    {
        if ((IsGrounded && rb.velocity.y <= 0)||IsWallSliding)
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

        IsTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
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
        anim.SetBool("IsWallSliding", IsWallSliding);
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity=new Vector2(rb.velocity.x, rb.velocity.y*variableJumpHeightMultiplier);
        }
    }
    private void Jump()
    {
        if (canJump&&!IsWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            amountOfJumpLeft--;
        }
        else if (IsWallSliding && movementInputDirection == 0 && canJump)
        {
            IsWallSliding = false;
            amountOfJumpLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallHopDirection.x * -facingDirection, wallHopForce * wallHopDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
        else if ((IsWallSliding || IsTouchingWall) && movementInputDirection != 0 && canJump)
        {

            IsWallSliding = false;
            amountOfJumpLeft--;
            Vector2 forceToAdd = new Vector2(wallHopForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
        }
    }

    private void ApplyMovement()
    {
        if (IsGrounded)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }else if (!IsGrounded&&!IsWallSliding&&movementInputDirection!=0)
        {
            Vector2 forceToAdd = new Vector2(movementForceInAir * movementInputDirection, 0);
            rb.AddForce(forceToAdd);
            
            if (Mathf.Abs(rb.velocity.x) > movementSpeed)
            {
                rb.velocity=new Vector2(movementSpeed*movementInputDirection,rb.velocity.y);
            }
        }else if (!IsGrounded && !IsWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x*airDragMultiplier,rb.velocity.y);
        }
        

        if (IsWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void Flip()
    {
        if (!IsWallSliding)
        {
            facingDirection *= -1;
            IsFacingRight = !IsFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundedCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
