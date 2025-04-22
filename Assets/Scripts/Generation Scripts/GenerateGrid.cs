using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    public List<GameObject> blockList = new List<GameObject>(); //List of possible world blocks
    public List<GameObject> enemyList = new List<GameObject>(); //List of possible enemy types
    public List<Vector3> validPositions = new List<Vector3>(); //Keeps track of positions the player could possibly spawn in
    public List<Vector3> validPositionsFinal = new List<Vector3>(); //Final list of valid positions, for keeping track of where tv can teleport to

    public GameObject player;
    public GameObject wall;
    public GameObject sideWall;
    public GameObject hatch;

    private CharacterStats playerStat;
    private NewFloor newFloor;

    public int worldWidth = 5;
    public int worldHeight = 5;
    public int minEnemies = 3;
    public int maxEnemies = 30;

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
        worldWidth = PlayerPrefs.GetInt("gridWidth", worldWidth);
        worldHeight = PlayerPrefs.GetInt("gridHeight", worldHeight);

        BoxCollider2D boxCollider = hatch.GetComponent<BoxCollider2D>();
        Vector2 boxSize = boxCollider.size * hatch.transform.localScale;

        playerStat = player.GetComponent<CharacterStats>();
        newFloor = hatch.GetComponent<NewFloor>();

        for (int x = 0; x < worldWidth; x++) //Create level with random tiles
        {
            for (int y = 0; y < worldHeight; y++)
            {
                Vector3 pos = new Vector3(x * gridOffset, y * gridOffset, 0);
                GameObject block = Instantiate(blockList[Random.Range(0, blockList.Count)], pos, Quaternion.identity);

                block.transform.SetParent(this.transform);

                validPositions.Add(pos);
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

        Vector3 randSpawn = validPositions[Random.Range(0, validPositions.Count)]; //Randomly spawn player in grid
        player.transform.position = randSpawn;
        validPositions.Remove(randSpawn);

        for (int i = 0; i < validPositions.Count; i++) //Hatch spawn
        {
            Vector3 hatchSpawn = validPositions[Random.Range(0, validPositions.Count)];

            Collider2D hitCollider = Physics2D.OverlapBox(hatchSpawn, boxSize, 0f, collisionLayer); //Check if there is an object where the hatch would spawn

            if (hitCollider == null && Vector3.Distance(hatchSpawn, randSpawn) >= minHatchDistance)
            {
                Instantiate(hatch, hatchSpawn, Quaternion.identity);
                validPositions.Remove(hatchSpawn);
                break; //Exit loop if a position is found so only one hatch spawns
            }
            else
            {
                Debug.Log("Hatch position denied");
            }
        }

        int maxEnemySpawn = Mathf.RoundToInt(Mathf.Lerp(minEnemies, maxEnemies, (1 - playerStat.currentSanity / 100f) * 0.5f + (newFloor.floorNum / 100f) * 0.5f)); //Mathf.RoundToInt(Mathf.Lerp(minEnemies, maxEnemies, (1 - playerStat.currentSanity / 100f) * (newFloor.floorNum / 100f))); //Scaling number of maximum enemies
        Debug.Log("Max number of enemies: " + maxEnemySpawn);
        for (int i = 0; i < maxEnemySpawn; i++) //Enemy spawn
        {
            int enemyToSpawn = Random.Range(0, enemyList.Count);
            Vector3 enemySpawnPos = validPositions[Random.Range(0, validPositions.Count)];

            if (Vector2.Distance(enemySpawnPos, randSpawn) >= minEnemyDistance) //Spawn enemies in random locations
            {
                Instantiate(enemyList[enemyToSpawn], enemySpawnPos, Quaternion.identity);
                validPositions.Remove(enemySpawnPos);
                AstarPath.active.Scan(); //Scan for obstacles everytime an enemy spawns just to guarantee a proper scan
            }
        }

        validPositionsFinal.AddRange(validPositions);
    }
}
