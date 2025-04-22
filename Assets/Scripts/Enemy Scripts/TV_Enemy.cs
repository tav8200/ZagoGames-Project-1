using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TV_Enemy : MonoBehaviour
{
    private CharacterStats playerStat;
    private GenerateGrid telePositions;

    public GameObject warning;
    public GameObject worldController;
    public GameObject scaryFace;

    private Image warningImage;

    public float sanityTickTimer = 2f;
    public float teleportTimerMin = 10f;
    public float teleportTimerMax = 40f;
    public float minTeleportDistance = 15f;
    private float effectRadius;

    public int sanityTickDamage = 5;

    private bool inTrigger = false;

    private Coroutine SanityDOT; //Timer which ticks down sanity

    public List<Vector3> teleportPositions;
    private static List<TV_Enemy> activeEnemies = new List<TV_Enemy>(); //List of all tvs you are in the radius of (for calculation of warning opacity)

    public void Awake()
    {
        StartCoroutine(ShowFace()); //Randomly flash face on screen

        GameObject player = GameObject.FindWithTag("Player");
        playerStat = player.GetComponent<CharacterStats>();

        var warningParent = GameObject.Find("Warning_Parent"); //Reference to ui warning when in range of tv
        warning = warningParent.transform.Find("TV_SanityDamage_Warning").gameObject;
        warningImage = warning.GetComponent<Image>();
        warning.SetActive(false);

        worldController = GameObject.Find("World Controller"); //Grab valid teleport positions from world controller
        GenerateGrid telePositions = worldController.GetComponent<GenerateGrid>();
        teleportPositions = telePositions.validPositionsFinal;

        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>(); //Make sure the effect radius is the same as the trigger collider
        effectRadius = circleCollider.radius * transform.lossyScale.x; //Lossyscale is just readonly localscale
        StartCoroutine(TeleportCountdown()); //TV is constantly teleporting
    }

    void Update()
    {
        if (activeEnemies.Count == 0)
        {
            SetWarningOpacity(0);
            warning.SetActive(false);
            return;
        }

        float closestDistance = float.MaxValue;

        foreach (TV_Enemy enemy in activeEnemies)
        {
            float distance = Vector2.Distance(playerStat.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        float opacity = Mathf.Clamp01(1 - (closestDistance / effectRadius)); //Formula for making ui image more opaque the closer to the tv you get
        SetWarningOpacity(opacity);

        warning.SetActive(opacity > 0);
    }

    void SetWarningOpacity(float opacity) //Change opacity of tv warning dynamically
    {
        Color color = warningImage.color;
        color.a = opacity;
        warningImage.color = color;
    }

    public void OnTriggerEnter2D(Collider2D collision) //Take constant damage while in radius
    {
        if (collision.CompareTag("Player"))
        {
            inTrigger = true;

            if (SanityDOT == null)
            {
                SanityDOT = StartCoroutine(TakeSanityDOT());
            }

            if (!activeEnemies.Contains(this)) //Count number of TVs in radius so warning doesn't disable when leaving one radius while still in another
            {
                activeEnemies.Add(this);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inTrigger = false;

            if (SanityDOT != null) //Clear coroutine when player leaves area
            {
                StopCoroutine(SanityDOT);
                SanityDOT = null;
            }

            activeEnemies.Remove(this);
        }
    }

    public void Teleport() //Randomly teleport to a position far enough away from player
    {
        //Debug.Log("beginning teleport countdown");
        Vector2 randPos = teleportPositions[Random.Range(0, teleportPositions.Count)];
        if (Vector2.Distance(playerStat.transform.position, randPos) >= minTeleportDistance)
        {
            transform.position = randPos;
        }
    }

    public IEnumerator TakeSanityDOT() //Take sanity damage while in range, if 0 sanity then take health damage
    {
        while (inTrigger)
        {
            yield return new WaitForSeconds(sanityTickTimer);
            if (playerStat.currentSanity > 0)
            {
                playerStat.TakeSanityDamage(sanityTickDamage); //Sanity damage
            }
            else
            {
                playerStat.TakeDamage(sanityTickDamage); //Health damage
            }
        }

        SanityDOT = null;
    }

    public IEnumerator TeleportCountdown() //Countdown until next teleport
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(teleportTimerMin, teleportTimerMax));
            Teleport();
        }
    }

    public IEnumerator ShowFace() //Randomly flash face on screen
    {
        while (true)
        {
            //Debug.Log("Showing face");
            scaryFace.SetActive(true);
            yield return new WaitForSeconds(Random.Range(0.25f, 1f));
            scaryFace.SetActive(false);
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minTeleportDistance);
    }
}
