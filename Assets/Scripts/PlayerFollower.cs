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


    //Camera position set after player movement
    private void LateUpdate()
    {
        if(player != null)
        {
            BlockCreator.GetSingleton().UpdateBlockPosition(player.transform);
            transform.position = new Vector3(transform.position.x, player.transform.position.y - offset.y, player.transform.position.z - offset.z);
        }
    }

}
