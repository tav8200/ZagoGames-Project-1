using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    public List<GameObject> blockList = new List<GameObject>();

    public int worldWidth = 5;
    public int worldHeight = 5;

    public float gridOffset = 2f;

    public void Start()
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                Vector3 pos = new Vector3(x * gridOffset, y * gridOffset, 0);
                GameObject block = Instantiate(blockList[Random.Range(0, blockList.Count)], pos, Quaternion.identity);

                block.transform.SetParent(this.transform);
            }
        }
    }
}
