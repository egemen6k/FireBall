using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;


public class GUIController : MonoBehaviour
{
    public Text scoreText;
    public Text highscoreText;
    public Text realtimeScoreText;
    public GameObject gameOverPanel;
    public GameObject holdStartText;


    public void HandleRestartButton()
    {

        SceneManager.LoadScene("Game");
        /*BlockCreator singleton = BlockCreator.GetSingleton();
        singleton = null;*/
    }
   
}
