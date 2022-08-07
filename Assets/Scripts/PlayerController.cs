using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D rb;
    public SpriteRenderer playerSr;
    public float jumpForce = 4f;
    public float moveSpd = 2f;
    public bool isJumping;
    public float coyTime = .2f;
    private float coyCounter;
    public float jBufferLength = .1f;
    private float jBufferCounter;
    bool isFacingRight = true;
    float dirX;
    public Transform camTarget;
    public float fwdAmount, fwdSpd;

    // Wall jump
    public float wallJumpTime = 0.2f;
    public float wallSlideSpeed = 0.3f;
    public float wallDistance = 0.5f;
    bool isWallSliding = false;
    RaycastHit2D WallCheckHit;
    float jumpTime;

    public LayerMask groundLayer;


    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    private void Update()
    {
        

        // Coyote Time
        if(isJumping == false)
        {
            coyCounter = coyTime;
        }
        else
        {
            coyCounter -= Time.deltaTime;
        }

        // Jump Buffer
        if (Input.GetButtonDown("Jump"))
        {
            jBufferCounter = jBufferLength;
        }
        else
        {
            jBufferCounter -= Time.deltaTime;
        }

        // Jump
        if (jBufferCounter >= 0 && coyCounter > 0f)
        {
            Jump();
            jBufferCounter = 0;
        }
        // Jump height
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

    }

    private void FixedUpdate()
    {
        // Move horizontal
        dirX = Input.GetAxisRaw("Horizontal");
        Walk();

        // Flip player
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            playerSr.flipX = false;
            isFacingRight = true;

        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            playerSr.flipX = true;
            isFacingRight = false;
        }

        // Move camera target
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            camTarget.localPosition = new Vector3(Mathf.Lerp(camTarget.localPosition.x, fwdAmount * Input.GetAxisRaw("Horizontal"), fwdSpd * Time.deltaTime), camTarget.localPosition.y, camTarget.localPosition.z);
        }

        // Wall Jump
        if (isFacingRight)
        {
            WallCheckHit = Physics2D.Raycast(transform.position, new Vector2(wallDistance, 0), wallDistance, groundLayer);
            Debug.DrawRay(transform.position, new Vector2(wallDistance, 0), Color.blue);
        }else
        {
            WallCheckHit = Physics2D.Raycast(transform.position, new Vector2(-wallDistance, 0), wallDistance, groundLayer);
            Debug.DrawRay(transform.position, new Vector2(-wallDistance, 0), Color.blue);

        }

        if (WallCheckHit && isJumping && dirX != 0)
        {
            isWallSliding = true;
            jumpTime = Time.time + wallJumpTime;
        } else if (jumpTime < Time.time)
        {
            isWallSliding = false;
        } 
        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideSpeed, float.MaxValue));
        }
    }
    // isJumping checks
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isJumping = true;
        }
    }
    void Walk()
    {
        rb.velocity = new Vector2(dirX * moveSpd, rb.velocity.y);
    }
    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}
