//This class is on the main camera to follow player.
//You may optimize it on SetPosition section and
//Write a proper way to update blocks positions on the map to make it an infite gameplay.

using UnityEngine;

public class PlayerFollower : MonoBehaviour {

    private Transform player;
    private Vector3 offset;

    public void SetPosition(Transform p)
    {
        player = p;
        offset = player.transform.position - transform.position;
    }

    private void Start()
    {
        SetPosition(GameObject.Find("Player").transform);
    }

    private void LateUpdate()
    {
        if(player != null)
        {
            BlockCreator.GetSingleton().UpdateBlockPosition(player.transform);
            transform.position = new Vector3(transform.position.x, player.transform.position.y - offset.y, player.transform.position.z - offset.z);
        }
    }

}
