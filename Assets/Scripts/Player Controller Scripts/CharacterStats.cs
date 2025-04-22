using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth {  get; private set; } //Any script can GET this value but only this script can SET
    public float damageImmuneTime = 2f; //How long the player is immune to damage after getting hit
    private bool canTakeDamage = true;

    public float maxStamina = 100f;
    public float currentStamina;

    public int maxSanity = 100;
    public int currentSanity {  get; private set; }

    public float maxLightLevel = 10f;
    public float currentLightLevel {  get; private set; }
    public float changeDuration = 0.5f;
    private float targetLightLevel;
    private Coroutine changeLightCoroutine;

    public int defaultWorldWidth = 5; //Increment worldWidth / worldHeight, reset to this value after Die()
    public int defaultWorldHeight = 5;

    public HealthBarControl healthBar;
    public HealthBarControl staminaBar;
    public HealthBarControl sanityBar;

    public Light2D lightSource;

    private void Awake()
    {
        currentSanity = PlayerPrefs.HasKey("sanityValue") ? PlayerPrefs.GetInt("sanityValue") : maxSanity; //Remember sanity value when going to new floor

        currentHealth = PlayerPrefs.HasKey("healthValue") ? PlayerPrefs.GetInt("healthValue") : maxHealth;

        currentLightLevel = PlayerPrefs.HasKey("lightValue") ? PlayerPrefs.GetFloat("lightValue") : maxLightLevel / 2f;

        lightSource.pointLightOuterRadius = currentLightLevel;

        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);

        sanityBar.SetMaxSanity(maxSanity);
        sanityBar.SetSanity(currentSanity); 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(50);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TakeSanityDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            TakeLightDamage(2);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            TakeLightDamage(1);
        }
    }

    public void TakeDamage (int damage)
    {
        if (damage < 0) //Do not give damage immunity if player heals from a pickup
        {
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
        }
        else if (canTakeDamage) //Only give damage immunity if player loses health
        {
            canTakeDamage = false;
            currentHealth -= damage;
            healthBar.SetHealth(currentHealth);
            StartCoroutine(HealthDamageImmunity(damageImmuneTime));
        }

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        else if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log(currentHealth + " HP");

        if (currentHealth <= 0)
        {
            Die();
        }

        PlayerPrefs.SetInt("healthValue", currentHealth);
        PlayerPrefs.Save();
    }

    public void TakeSanityDamage (int sanDamage)
    {
        currentSanity -= sanDamage;

        if (currentSanity < 0)
        {
            currentSanity = 0;
        }
        else if (currentSanity > maxSanity)
        {
            currentSanity = maxSanity;
        }

        sanityBar.SetSanity(currentSanity);

        PlayerPrefs.SetInt("sanityValue", currentSanity);
        PlayerPrefs.Save();

        Debug.Log(currentSanity + " Sanity");
    }

    public void TakeLightDamage (int ligDamage)
    {
        targetLightLevel = Mathf.Clamp(currentLightLevel - ligDamage, 1, maxLightLevel); //Restrict radius size between 1 and maximum light level

        if (changeLightCoroutine != null) //Ensure transitions don't overlap
        {
            StopCoroutine(changeLightCoroutine); //Light radius values become decimals if the previous call doesn't finish
        }
        changeLightCoroutine = StartCoroutine(SmoothChangeLightRadius(targetLightLevel));
    }

    public virtual void Die() //Make sure to set floorNum back to 1 here also
    {
        currentHealth = maxHealth;
        PlayerPrefs.SetInt("healthValue", currentHealth);

        currentSanity = maxSanity;
        PlayerPrefs.SetInt("sanityValue", currentSanity);

        currentLightLevel = maxLightLevel / 2f;
        PlayerPrefs.SetFloat("lightValue", currentLightLevel);

        PlayerPrefs.SetInt("gridWidth", defaultWorldWidth);
        PlayerPrefs.SetInt("gridHeight", defaultWorldHeight);

        PlayerPrefs.SetInt("floorNumber", 1);

        PlayerPrefs.Save();
    }

    private IEnumerator SmoothChangeLightRadius(float targetLightLevel) //Smoothly transition between light levels
    {
        float initialLightLevel = currentLightLevel;
        float elapsedTime = 0f;

        while (elapsedTime < changeDuration)
        {
            currentLightLevel = Mathf.Lerp(initialLightLevel, targetLightLevel, elapsedTime / changeDuration);
            lightSource.pointLightOuterRadius = currentLightLevel;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentLightLevel = targetLightLevel;
        lightSource.pointLightOuterRadius = targetLightLevel;

        PlayerPrefs.SetFloat("lightValue", currentLightLevel);
        PlayerPrefs.Save();
    }

    private IEnumerator HealthDamageImmunity(float time)
    {
        yield return new WaitForSeconds(time);
        canTakeDamage = true;
    }
}
