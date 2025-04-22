using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Light_Enemy : MonoBehaviour
{
    private AIPath AIPath;
    private CharacterStats CharacterStats;

    private Transform target;

    public Animator animator;
    public LayerMask attackMask;

    public float lookRadius = 10f;
    public float attackRange = 2f;
    public float attackSize = 2f;
    public int lightDamage = 2;

    void Awake()
    {
        AIPath = GetComponent<AIPath>();
        GameObject player = GameObject.FindWithTag("Player");

        CharacterStats = player.GetComponent<CharacterStats>();
        target = player.transform;

        AIPath.enabled = false;
    }

    private void Update()
    {
        float distance = Vector2.Distance(target.position, transform.position);
        Vector2 direction = (target.position - transform.position).normalized;

        if (distance <= lookRadius) //Follow player if in radius
        {
            AIPath.enabled = true;
            //animator.SetBool("isMoving", true);
        }
        else
        {
            AIPath.enabled = false;
        }

        if (distance <= attackRange) 
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
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

    void Attack()
    {
        Vector2 playerPos = CharacterStats.transform.position; //Get position of enemy and player
        Vector2 enemyPos = transform.position;

        Vector2 direction = (playerPos - enemyPos).normalized; //Direction vector between player and enemy
        Vector2 attackPos = enemyPos + direction * 2f; //Final location of attack hitbox

        Collider2D colInfo = Physics2D.OverlapCircle(attackPos, attackSize, attackMask);
        if (colInfo != null)
        {
            if (CharacterStats.currentLightLevel == 1) //If light level is as low as it can be, take sanity damage instead
            {
                CharacterStats.TakeSanityDamage(10);
            }
            else
            {
                CharacterStats.TakeLightDamage(lightDamage);
            }
        }
    }

    private void OnDrawGizmosSelected() //Show size of enemy vision in editor
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
