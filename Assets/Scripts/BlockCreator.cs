﻿using System.Collections.Generic;
using UnityEngine;

public class BlockCreator : MonoBehaviour {

    private static BlockCreator singleton = null;
    private int blockCount;
    private GameObject[] blockPrefabs;
    private GameObject pointPrefab;

    private Transform playerTR;
    private float pointThresholdZ = 5f;
    private Vector3 pointSpawnOffset = new Vector3(0, 10f, -6);

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private float randomYUpper, randomYDowner;
    private int zToSpawn = 1;
    private int blockWidth = 1;
    private int firstSpawnBlockCount = 20;
    private List<GameObject> upperBlocks;
    private GameObject[] sceneBlockes;

    private bool limitHeight = false;
    private float yOffset = 10;

    private float pointCountDownTimer = 5f;

    private int relativeBlockOffset = 3;

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
        sceneBlockes = GameObject.FindGameObjectsWithTag("Block");
        playerTR = GameObject.Find("Player").transform;
        upperBlocks = new List<GameObject>();

        for (int i = 0; i < firstSpawnBlockCount; i++)
        {
            SpawnBlock();
        }
    }

    #region Block Adder & Updater
    private void SpawnBlock()
    {
        float randomValue = Random.Range(-1.5f, 1.5f);

        randomYUpper =  randomValue + yOffset;
        randomYDowner = randomValue + yOffset - 20;

        int blockIndex = Random.Range(0, blockPrefabs.Length);
        GameObject pooledUpper = GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) + (Vector3.up * randomYUpper), Quaternion.identity);
        upperBlocks.Add(pooledUpper);

        blockIndex = Random.Range(0, blockPrefabs.Length);
        GetPooledBlock(blockPrefabs[blockIndex].name, (Vector3.forward * zToSpawn) + (Vector3.up * randomYDowner), Quaternion.identity);

        zToSpawn += blockWidth;
    }
    public void UpdateBlockPosition(Transform player)
    {
        if (player.transform.position.z - 4f > zToSpawn - (firstSpawnBlockCount * blockWidth))
        {
            SpawnBlock();

            if (!limitHeight)
            {
                yOffset += .2f;

                if (yOffset >= 14f)
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

            if (sceneBlockes[0] != null)
            {
                Destroy(sceneBlockes[0]);
                Destroy(sceneBlockes[1]);
            }
        }
    }
    #endregion

    private void Update()
    {
        if (pointCountDownTimer <= 0 && playerTR.position.z > pointThresholdZ)
        {
            PointSpawn();
            pointCountDownTimer = 5f;
            pointThresholdZ += 30;
        }
        pointCountDownTimer -= Time.deltaTime;
    }
    private void PointSpawn()
    {
        Instantiate(pointPrefab, GetRelativeBlock(playerTR.position.z).position - pointSpawnOffset, pointPrefab.transform.rotation);
    }

    public Transform GetRelativeBlock(float playerPosZ)
    {
        int indexBlock = (int)Mathf.Round(playerPosZ) + relativeBlockOffset;
        return upperBlocks[indexBlock].transform;
    }
}
