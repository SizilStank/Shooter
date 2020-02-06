using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Image _livesImg;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _loadNewGameText;

    [SerializeField] private bool _isGameOverFlicker;

    GameManagerScript _gameManagerScript;


    // Start is called before the first frame update
    void Start()
    {
        
        _gameManagerScript = GameObject.Find("GameManagerObject").GetComponent<GameManagerScript>();

        if (_gameManagerScript == null)
        {
            Debug.Log("GameManagerScript is NULL!");
        }

        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _loadNewGameText.text = "";
    }

    public void UpdateScore(int playerSocre)
    {
        _scoreText.text = "Score " + playerSocre.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        currentLives = Mathf.Clamp(currentLives, 0, 4);

        if (currentLives == 4)
        {
            return;
        }
            _livesImg.sprite = _liveSprites[currentLives];
             

        if (currentLives == 0)
        {
            _gameManagerScript.GameOver();
            _isGameOverFlicker = true;
            StartCoroutine(GameOverFlickerRoutine());
            _loadNewGameText.text = "Press R to restart game!";
        }
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (_isGameOverFlicker == true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(.5f);
            _gameOverText.gameObject.SetActive(true);
        }
    }
}
