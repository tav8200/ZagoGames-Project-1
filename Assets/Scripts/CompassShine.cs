using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CompassShine : MonoBehaviour
{
    public int maxLoops = 5; //Number of times shine will pulse before disappearing

    public float pulseSpeed = 0.75f;
    public float maxIntensity = 0.75f;
    public float minIntensity = 0f;
    private float pulseTime = 0f;

    private SpriteRenderer spriteRenderer;
    private Light2D spotLight;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spotLight = GetComponentInChildren<Light2D>();

        StartCoroutine(PulseEffect());
    }

    IEnumerator PulseEffect()
    {
        int completedLoops = 0;

        while (completedLoops < maxLoops)
        {
            pulseTime = 0f;

            while (pulseTime < 1f)
            {
                pulseTime += Time.deltaTime * pulseSpeed;
                float pulseValue = (Mathf.Sin(pulseTime * Mathf.PI * 2) + 1f) / 2f;

                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(0f, 1f, pulseValue);
                spriteRenderer.color = color;

                spotLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulseValue); //Spotlight pulse

                yield return null;
            }
            Debug.Log(completedLoops);
            completedLoops++;
        }

        Destroy(gameObject);
    }
}
