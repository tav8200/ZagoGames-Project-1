using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler_Enemy : MonoBehaviour
{
    private AIPath AIPath;
    private CharacterStats playerStat;

    public Animator animator;
    private Transform target;

    public LayerMask attackMask;

    public float wanderSpeed = 2f;
    public float attackRange = 2f;
    public float attackSize = 2f;
    public float lookRadius = 5f;
    public int damage = 10;

    private Vector2 moveDirection;

    private void Awake()
    {
        AIPath = GetComponent<AIPath>();
        GameObject player = GameObject.FindWithTag("Player");

        playerStat = player.GetComponent<CharacterStats>();
        target = player.transform;

        AIPath.enabled = false;
    }

    private void FixedUpdate()
    {
        float distance = Vector2.Distance(target.position, transform.position);
        Vector2 direction = (target.position - transform.position).normalized;

        if (distance <= lookRadius) //Follow player if in radius
        {
            AIPath.enabled = true;
            animator.SetBool("isMoving", true);
        }
        else
        {
            AIPath.enabled = false;
            animator.SetBool("isMoving", false);
            //Wander();
        }

        if (Vector2.Distance(target.position, transform.position) <= attackRange)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Attack", false);
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
        Vector3 pos = playerStat.transform.position; //Change this to make attack feel more responsive??

        float distanceToPlayer = Vector3.Distance(transform.position, playerStat.transform.position);

        Collider2D colInfo = Physics2D.OverlapCircle(pos, attackSize, attackMask);
        if (colInfo != null)
        {
            playerStat.TakeDamage(damage);
            Debug.Log("Attack Landed");
        }
    }

    public void Wander() //This isnt working for some reason
    {
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        Vector3 newPosition = transform.position + (Vector3)(moveDirection * wanderSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected() //Show size of enemy vision in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
