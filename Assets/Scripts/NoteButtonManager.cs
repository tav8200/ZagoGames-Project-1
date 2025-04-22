using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteButtonManager : MonoBehaviour
{
    public List<Button> noteButtonList; //List of buttons which display different notes
    public NoteWindow noteWindow;

    public void Start()
    {
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        if (NoteManager.Instance == null)
        {
            return;
        }

        for (int i = 0; i < noteButtonList.Count; i++)
        {
            string note = NoteManager.Instance.loreList[i];
            bool isFound = NoteManager.Instance.FoundNotesContains(note);

            noteButtonList[i].interactable = isFound;

            noteButtonList[i].onClick.RemoveAllListeners();

            if (isFound)
            {
                noteButtonList[i].onClick.AddListener(() => noteWindow.ShowNote(note));
            }
        }
    }
}
