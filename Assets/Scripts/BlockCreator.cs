using System.Collections.Generic;
using UnityEngine;


//In this class, the map has been created.
//You have to edit GetRelativeBlock section to calculate current relative block to cast player rope to hold on
//Update Block Position section to make infinite map.
public class BlockCreator : MonoBehaviour {

    private static BlockCreator singleton = null;
    private GameObject[] blockPrefabs;
    private GameObject pointPrefab;
    private GameObject pointObject;
    public int blockCount;

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private int difficulty = 1;

    private int zToSpawn = 1;
    private int blockWidth = 1;
    private int firstSpawnBlockCount = 20;
    private List<GameObject> upperBlocks;
    private GameObject[] sceneBlockes;

    private float upperPositionLimitDown, upperPositionLimitUp;
    private float downerPositionLimitDown, downerPositionLimitUp;
    private float yOffset = 10;
    private float roadmapThreshold = 5f;

    #region Singleton Getter
    public static BlockCreator GetSingleton()
    {
        if (singleton == null)
        {
            singleton = new GameObject("_BlockCreator").AddComponent<BlockCreator>();
        }
        return singleton;
    }
    #endregion

    #region ObjectPooler Setter&Getter
    public void Initialize(int bCount, GameObject[] bPrefabs, GameObject pPrefab)
    {
        blockCount = bCount;
        blockPrefabs = bPrefabs;
        pointPrefab = pPrefab;
        InstantiateBlocks();
    }


    //Object Pooler Instantiation
    public void InstantiateBlocks()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        GameObject parentEmptyObject = GameObject.Find("PooledObjects");
        foreach (GameObject block in blockPrefabs)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < blockCount; i++)
            {
                GameObject obj = Instantiate(block);
                obj.transform.parent = parentEmptyObject.transform;
                //obj.hideFlags = HideFlags.HideInHierarchy;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(block.name, objectPool);
        }
    }

    //Pulling pooled object
    public GameObject GetPooledBlock(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("TAG is NULL!!!");
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    #endregion

    private void Start()
    {
        upperPositionLimitDown = -1f;
        upperPositionLimitUp = +1f;

        downerPositionLimitDown = -1f;
        downerPositionLimitUp = +1f;

        upperBlocks = new List<GameObject>();

        for (int i = 0; i < firstSpawnBlockCount; i++)
        {
            SpawnBlock();
        }

        sceneBlockes = GameObject.FindGameObjectsWithTag("Block");
    }

    #region Block Adder&Deleter
    private void SpawnBlock()
    {
        float RandomYUpper = Random.Range(upperPositionLimitDown, upperPositionLimitUp) + yOffset;
        float RandomYDowner = Random.Range(downerPositionLimitDown, downerPositionLimitUp) + yOffset;

        int blockIndex = Random.Range(0, blockPrefabs.Length);
        GameObject pooledUpper = GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) + (Vector3.up * RandomYUpper), Quaternion.identity);

        blockIndex = Random.Range(0, blockPrefabs.Length);
        GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) + (Vector3.down * RandomYDowner), Quaternion.identity);

        zToSpawn += blockWidth;
        upperBlocks.Add(pooledUpper);
    }

    void DeleteBlock()
    {
        for (int i = 1; i >= 0; i--)
        {
            if (sceneBlockes[i] != null)
            {
                Destroy(sceneBlockes[i]);
            }
            upperBlocks[i].SetActive(false);
            upperBlocks.RemoveAt(i);
        }
    }

    #endregion

    public void UpdateBlockPosition(Transform player)
    {
        if (player.transform.position.z - 4f > zToSpawn - (firstSpawnBlockCount * blockWidth))
        {
            SpawnBlock();

            //daha güzel yazılabilir.
            if (sceneBlockes[0] != null)
            {
                Destroy(sceneBlockes[0]);
                Destroy(sceneBlockes[1]);
                Debug.Log("Destroyed");
            }
        }

        //daha güzel yazılabilir.
        if (player.transform.position.z > roadmapThreshold)
        {
            upperPositionLimitDown+= .1f;
            upperPositionLimitUp += .1f;
            downerPositionLimitDown-= .1f;
            downerPositionLimitUp -= .1f;

            roadmapThreshold *= 2;
        }
    }

    public Transform GetRelativeBlock(float playerPosZ)
    {
        int indexBlock = (int)Mathf.Round(playerPosZ) + 2;
        return upperBlocks[indexBlock].transform;

        //You may need this type of getter to which block are we going to cast our rope into
    }
}
