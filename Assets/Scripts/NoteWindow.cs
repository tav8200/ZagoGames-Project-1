using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteWindow : MonoBehaviour
{
    public GameObject window;
    public TextMeshProUGUI noteText;
    public Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(CloseWindow);
    }

    public void ShowNote(string noteContent)
    {
        noteText.text = noteContent;
        window.SetActive(true);
    }

    public void CloseWindow()
    {
        window.SetActive(false);
    }

}
