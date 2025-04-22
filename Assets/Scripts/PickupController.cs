using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PickupController : MonoBehaviour
{
    public ItemType itemType;

    public GameObject noteScreen;
    public GameObject compassFinder;

    public NoteManager noteHolder;
    private NoteButtonManager noteButtonManager;

    public Animator animator;

    public LayerMask enemyLayer;

    public TextMeshProUGUI pickupPrompt; //Prompt that appears when near an item
    public TextMeshProUGUI noteText; //Text displaying what is on a note item
    public Material defaultMaterial;
    public Material highlightMaterial;

    private bool isPlayerNearby = false;
    private bool isOutlined = false;
    private bool noteIsOpen = false;
    private bool isPaused = false; //For pausing the game when picking up a note

    public int healAmount = -30;
    public int sanityRestoreAmount = -30;
    public int lightRestoreAmount = 1;

    public float distractRadius = 5f;
    public float targetChangeDuration = 5f;

    [Header("SoundFX")]
    [SerializeField] private AudioClip medPickup;
    [SerializeField] private AudioClip pillPickup;
    [SerializeField] private AudioClip compassPickup;
    [SerializeField] private AudioClip notePickup;
    [SerializeField] private AudioClip boxesPickup;
    [SerializeField] private AudioClip batteryPickup;

    public enum ItemType
    {
        Heal,
        Sanity,
        Light,
        Note,
        Distraction,
        Compass,
    }

    public void Awake()
    {
        pickupPrompt = GameObject.FindWithTag("Pickup Text").GetComponent<TextMeshProUGUI>(); //Press E to pickup prompt

        var parent = GameObject.Find("NoteScreenParent");
        noteScreen = parent.transform.Find("NoteScreen").gameObject;
        noteText = noteScreen.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(t => t.gameObject.name == "LoreText"); //Screen which will popup when a lore note is found

        noteHolder = GameObject.Find("Note Holder").GetComponent<NoteManager>(); //Object holding list of possible lore notes

        gameObject.GetComponent<Renderer>().material = defaultMaterial; //Non highlighted material for items

        noteScreen.SetActive(false);

        if (noteHolder.loreList.Count == noteHolder.foundNotes.Count && itemType == ItemType.Note) //Remove note if you have discovered all notes
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) //Highlight item and show pickup prompt
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            isPlayerNearby = true;
            foreach (Renderer renderer in renderers)
            {
                renderer.material = highlightMaterial;
            }
            if (gameObject.name != "Breakable Boxes") //Make sure to name all distraction objects the same
            {
                pickupPrompt.gameObject.SetActive(true);
                pickupPrompt.text = "Press E to pick up.";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //Unhighlight item and remove pickup prompt
    {
        if (collision.CompareTag("Player"))
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.material = defaultMaterial;
            }
            isPlayerNearby = false;
            pickupPrompt.gameObject.SetActive(false);
        }
    }

    public void Pickup(CharacterStats player)
    {
        switch (itemType)
        {
            case ItemType.Heal:
                if (player.currentHealth < player.maxHealth)
                {
                    player.TakeDamage(healAmount);
                    SoundFXManager.Instance.PlaySoundFXClip(medPickup, transform, 1f);
                    Destroy(gameObject);
                }
                break;
            case ItemType.Sanity:
                if (player.currentSanity < player.maxSanity)
                {
                    player.TakeSanityDamage(sanityRestoreAmount);
                    SoundFXManager.Instance.PlaySoundFXClip(pillPickup, transform, 1f);
                    Destroy(gameObject);
                }
                break;
            case ItemType.Light:
                if (player.currentLightLevel < player.maxLightLevel)
                {
                    player.TakeLightDamage(lightRestoreAmount);
                    SoundFXManager.Instance.PlaySoundFXClip(batteryPickup, transform, 1f);
                    Destroy(gameObject);
                }
                break;
            case ItemType.Note:
                StartCoroutine(NoteDisplayDelay());
                SoundFXManager.Instance.PlaySoundFXClip(notePickup, transform, 1f);
                break;
            case ItemType.Distraction:
                Distract();
                SoundFXManager.Instance.PlaySoundFXClip(boxesPickup, transform, 1f);
                animator.SetFloat("Speed", 1f); //Play topple animation
                break;
            case ItemType.Compass:
                CompassShine();
                SoundFXManager.Instance.PlaySoundFXClip(compassPickup, transform, 1f);
                Destroy(gameObject);
                break;
        }
    }

    public void NoteDisplay() //Also pause game in background
    {
        noteButtonManager = GameObject.Find("Note Button Manager").GetComponent<NoteButtonManager>(); //Find note button manager so you can update notes after picking up a new one
        noteText.text = noteHolder.FindRandomNote(); //Pull list from global note holder object
        noteScreen.SetActive(true);
        noteIsOpen = true;
        noteButtonManager.UpdateButtons();

        Time.timeScale = 0f; //Pause game while reading note
    }

    public void Distract() //Pull enemies to this item for a time
    {
        Collider2D[] colInfo = Physics2D.OverlapCircleAll(transform.position, distractRadius, enemyLayer); //Put all detected enemies in a list
        foreach (Collider2D col in colInfo)
        {
            AIPath aiPath = col.GetComponent<AIPath>();
            StartCoroutine(TempTargetChange(aiPath));
        }

        if (colInfo.Count() == 0)
        {
            StartCoroutine(DestroyObject());
        }
    }

    public void CompassShine() //Show the way to the ladder briefly
    {
        GameObject hatch = GameObject.Find("Ladder_Down(Clone)"); //Find position of ladder
        Vector3 hatchPos = hatch.transform.position;

        GameObject player = GameObject.FindWithTag("Player");
        Vector3 playerPos = player.transform.position;

        Vector3 direction = (hatchPos - playerPos).normalized;
        Vector3 midpoint = (playerPos + hatchPos) / 2f; //Find midpoint between player and ladder

        float distanceToMidpoint = Vector3.Distance(playerPos, midpoint);

        if (distanceToMidpoint > 5f) //Restrict ping to ladder from showing up too far away
        {
            midpoint = playerPos + direction * 5f;
        }

        Instantiate(compassFinder, midpoint, Quaternion.identity);
    }

    IEnumerator TempTargetChange(AIPath path) //Change target of enemy to this object
    {
        path.target = gameObject.transform;
        yield return new WaitForSeconds(targetChangeDuration);
        path.target = GameObject.Find("Player").transform;
        Destroy(gameObject);
    }

    IEnumerator DestroyObject() //Still need to destroy object even if it doesn't detect anything
    {
        yield return new WaitForSeconds(targetChangeDuration);
        Destroy(gameObject);
    }

    IEnumerator NoteDisplayDelay()
    {
        GameObject.Find("NotePopup").GetComponent<Animator>().Play("LoreNotePopup");
        yield return new WaitForSeconds(0.75f);
        NoteDisplay();
        Destroy(gameObject);
    }
}
