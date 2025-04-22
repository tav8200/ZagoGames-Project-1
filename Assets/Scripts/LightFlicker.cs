using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Flicker Time Range")]
    [SerializeField] private float minTimer = 5f;
    [SerializeField] private float maxTimer = 20f;

    private void Awake()
    {
        StartCoroutine(LightFlickerTimer());
    }

    private IEnumerator LightFlickerTimer()
    {
        while (true)
        {
            float timer = Random.Range(minTimer, maxTimer);

            yield return new WaitForSeconds(timer);

            animator.SetTrigger("Flicker");
        }
    }
}
