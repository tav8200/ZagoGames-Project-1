using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public GameObject player;
    
    public CharacterStats playerStat;

    public Animator transition;

    public float transitionTime = 1f;

    public void Awake()
    {
        if (player != null)
        {
            playerStat = player.GetComponent<CharacterStats>();
        }
    }

    public void ChangeScene(int sceneNo)
    {
        StartCoroutine(LoadLevel(sceneNo));

        if (sceneNo == 0)
        {
            playerStat.Die(); //Reset player if they quit
        }

        Time.timeScale = 1.0f; //Resume time again after qutting from pause
    }
    
    public void ExitGame()
    {
        if (player != null)
        {
            playerStat.Die();
        }

        Application.Quit();
    }

    public void OnApplicationQuit()
    {
        if (player != null)
        {
            playerStat.Die();
        }

        PlayerPrefs.Save();
    }

    IEnumerator LoadLevel(int levelIndex) //Fade to black transition
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
