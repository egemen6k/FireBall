using System.Collections.Generic;
using UnityEngine;


//In this class, the map has been created.
//You have to edit GetRelativeBlock section to calculate current relative block to cast player rope to hold on
//Update Block Position section to make infinite map.
public class BlockCreator : MonoBehaviour {

    private static BlockCreator singleton = null;
    private GameObject[] blockPrefabs;
    private GameObject pointPrefab;

    private Transform playerTR;
    private Vector3 yOffsetPoint = new Vector3(0, 10f, 0);

    public int blockCount;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private int zToSpawn = 1;
    private int blockWidth = 1;
    private int firstSpawnBlockCount = 20;
    private List<GameObject> upperBlocks;
    private GameObject[] sceneBlockes;

    private bool limitHeight = false;
    private float yOffset = 10;
    private float roadmapThreshold = 5f;

    private float pointCountDownTimer = 5f;



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
        playerTR = GameObject.Find("Player").transform;

        upperBlocks = new List<GameObject>();

        for (int i = 0; i < firstSpawnBlockCount; i++)
        {
            SpawnBlock();
        }

        sceneBlockes = GameObject.FindGameObjectsWithTag("Block");
    }

    #region Block Adder & Updater
    private void SpawnBlock()
    {
        float RandomYUpper = Random.Range(-1.5f,1.5f) + yOffset;
        float RandomYDowner = Random.Range(-1.5f,1.5f) + yOffset - 20;

        int blockIndex = Random.Range(0, blockPrefabs.Length);
        GameObject pooledUpper = GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) + (Vector3.up * RandomYUpper), Quaternion.identity);

        blockIndex = Random.Range(0, blockPrefabs.Length);
        GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) + (Vector3.up * RandomYDowner), Quaternion.identity);

        zToSpawn += blockWidth;
        upperBlocks.Add(pooledUpper);
    }


    public void UpdateBlockPosition(Transform player)
    {
        if (player.transform.position.z - 4f > zToSpawn - (firstSpawnBlockCount * blockWidth))
        {
            SpawnBlock();

            if (sceneBlockes[0] != null)
            {
                Destroy(sceneBlockes[0]);
                Destroy(sceneBlockes[1]);
            }

            if (!limitHeight)
            {
                yOffset += .2f;

                if (yOffset >= 15f)
                {
                    limitHeight = true;
                }
            }
            else
            {
                yOffset -= .2f;

                if (yOffset <= 10f)
                {
                    limitHeight = false;
                }
            }
        }
    }

    #endregion

    private void Update()
    {
        if (pointCountDownTimer <= 0)
        {
            PointSpawn();
            pointCountDownTimer = 5f;
        }

        pointCountDownTimer -= Time.deltaTime;
    }

    private void PointSpawn()
    {
        Instantiate(pointPrefab, GetRelativeBlock(playerTR.position.z + 4f).position - yOffsetPoint, pointPrefab.transform.rotation);
    }

    public Transform GetRelativeBlock(float playerPosZ)
    {
        int indexBlock = (int)Mathf.Round(playerPosZ) + 3;
        return upperBlocks[indexBlock].transform;
    }
}
