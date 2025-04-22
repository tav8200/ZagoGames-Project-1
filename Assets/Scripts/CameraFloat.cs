using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFloat : MonoBehaviour
{
    public float speed = 1f;
    public float returnAngleMax = 5f;
    public float returnAngleMin = -5f;

    public Transform returnPoint;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    void FixedUpdate()
    {
        rb.velocity = rb.velocity.normalized * speed; //Make sure camera speed is always 1
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionPoint = collision.contacts[0].point;
        Vector2 directionToReturn = (returnPoint.position - (Vector3)collisionPoint).normalized;

        directionToReturn.x += Random.Range(returnAngleMin, returnAngleMax);
        directionToReturn.y += Random.Range(returnAngleMin, returnAngleMax);

        rb.velocity = directionToReturn * speed;
    }
}
