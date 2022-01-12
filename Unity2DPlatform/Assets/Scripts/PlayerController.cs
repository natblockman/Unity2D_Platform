using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float movementInputDirection;
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;

    public float movementSpeed = 10.0f;
    public float groundCheckRadius = 0.1f;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float movementForceInAir;
    public float airDragMultiplier=0.95f;
    public float variableJumpHeightMultiplier=0.5f;
    public float wallHopForce;
    public float wallJumpForce;
    public float jumpTimerSet=0.15f;
    public float turnTimerSet=0.1f;
    public float wallJumpTimerSet=0.5f;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public int amountOfJumpLeft;
    public int amountOfJump=1;

    private int facingDirection = 1;
    private int lastWallJumpDirection;

    private bool IsFacingRight=true;
    private bool IsWalking;
    private bool canJump;
    public bool IsTouchingWall;
    private bool IsGrounded;
    private bool IsWallSliding;
    private bool canNormalJump;
    private bool isAttempingToJump;
    private bool canWallJump;
    private bool checkJumpMultiplier;
    private bool canMove;
    private bool canFlip;
    private bool hasWallJumped;

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
        CheckJump();
    }
    private void CheckIfWallSliding()
    {
        if (IsTouchingWall && movementInputDirection == facingDirection && rb.velocity.y<0)
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
        if (IsGrounded && rb.velocity.y <= 0.01f)
        {
            amountOfJumpLeft = amountOfJump;
            
        }

        if (IsTouchingWall)
        {
            canWallJump = true;
        }
       

        if(amountOfJumpLeft<=0)
        {
            canNormalJump = false;
        }
        else
        {
            canNormalJump = true;
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
            if (IsGrounded || (amountOfJumpLeft > 0 && !IsTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttempingToJump = true;
            }
        }
        if (Input.GetButtonDown("Horizontal") && IsTouchingWall)
        {
            if (!IsGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;
                turnTimer = turnTimerSet;
            }
        }
        if (!canMove)
        {
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                canMove=true;
                canFlip=true;
            }
        }

        if (checkJumpMultiplier&&!Input.GetButton("Jump"))
        {
            checkJumpMultiplier = false;
            rb.velocity=new Vector2(rb.velocity.x, rb.velocity.y*variableJumpHeightMultiplier);
        }
    }
    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            if (!IsGrounded && IsTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection)
            {
                WallJump();
            }else if (IsGrounded)
            {
                NormalJump();
            }
        }
        if(isAttempingToJump)
        {
            jumpTimer-=Time.deltaTime;
        }
        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0.0f);
                hasWallJumped = false;
            }else if (wallJumpTimer <= 0) 
            {
                hasWallJumped=false;
            }
            else
            {
                wallJumpTimer-=Time.deltaTime;
            }
        }
       
        
    }
    private void NormalJump()
    {
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
            amountOfJumpLeft--;
            jumpTimer = 0;
            isAttempingToJump=false;
            checkJumpMultiplier = true;
        }
    }
    private void WallJump()
    {
        if (canWallJump)
        {
            rb.velocity=new Vector2(rb.velocity.x,0.0f);
            IsWallSliding = false;
            amountOfJumpLeft = amountOfJump;
            amountOfJumpLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y);
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            jumpTimer = 0;
            isAttempingToJump = false;
            checkJumpMultiplier = true;
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
        }
    }

    private void ApplyMovement()
    {
        if (!IsGrounded && !IsWallSliding && movementInputDirection == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
       
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
        if (!IsWallSliding&&canFlip)
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
