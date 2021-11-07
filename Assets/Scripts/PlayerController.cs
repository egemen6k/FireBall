using UnityEngine;
using System.Collections;

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
    private GameObject pointPrefab;
    [SerializeField]
    private GUIController guiController;
    [SerializeField] 
    private GameObject particleEffect;

    private BlockCreator blockCreator;

    private bool gameOver = false;
    private Vector3 yOffset = new Vector3(0, 4.5f, 0);
    private bool isJointing = false;

    private Vector3 playerPositionComparer = Vector3.zero;
    private float score;
    private float scoreUpdateZDifference = .1f;


	void Start ()
    {
        blockCreator = BlockCreator.GetSingleton();
        blockCreator.Initialize(30, blockPrefabs, pointPrefab);

        FindRelativePosForHingeJoint(new Vector3(0, 5.5f ,0));
        playerRigidbody.AddRelativeForce(Vector3.forward * 75f);
	}

    #region JointMaker
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
        hJoint.anchor = (blockPosition - transform.position);

        playerRigidbody.AddRelativeForce(Vector3.forward * 75f);
        lRenderer.enabled = true;
        lRenderer.SetPosition(1, hJoint.anchor);
        lRenderer.enabled = true;
        isJointing = false;
    }
    #endregion

    #region Pointer Callbacks
    public void PointerDown()
    {
        Transform blockToCatch = blockCreator.GetRelativeBlock(transform.position.z);
        FindRelativePosForHingeJoint(blockToCatch.position - yOffset);
        particleEffect.SetActive(true);
    }

    public void PointerUp()
    {
        if (!gameOver)
        {
            if(hJoint != null)
                hJoint.breakForce = 1f;
            lRenderer.enabled = false;
            particleEffect.SetActive(false);
        }     
    }
    #endregion

    #region EndGame
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
    #endregion

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

            Transform parentTarget = other.transform.parent;
            Destroy(parentTarget.gameObject);    
        }
    }
    private void FixedUpdate()
    {
       SetScore();
        
    }
    public void SetScore()
    {
        if(transform.position.z - playerPositionComparer.z > scoreUpdateZDifference)
        {
            score += 0.1f;
            playerPositionComparer = transform.position;
        }
        guiController.realtimeScoreText.text = score.ToString("0.00");
    }
    #endregion
}
