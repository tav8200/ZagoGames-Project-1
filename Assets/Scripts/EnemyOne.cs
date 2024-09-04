using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyOne : MonoBehaviour
{
    public int damage;
    public float lookRadius = 10f;
    public float crawlSpeed = 5f;

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

        //Move towards player
        if (distance <= lookRadius)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * crawlSpeed;
        } 
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerStat.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
