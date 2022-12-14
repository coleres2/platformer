using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float jumpSpeed = 0.0f;
    public float previous_x_speed = 0.0f;
    public bool canJump = true;
    private float direction = 0;
    public bool stuck = false;
    public int counter = 0;

    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    
    public LayerMask groundLayer;
    public LayerMask iceGroundLayer;
    public LayerMask sandGroundLayer;
    public LayerMask snowLayer;
    public bool isTouchingGround;
    public bool isTouchingIceGround;
    public bool isTouchingSandGround;
    public bool isTouchingSnow;
    public bool isTouchingBounce;

    public PhysicsMaterial2D bounceMaterial, normalMaterial, iceMaterial, sandMaterial, SnowMaterial;

    private Rigidbody2D player;

    public bool isMoving;

    private Animator anim;
    private bool grounded;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        PlayerPrefs.DeleteAll();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            Debug.Log("You Won!");
        }
    }


    // Update is called once per frame
    void Update()
    {
        direction = Input.GetAxis("Horizontal");
        player.velocity = new Vector2(player.velocity.x, player.velocity.y);
        isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingIceGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, iceGroundLayer);
        isTouchingSandGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, sandGroundLayer);
        isTouchingSnow = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, snowLayer);
        // isTouchingBounce= Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, bounceMaterial);
        if(isTouchingIceGround||isTouchingSandGround||isTouchingSnow)
        {
            isTouchingGround = true;
        }
        if (player.velocity.magnitude == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        //Makes the player face the correct direction and move left or right when jumping
        if (direction > 0f)
        {
            if (!isTouchingGround && !isTouchingIceGround && !isTouchingSnow)
            {
                if (jumpSpeed == 0.0f && (isTouchingGround || isTouchingIceGround || isTouchingSnow))
                    player.velocity = new Vector2(direction * walkSpeed, player.velocity.y);

            }
            else
                transform.localScale = new Vector2(.45f, .45f);

        }
        else if (direction < 0f)
        {
            if (!isTouchingGround && !isTouchingIceGround)
            {
                if (jumpSpeed == 0.0f && (isTouchingGround || isTouchingIceGround || isTouchingSnow))
                    player.velocity = new Vector2(direction * walkSpeed, player.velocity.y);
            }
            else
                transform.localScale = new Vector2(-.45f, .45f);

        }
        if(!isMoving) {
            counter++;
            if(counter > 120) {
                stuck = true;
            }
        }

        //Bounce
        if (jumpSpeed > 0  || !isTouchingGround)
        {
            player.sharedMaterial = bounceMaterial;
        }
        else
        {
            player.sharedMaterial = normalMaterial;
        }

        if (isTouchingIceGround)
        {
            player.sharedMaterial = iceMaterial;
        }

        if (isTouchingSnow)
        {
            player.sharedMaterial = SnowMaterial;
        }

        //Jump
        if (!isMoving || isMoving)
        {
            if ((Input.GetKey("space")) && (!isMoving || isTouchingGround))
            {
                
                
                if(isTouchingSnow) {
                    jumpSpeed += 0.2f;
                    if(jumpSpeed > 3f) {
                        jumpSpeed = 4f;
                    }
                }
                else if(jumpSpeed < 8f) {
                    jumpSpeed += 0.2f;
                }
            }
            
            // if (Input.GetKeyDown("space") && (isTouchingGround) && canJump)
            // {
            //     player.velocity = new Vector2(previous_x_speed, player.velocity.y);
            // }
            // if (jumpSpeed >= 10f && (isTouchingGround || isTouchingIceGround))
            // {
            //     float tempx = direction * walkSpeed;
            //     float tempy = jumpSpeed;
            //     player.velocity = new Vector2(tempx, tempy);
            //     Invoke("ResetJump", 0.2f);
            // }
            if (Input.GetKeyUp("space"))
            {
                
                if (isTouchingGround || !isMoving)
                {
                    if (jumpSpeed < .5f)
                    {
                        jumpSpeed = .5f;
                    }
                    previous_x_speed = player.velocity.x + direction*1.5f * walkSpeed;
                    Vector2 temp = new Vector2(0, .1f);
                    player.MovePosition(player.position + temp);
                    player.velocity = new Vector2(previous_x_speed, jumpSpeed);
                    jumpSpeed = 0f;
                }
                // if (isTouchingSnow)
                // {
                //     if (jumpSpeed < .5f)
                //     {
                //         jumpSpeed = .5f;
                //     }
                //     player.velocity = new Vector2(direction * walkSpeed, jumpSpeed);
                //     jumpSpeed = 0f;
                // }
                stuck = false;
                counter = 0;
                canJump = true;
            }
        }

        if (isTouchingGround)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
        anim.SetBool("grounded", grounded);
        

}
    void ResetJump()
    {
        canJump = false;
        jumpSpeed = 0;
    }
}