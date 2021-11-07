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

    private List<GameObject> blockPool = new List<GameObject>();
    private float lastHeightUpperBlock = 10;
    private int difficulty = 1;

    public static BlockCreator GetSingleton()
    {
        if(singleton == null)
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
    
	
    public void InstantiateBlocks()
    {
        //Instantiate blocks here
    }

    void Update () {
		
	}

    public Transform GetRelativeBlock(float playerPosZ)
    {
        //You may need this type of getter to which block are we going to cast our rope into
        return null;
    }

    public void UpdateBlockPosition(int blockIndex)
    {
        //Block Pool has been created. Find a proper way to make infite map when it is needed
    }
}
