using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFlicker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Button thisButton;
    public Image buttonImage;
    public TextMeshProUGUI buttonText;

    private Coroutine flickerCoroutine;

    [SerializeField] private AudioClip buttonHover;

    public void Start()
    {
        if (thisButton.GetComponent<Button>().interactable == false) //If note has not been found, the button will look different
        {
            buttonText.text = "?????";
            buttonImage.color = Color.gray;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (flickerCoroutine != null) //If coroutine is already running when hovering, stop it and restart it
        {
            StopCoroutine(flickerCoroutine);
        }

        if (thisButton.GetComponent<Button>().interactable == true) //If you can't click the button, it should not flicker
        {
            flickerCoroutine = StartCoroutine(FlickerEffect());
        }

        if (thisButton.interactable == true)
        {
            SoundFXManager.Instance.PlaySoundFXClip(buttonHover, transform, 1f);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (flickerCoroutine != null) //Stop and clear coroutine when exiting hover to prevent it from overlapping
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }
        
        if (thisButton.GetComponent<Button>().interactable == true) //Reset proper colors after pointer exit
        {
            buttonImage.color = Color.black;
            buttonText.color = Color.white;
        }
        else
        {
            buttonImage.color = Color.gray;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (thisButton.GetComponent<Button>().interactable == true) //Reset colors after clicking, but only if button is interactable
        {
            buttonImage.color = Color.black;
            buttonText.color = Color.white;
        }
    }

    public void PlaySpecialSound(AudioClip sound) //Function for playing a specific sound when a button is clicked
    {
        SoundFXManager.Instance.PlaySoundFXClip(sound, transform, 1f);
    }

    private IEnumerator FlickerEffect() //Flicker between two colors
    {
        while (true)
        {
            buttonImage.color = Color.white;
            buttonText.color = Color.black;
            yield return new WaitForSecondsRealtime(0.5f);

            buttonImage.color = Color.black;
            buttonText.color = Color.white;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}
