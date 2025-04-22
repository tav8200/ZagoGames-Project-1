using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewFloor : MonoBehaviour
{
    public int floorNum = 1;

    private bool isColliding = false;
    private bool canDescend = true;

    public CharacterStats playerStat;

    private GenerateGrid worldSize;
    private Scene_Manager sceneManager;

    [SerializeField] private AudioClip descendSound;

    private void Awake()
    {
        canDescend = true; //Reset ability to descend

        floorNum = PlayerPrefs.GetInt("floorNumber", floorNum);
        Debug.Log("Floor number: " + floorNum);

        GameObject player = GameObject.FindWithTag("Player");
        playerStat = player.GetComponent<CharacterStats>();

        GameObject scene = GameObject.FindWithTag("Scene Change");
        sceneManager = scene.GetComponent<Scene_Manager>();

        GameObject world = GameObject.FindWithTag("Respawn"); //World controller
        if (world != null)
        {
            worldSize = world.GetComponent<GenerateGrid>();
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isColliding = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isColliding = false;
        }
    }

    private void Update()
    {
        if (isColliding && canDescend && Input.GetKeyDown(KeyCode.E) )
        {
            canDescend = false; //Prevent multiple e inputs to descend multiple floors at a time
            SoundFXManager.Instance.PlaySoundFXClip(descendSound, transform, 1f);

            if (PlayerPrefs.GetInt("floorNumber", floorNum) % 2 == 0) //Increase size of grid only every other floor
            {
                worldSize.worldWidth++; //worldSize.worldWidth++;
                worldSize.worldHeight++; //worldSize.worldHeight++;

                PlayerPrefs.SetInt("gridWidth", worldSize.worldWidth);
                PlayerPrefs.SetInt("gridHeight", worldSize.worldHeight);
            }

            playerStat.TakeSanityDamage(5); //Decrease sanity every floor
            PlayerPrefs.SetInt("sanityValue", playerStat.currentSanity);

            floorNum++;
            PlayerPrefs.SetInt("floorNumber", floorNum);

            PlayerPrefs.Save();

            sceneManager.ChangeScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
