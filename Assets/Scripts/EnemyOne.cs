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

    public LayerMask attackMask;
    public LayerMask obstacleMask;

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
            animator.SetBool("Attack", true);
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

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, lookRadius, obstacleMask);

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
        Vector3 pos = playerStat.transform.position;

        float distanceToPlayer = Vector3.Distance(transform.position, playerStat.transform.position);
        Debug.Log(distanceToPlayer);
        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackRange, attackMask);
        if (colInfo != null)
        {
            playerStat.TakeDamage(damage);
            Debug.Log("Attack Landed");
        }
    }

    private void OnColliderStay2D(Collision2D collision) //Deal damage to player
    {
        if (collision.gameObject.tag == "Player")
        {
            playerStat.TakeDamage(damage);
            Debug.Log("Bumped into player");
        }
    }

    private void OnDrawGizmosSelected() //Show size of enemy vision in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, stopRadius);
    }
}
