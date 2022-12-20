using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyJumping : MonoBehaviour
{

    //ENEMY JUMPING FROM ONE PLATFORM TO ANOTHER
    private Rigidbody2D rb;
    private Animator anim;
    private RaycastHit2D hit;
    private bool JUMP = false;
    private float count = 0;
    [SerializeField] LayerMask Jumping;
    [SerializeField] LayerMask Ledge;
    [SerializeField] LayerMask ground;
    private BoxCollider2D box;
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        hit = Physics2D.Raycast(transform.position, transform.right, .5f, Jumping);
        Debug.DrawRay(transform.position, transform.right * .5f, Color.red);
        if(hit.collider!=null && hit.collider.isTrigger)
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("CanWalk", false);
             JUMP = true;
            hit.collider.enabled = false;
           
        }

        if (JUMP && count <= 1f)
        {
            rb.AddForce(new Vector2(3f, 30f) * Time.deltaTime, ForceMode2D.Impulse);
           
            count += Time.deltaTime;


        }

        if(count>=1f)
        {
            count = 0f;
            JUMP = false;
            
           
         
        }

      
        if(isOntheGround())
        {
            Destroy(gameObject);

        }


    }

    private void FixedUpdate()
    {
        if(!JUMP && isOntheLedge())
        {
            rb.velocity = new Vector2(20 * Time.deltaTime, 0);
            if (rb.velocity.magnitude > .1f)
            {
                anim.SetBool("CanWalk", true);
            }
        }

        

      


    }

    public bool isOntheLedge()
    {
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, 1f, Ledge);
    }

    bool isOntheGround()
    {
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0f, Vector2.down, .1f, ground);
    }


}
