using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMenuGrid : MonoBehaviour
{
    public List<GameObject> blockList = new List<GameObject>(); //List of possible world blocks
    public List<GameObject> enemyList = new List<GameObject>(); //List of possible enemy types
    private List<Vector3> validPositions = new List<Vector3>(); //Keeps track of positions the player could possibly spawn in

    public GameObject player;
    public GameObject wall;
    public GameObject sideWall;
    public GameObject hatch;

    private CharacterStats playerStat;
    private NewFloor newFloor;

    public int worldWidth = 5;
    public int worldHeight = 5;
    public int enemyCount = 30;

    public float gridOffset = 2f; //Space between tile spawns
    public float horizontalWallOffset = 2f;
    public float leftWallOffset = 2f;
    public float rightWallOffset = 2f;
    public float cellSize = 1f;
    public float minHatchDistance = 5f; //Minimum distance hatch can spawn from player, increases as floors get bigger
    public float minEnemyDistance = 50f;

    public LayerMask collisionLayer; //Layer that hatch won't spawn inside of

    public void Start()
    {
        BoxCollider2D boxCollider = hatch.GetComponent<BoxCollider2D>();
        Vector2 boxSize = boxCollider.size * hatch.transform.localScale;

        newFloor = hatch.GetComponent<NewFloor>();

        for (int x = 0; x < worldWidth; x++) //Create level with random tiles
        {
            for (int y = 0; y < worldHeight; y++)
            {
                Vector3 pos = new Vector3(x * gridOffset, y * gridOffset, 0);
                GameObject block = Instantiate(blockList[Random.Range(0, blockList.Count)], pos, Quaternion.identity);

                block.transform.SetParent(this.transform);

                validPositions.Add(pos);  //Generate list of valid spawn positions
            }
        }

        for (int x = 0; x < worldWidth; x++) //Spawn walls on top and bottom of world grid
        {
            Vector3 topPosition = new Vector3(x * cellSize, worldHeight * cellSize - horizontalWallOffset, 0); //Spawn walls along top of world
            Instantiate(wall, topPosition, Quaternion.identity);

            Vector3 bottomPosition = new Vector3(x * cellSize, -horizontalWallOffset, 0); //Spawn walls along bottom of world
            Instantiate(wall, bottomPosition, Quaternion.identity);
        }

        for (int y = 0; y < worldHeight; y++) //Spawn walls to the left and right of world grid
        {
            Vector3 leftPosition = new Vector3(leftWallOffset, y * cellSize, 0); //Spawn walls along left side of world
            Instantiate(sideWall, leftPosition, Quaternion.identity);

            Vector3 rightPosition = new Vector3(worldWidth * cellSize - rightWallOffset, y * cellSize, 0); //Spawn walls along right side of world
            Instantiate(sideWall, rightPosition, Quaternion.identity);
        }

        Vector3 cameraSpawn = new Vector3(100f, 100f, -10f); //Spawn camera in center of grid to prevent wall clipping
        player.transform.position = cameraSpawn;

        for (int i = 0; i < validPositions.Count; i++) //Hatch spawn
        {
            Vector3 hatchSpawn = validPositions[Random.Range(0, validPositions.Count)];

            Collider2D hitCollider = Physics2D.OverlapBox(hatchSpawn, boxSize, 0f, collisionLayer); //Check if there is an object where the hatch would spawn

            if (hitCollider == null && Vector3.Distance(hatchSpawn, cameraSpawn) >= minHatchDistance)
            {
                Instantiate(hatch, hatchSpawn, Quaternion.identity);
                validPositions.Remove(hatchSpawn);
                break;
            }
            else
            {
                Debug.Log("Hatch position denied");
            }
        }

        for (int i = 0; i < enemyCount; i++) //Enemy spawn
        {
            int enemyToSpawn = Random.Range(0, enemyList.Count);
            Vector3 enemySpawnPos = validPositions[Random.Range(0, validPositions.Count)];
            Instantiate(enemyList[enemyToSpawn], enemySpawnPos, Quaternion.identity);
            validPositions.Remove(enemySpawnPos);
        }
    }
}
