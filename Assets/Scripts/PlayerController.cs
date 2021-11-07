using UnityEngine;
using System.Collections;

//In this section, you have to edit OnPointerDown and OnPointerUp sections to make the game behave in a proper way using hJoint
//Hint: You may want to Destroy and recreate the hinge Joint on the object. For a beautiful gameplay experience, joint would created after a little while (0.2 seconds f.e.) to create mechanical challege for the player
//And also create fixed update to make score calculated real time properly.
//Update FindRelativePosForHingeJoint to calculate the position for you rope to connect dynamically
//You may add up new functions into this class to make it look more understandable and cosmetically great.

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private GameObject[] blockPrefabs;
    [SerializeField]
    private HingeJoint hJoint;
    [SerializeField]
    private LineRenderer lRenderer;
    [SerializeField]
    private Rigidbody playerRigidbody;
    [SerializeField]
    private PlayerFollower playerFollower;
    [SerializeField]
    private GameObject pointPrefab;
    [SerializeField]
    
    private GUIController guiController;

    private float score;

    private bool gameOver = false;

    private Vector3 yOffset;

    private Vector3 playerPositionComparer = Vector3.zero;
    private float scoreUpdatePositionThreshold = .1f;

    private bool isJointing = false;

	void Start ()
    {
        BlockCreator.GetSingleton().Initialize(30, blockPrefabs, pointPrefab);
        FindRelativePosForHingeJoint(new Vector3(0, 5.5f ,0));

        yOffset = new Vector3(0, 4.5f, 0);
	}
	
    public void FindRelativePosForHingeJoint(Vector3 blockPosition)
    {
        if(!isJointing)
            StartCoroutine(JointCreater(blockPosition));
    }

    IEnumerator JointCreater(Vector3 blockPosition)
    {
        isJointing = true;
        yield return new WaitForSeconds(0.2f);

        transform.rotation = Quaternion.identity;
        if (GetComponent<HingeJoint>() == null)
        {
            hJoint = gameObject.AddComponent<HingeJoint>();
        }
        hJoint.anchor = (blockPosition - transform.position) ;

        playerRigidbody.AddRelativeForce(Vector3.forward * 50f);
        lRenderer.enabled = true;
        lRenderer.SetPosition(1, hJoint.anchor);
        lRenderer.enabled = true;
        isJointing = false;
    }

    public void PointerDown()
    {
        Debug.Log("Pointer Down");

        Transform blockToCatch = BlockCreator.GetSingleton().GetRelativeBlock(transform.position.z);
        FindRelativePosForHingeJoint(blockToCatch.position - yOffset);
    }

    public void PointerUp()
    {
        Debug.Log("Pointer Up");

        if (!gameOver)
        {
            if(hJoint != null)
                hJoint.breakForce = 1f;
            lRenderer.enabled = false;
        }     
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Equals("Block") && !gameOver)
        {
            gameOver = true;
            PointerUp(); //Finishes the game here to stoping holding behaviour

            guiController.scoreText.text = score.ToString("0.00");
            //If you know a more modular way to update UI, change the code below
            if(PlayerPrefs.HasKey("HighScore"))
            {
                float highestScore = PlayerPrefs.GetFloat("HighScore");
                if(score > highestScore)
                {
                    PlayerPrefs.SetFloat("HighScore", score);
                    guiController.highscoreText.text = "HighestScore: " + score.ToString("0.00");
                }
                else
                {
                    guiController.highscoreText.text = "HighestScore: " + highestScore.ToString("0.00");
                }
            }
            else
            {
                PlayerPrefs.SetFloat("HighScore", score);
                guiController.highscoreText.text = "HighestScore: " + score.ToString("0.00");
            }
            guiController.gameOverPanel.SetActive(true);
        }
    }

    #region ScoreUpdate
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Point"))
        {
            if(Vector3.Distance(transform.position, other.gameObject.transform.position) < .5f)
            {
                score += 10f;
            }
            else
            {
                score += 5f;
            }
            other.gameObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
       SetScore();
        
    }
    public void SetScore()
    {
        if(transform.position.z - playerPositionComparer.z > scoreUpdatePositionThreshold)
        {
            score += 0.1f;
            playerPositionComparer = transform.position;
        }

        guiController.realtimeScoreText.text = score.ToString("0.00");
    }

    #endregion
}
