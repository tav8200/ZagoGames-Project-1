using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOne : MonoBehaviour
{
    public int damage;

    public float lookRadius = 10f; //How far enemy can see
    public float stopRadius = 2f; //How close enemy has to be to player before it stops moving
    public float crawlSpeed = 10f;
    public float attackRange = 1f;
    public Transform attackPoint;
    public LayerMask attackMask;

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
        }
        else if (distance <= stopRadius)
        {
            canMove = false;
            //animator.SetBool("isMoving", false);
            animator.SetTrigger("Attack");
        }
        else
        {
            canMove = false;
            animator.SetBool("isMoving", false);
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = (target.position - transform.position).normalized;

        if (canMove) //Move towards player only when inside of look radius
        {
            rb.MovePosition(rb.position + direction * crawlSpeed * Time.deltaTime);
        }

        if (direction.x <= 0.01) //Flip sprite depending on direction
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void Attack()
    {
        Vector3 pos = attackPoint.position;

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            playerStat.TakeDamage(damage);
            Debug.Log("hit");
        }
    }

    private void OnTriggerStay2D(Collider2D collision) //Deal damage to player
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

        Vector3 pos = attackPoint.position;
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
