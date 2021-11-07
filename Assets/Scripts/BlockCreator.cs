﻿using System.Collections.Generic;
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
    private int blocksSize;
    private List<GameObject> activeBlocks;
    private GameObject[] sceneBlockes;

    public static BlockCreator GetSingleton()
    {
        if (singleton == null)
        {
            singleton = new GameObject("_BlockCreator").AddComponent<BlockCreator>();
        }
        return singleton;
    }

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

    private void Start()
    {
        blocksSize = poolDictionary.Count;
        activeBlocks = new List<GameObject>();

        for (int i = 0; i < firstSpawnBlockCount; i++)
        {
            SpawnBlock();
        }

        sceneBlockes = GameObject.FindGameObjectsWithTag("Block");
    }

    private void SpawnBlock()
    {
        int RandomY = Random.Range(8, 13);

        int blockIndex = Random.Range(0, blockPrefabs.Length);
        GameObject pooledUpper = GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) + (Vector3.up * RandomY), Quaternion.identity);

        blockIndex = Random.Range(0, blockPrefabs.Length);
        GameObject pooledDowner = GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) - (Vector3.up * RandomY), Quaternion.identity);

        zToSpawn += blockWidth;
        activeBlocks.Add(pooledUpper);
        activeBlocks.Add(pooledDowner);
    }

    void Update()
    {

    }

    void DeleteBlock()
    {
        for (int i = 1; i >= 0; i--)
        {
            Destroy(sceneBlockes[i]);
            activeBlocks[i].SetActive(false);
            activeBlocks.RemoveAt(i);
        }
    }

    //Yukarı ve aşağı ilerlemeli hissi için transform.z ile değişen Y offset değeri eklenecek.

    public Transform GetRelativeBlock(float playerPosZ)
    {
        //You may need this type of getter to which block are we going to cast our rope into
        return null;
    }

    public void UpdateBlockPosition(Transform player)
    {
        if (player.transform.position.z - 10f > zToSpawn - (firstSpawnBlockCount * blockWidth))
        {
            SpawnBlock();
            DeleteBlock();
        }
    }
}
