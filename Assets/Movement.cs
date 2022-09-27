using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float CharacterSpeed=10f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] LayerMask Ground;


    private Animator anim;
    private float Horizontal;
    private float jumpingSpeed = 5f;
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private bool flip = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");

        rb.velocity = new Vector2(Horizontal * CharacterSpeed, rb.velocity.y);

        if(Input.GetButtonDown("Jump") && isOntheGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingSpeed);
        }

     

         if(Input.GetMouseButtonDown(0))
        {
            anim.SetBool("Attack", true);
        }
        if(Input.GetMouseButtonUp(0))
        {
            anim.SetBool("Attack", false);

        }

        checkforFlip();

        CheckForAnimation();
    }

    bool isOntheGround()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .3f, Ground);
    }


    void checkforFlip()
    {
        if(Horizontal<0f && isOntheGround() && flip)
        {
            sr.flipX = true;
            Vector2 offset = col.offset;
            offset.x += +1;
            col.offset = offset;
            flip = false;
        }
        else if(Horizontal >0f && isOntheGround() && !flip)
        {
            sr.flipX = false;
            Vector2 offset = col.offset;
            offset.x += -1;
            col.offset = offset;
            flip = true;
        }
    }

    void CheckForAnimation()
    {
        if(Horizontal >0f || Horizontal <0f)
        {
            anim.SetInteger("State", 1);
        }else
        {
            anim.SetInteger("State", 0);

        }

        if(rb.velocity.y >=.1f)
        {
            anim.SetInteger("State", 2);

        }else if(rb.velocity.y <=-.1f)
        {
            anim.SetInteger("State", 3);
        }
    }
}
