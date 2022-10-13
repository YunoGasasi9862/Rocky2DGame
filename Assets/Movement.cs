using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] float CharacterSpeed = 10f;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] LayerMask Ground;
    [SerializeField] GameObject Enemy;
    [SerializeField] GameObject EnemyHitAnimation;


    private Animator anim;
    private float Horizontal;
    private float jumpingSpeed = 5f;
    private BoxCollider2D col;
    private Rigidbody2D rb;
    private bool flip = true;
    private bool Death = false;
    private int AttackCount = 0;
    private float slidingspeed = 5f;
    private float elapsedTime = 0;
    private bool kickoffElapsedTime;

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
        rb.velocity = new Vector3(Horizontal * CharacterSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isOntheGround())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingSpeed);
        }



        if (Death)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        if(kickoffElapsedTime)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log(elapsedTime);
        }

        Sliding();

        checkforFlip();

        CheckForAnimation();

        AttackingMechanism();


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
                sr.flipX = true;
                Vector2 offset = col.offset;
                offset.x += 1;
                col.offset = offset;
                flip = false;
            }
            else if (Horizontal > 0f && isOntheGround() && !flip)
            {
                sr.flipX = false;
                Vector2 offset = col.offset;
                offset.x -= 1;
                col.offset = offset;
                flip = true;
            }
        }

    }

    void AttackingMechanism()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName(""))

        if (!isOntheGround() && Input.GetMouseButtonDown(0))
        {
            anim.SetBool("AttackJ", true);
        }
        else
        {
            anim.SetBool("AttackJ", false);
        }

        if (Input.GetMouseButtonDown(0))
        {
            kickoffElapsedTime = true;

            AttackCount++;
            anim.SetInteger("AttackCount", AttackCount);

            anim.SetBool("Attack", true);
            elapsedTime = 0;  // YAYAY SOLVED IT!!!

        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //fix with new elapsedTime thingy

            if (elapsedTime > 1f)
            {
              
                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }else
            {
                anim.SetFloat("ElapsedTime", elapsedTime);
              
            }
          

        }else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            
            if (elapsedTime > 1f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }
            else
            {
                anim.SetFloat("ElapsedTime", elapsedTime);

            }





        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            if (elapsedTime > 1f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }
            else
            {
                anim.SetFloat("ElapsedTime", elapsedTime);

            }


        }
        else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            if (elapsedTime > 1f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }
            else
            {
                anim.SetFloat("ElapsedTime", elapsedTime);

            }

        }

        if (AttackCount > 4)
        {
            anim.SetBool("Attack", false);
            AttackCount = 0;
            
        }

      

        if (CheckRangeForDestroyEnemy())
        {
            GameObject HitAnim = Instantiate(EnemyHitAnimation, Enemy.transform.position, Quaternion.identity);
            Destroy(Enemy.gameObject);
            Destroy(HitAnim, 3f);
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
        }

        bool CheckRangeForDestroyEnemy()
        {
            if (Enemy != null)
            {
                if (Vector2.Distance(transform.position, Enemy.transform.position) <= 1f)
                {
                    return true;
                }

            }

            return false;
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
        }
    
}
