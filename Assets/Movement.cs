using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] float CharacterSpeed = 10f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] LayerMask Ground;
    [SerializeField] GameObject EnemyHitAnimation;
    [SerializeField] AttackEnemy Enemy;
    [SerializeField] LayerMask Ledge;
    [SerializeField] GameObject Ceiling;
    [SerializeField] BoxCollider2D CeilingCollider;
    [SerializeField] LedgeDetector _ledge;   

    private Animator anim;
    private float Horizontal;
    private float jumpingSpeed = 5f;
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private bool flip = true;
    private bool Death = false;
    private float ledgeTiming = 0f;
    private float slidingspeed = 5f;
    [SerializeField] bool once=true;
 


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        CeilingCollider = Ceiling.GetComponent<BoxCollider2D>();
     
    }
    void Update()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector3(Horizontal * CharacterSpeed, rb.velocity.y);



        if (Input.GetButtonDown("Jump") && isOntheGround())
        {
           
            rb.velocity = new Vector2(rb.velocity.x, jumpingSpeed);
        }

        if(isOntheGround() && _ledge.Allowed)
        {
            CeilingCollider.enabled = true;
            Ceiling.gameObject.SetActive(true);

        }
        if(!isOntheGround() && !_ledge.Allowed)
        {
            CeilingCollider.enabled = false;
            Ceiling.gameObject.SetActive(false);
            _ledge.Allowed = true;
        }

        if (Death)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

    
        if(!once)
        {
            once = true;
            rb.AddForce(transform.up * 35f * Time.deltaTime, ForceMode2D.Impulse);
            rb.AddForce(-transform.right * 30f * Time.deltaTime, ForceMode2D.Impulse);
        }

       
        Sliding();

        checkforFlip();

        CheckForAnimation();

        RayCastGenerator();



    }
   
    void Sliding()
    {


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetBool("Sliding", true);
            rb.velocity = new Vector2(slidingspeed, rb.velocity.y);
         }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            anim.SetBool("Sliding", false);

        }
    }
    bool isOntheGround()
    {
        return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, Ground);
    }


    void checkforFlip()
    {
        if (!Death)
        {
            if (Horizontal < 0f && isOntheGround() && flip)
            {
                //character flip
                sr.flipX = true;
                Enemy.HeroineFlipped = true;
                Vector2 offset = col.offset;
                offset.x += 1;
                col.offset = offset;


                //ceiling
                Vector2 ceilingoffset = CeilingCollider.offset;
                ceilingoffset.x += 1.36f;
                CeilingCollider.offset = ceilingoffset;
                flip = false;
            }
            else if (Horizontal > 0f && isOntheGround() && !flip)
            {
                sr.flipX = false;
                Enemy.HeroineFlipped = false;

                Vector2 offset = col.offset;
           
                offset.x -= 1;
              
                col.offset = offset;


                Vector2 ceilingoffset = CeilingCollider.offset;
                ceilingoffset.x -= 1.36f;
                CeilingCollider.offset = ceilingoffset;
                flip = true;
            }
        }

    }

   
        void CheckForAnimation()
        {
            if (Horizontal > 0f || Horizontal < 0f)
            {
                anim.SetInteger("State", 1);
            } else
            {
                anim.SetInteger("State", 0);

            }

            if (rb.velocity.y >= .1f)
            {
                anim.SetInteger("State", 2);

            } else if (rb.velocity.y <= -.1f)
            {
                anim.SetInteger("State", 3);
            }

            if(anim.GetCurrentAnimatorStateInfo(0).IsName("LedgeGrab"))
              {
                 ledgeTiming += Time.deltaTime;

                   if (ledgeTiming>.5f)
                  {
                         anim.SetBool("LedgeGrab", false);
                         ledgeTiming = 0;

                  }
              }
      
        }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {

            GameObject HitAnim = Instantiate(EnemyHitAnimation, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            Destroy(HitAnim, 3f);
        }
    }



    void Restart()
        {

            rb.bodyType = RigidbodyType2D.Static;

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                Death = true;
                anim.SetBool("Death", true);
            }

            if(collision.collider.CompareTag("Ledge"))
             {
                 transform.parent = collision.transform;
                    rb.bodyType = RigidbodyType2D.Static;
             }
        }


     void RayCastGenerator()
    {
        if (sr.flipX)
        {
             if(Physics2D.Raycast(transform.position, -transform.right, .5f, Ledge) && once)
            {
                anim.SetBool("LedgeGrab", true);  
                once = false;
            }
              Debug.DrawRay(transform.position, -transform.right * .5f, Color.red);

           

  
        }
        else
        {
           if (Physics2D.Raycast(transform.position, transform.right, .5f, Ledge) && once)
            {
                anim.SetBool("LedgeGrab", true);
                once = false;

            }
            Debug.DrawRay(transform.position, transform.right * .5f, Color.red);


        }
        //5 is how long the raycast should be

       
        
    }

        

    }
