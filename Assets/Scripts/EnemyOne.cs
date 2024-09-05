using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOne : MonoBehaviour
{
    public int damage;

    public float lookRadius = 10f; //How far enemy can see
    public float stopRadius = 2f; //How close enemy has to be to player before it stops moving
    public float crawlSpeed = 10f;

    public Animator animator;

    private bool canMove = false;

    private CharacterStats playerStat;

    private Transform target;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindWithTag("Player"); //Find player when spawned in

        if (player != null)
        {
            target = player.transform;
            playerStat = player.GetComponent<CharacterStats>();
        }
    }

    private void Update()
    {
        float distance = Vector2.Distance(target.position, transform.position);

        if (distance <= lookRadius && distance >= stopRadius)
        {
            canMove = true;
            animator.SetBool("isMoving", true);
            animator.SetBool("isAttacking", false);
        } 
        else if (distance <= stopRadius)
        {
            canMove = false;
            animator.SetBool("isAttacking", true);
            animator.SetBool("isMoving", false);
        }
        else
        {
            canMove = false;
            animator.SetBool("isMoving", false);
            animator.SetBool("isAttacking", false);
        }
    }

    private void FixedUpdate()
    {
        if (canMove) //Move towards player only when inside of look radius
        {
            Vector2 direction = (target.position - transform.position).normalized; 
            rb.MovePosition(rb.position + direction * crawlSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) //Deal damage to player
    {
        if (collision.gameObject.tag == "Player")
        {
            playerStat.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected() //Show size of enemy vision in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, stopRadius);
    }
}
