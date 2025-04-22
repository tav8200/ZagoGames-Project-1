using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterStats characterStats;
    private PickupController pickup;

    private float moveSpeed;
    public float walkSpeed = 1f;
    public float sprintSpeed = 10f;

    public float sprintCost = 25f;
    public float sprintCharge = 10f;

    public Rigidbody2D rb;
    public Animator animator;
    public GameObject noteScreen;

    Vector2 movement;

    //State handler not needed for only move and sprint, but is scalable
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting
    }

    //Check whether player is pressing sprint button or not
    private void StateHandler()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && characterStats.currentStamina > 0 && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            characterStats.currentStamina -= sprintCost * Time.deltaTime; //Drain stamina bar
            characterStats.staminaBar.SetStamina(characterStats.currentStamina);
        }
        else
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            if (characterStats.currentStamina <= characterStats.maxStamina)
            {
                characterStats.currentStamina += sprintCharge * Time.deltaTime; //Recharge stamina bar
            }
            characterStats.staminaBar.SetStamina(characterStats.currentStamina);
        }
    }

    void Update()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (movement.sqrMagnitude > 0.01f) // Check last direction to set correct idle animation
        {
            animator.SetFloat("LastHorizontal", movement.x);
            animator.SetFloat("LastVertical", movement.y);
        }

        StateHandler();

        if (pickup != null && Input.GetKeyDown(KeyCode.E)) //Press E to pickup item
        {
            pickup.Pickup(characterStats);
            pickup = null;
        }

        if (noteScreen.activeSelf && Input.GetKeyDown(KeyCode.E)) //If the note window is active, you can also press e to close it
        {
            noteScreen.SetActive(false);
            GameObject.Find("NotePopup").GetComponent<Animator>().Play("LoreNoteIdle");
            Time.timeScale = 1.0f; //Resume from being paused
        }
    }

    //Fixed update used so movement isnt affected by players' variable framerate
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            pickup = collision.GetComponent<PickupController>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            pickup = null;
        }
    }
}
