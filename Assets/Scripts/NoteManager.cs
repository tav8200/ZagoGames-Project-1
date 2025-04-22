using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance {get; private set;}

    public List<string> loreList; //Full list of lore notes
    public HashSet<string> foundNotes = new HashSet<string>(); //Hashset of only the found notes

    private const string PlayerPrefsKey = "FoundNotes"; //Playerprefs key, value is a large string of all remaining notes

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadFoundNotes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string FindRandomNote()
    {
        List<string> availableNotes = new List<string>(loreList);
        availableNotes.RemoveAll(note => foundNotes.Contains(note));

        if (availableNotes.Count == 0)
        {
            Debug.Log("No more notes");
            return null;
        }

        string randomNote = availableNotes[Random.Range(0, availableNotes.Count)];
        foundNotes.Add(randomNote);

        SaveFoundNotes();

        return randomNote;
    }

    private void SaveFoundNotes() //Join all remaining notes into a string and store in playerprefs
    {
        string serializedNotes = string.Join("|", foundNotes);
        PlayerPrefs.SetString(PlayerPrefsKey, serializedNotes);
        PlayerPrefs.Save();
    }

    private void LoadFoundNotes() //Return the single playerprefs string into multiple strings
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string serializedNotes = PlayerPrefs.GetString(PlayerPrefsKey);
            foundNotes = new HashSet<string>(serializedNotes.Split("|"));
        }
    }

    public void ResetFoundNotes()
    {
        foundNotes.Clear();
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
    }

    public bool FoundNotesContains(string note)
    {
        return foundNotes.Contains(note);
    }
}
