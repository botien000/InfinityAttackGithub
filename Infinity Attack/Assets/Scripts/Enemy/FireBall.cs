using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 velocity;
    private Animator animator;
    private bool Explosion = false;
    private int atkDmg;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        

    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            Explosion = true;
            GameObject ball = GameObject.FindGameObjectWithTag("FireBall");
            Destroy(ball, 1f);
        }
        
        animator.SetBool("Explosion", Explosion);
        rb.velocity = velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            Explosion = true;
            Destroy(gameObject, 1f);
            velocity = Vector2.zero;

            if (CharacterObject.instance.isAttacked)
            {
                return;
            }
            else
            {
                InGameCharLoading.instance.Damage(atkDmg);
            }
        }
        else if(collision.gameObject.tag == "Ground")
        {
            Explosion = true;
            Destroy(gameObject, 1f);
            velocity = Vector2.zero;
        }
        else if(collision.gameObject.tag == "Enemy")
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            Explosion = true;
            Destroy(gameObject, 1f);
            velocity = Vector2.zero;
        }
        else if (collision.gameObject.tag == "Boss")
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            Explosion = true;
            Destroy(gameObject, 1f);
            velocity = Vector2.zero;
        }

    }

    public void setVelocity(Vector2 vector2)
    {
        velocity = vector2;
    }

    public void setAttackDamage(int attackDamage)
    {
        atkDmg = attackDamage;
    }

}
