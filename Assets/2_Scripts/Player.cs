using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _speedPowerUpSpeed = 10f;


    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleLaserPrefab;
    [SerializeField] private GameObject _shieldAnimationPrefab;

    [SerializeField] private AudioClip _laserShoot;
    [SerializeField] private AudioClip _explosion;
    [SerializeField] private AudioClip _powerUpClip;
    [SerializeField] private AudioClip _shieldTakeDamage;
    [SerializeField] private AudioClip _playerHit;
    [SerializeField] private float _volumeMax = 1f;
    [SerializeField] private float _volumeMin = 0.25f;

    [SerializeField] private GameObject _playerDamage_L;
    [SerializeField] private GameObject _playerDamage_R;


    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private float _canFire = -1f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private int _shieldLives = 3;
    [SerializeField] private int _score;


    [SerializeField] private bool _isTripleShotActive;
    [SerializeField] private bool _isSpeedPowerUpActive;
    [SerializeField] private bool _isShieldActive;


    [SerializeField] private float _activeTripleShotTime = 5f;
    [SerializeField] private float _activeSpeedPowerUpTime = 5f;
    [SerializeField] private float _activeShieldPowerUpTime = 5;


    [SerializeField] private GameObject[] _setRandomPlayerWingDamage;
    [SerializeField] private int _currentIndex = 0;


    private SpawnManager _spawnManager;//handle to component... then FIND it
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private ShieldAnim _shieldAnim;



    private void Start()
    {
        PlayerComponets();
    }

    private void PlayerComponets()
    {
        _playerDamage_L.SetActive(false);
        _playerDamage_R.SetActive(false);

        if (_shieldAnim != null)
        {
            _shieldAnim = GameObject.Find("ShieldAnim").GetComponent<ShieldAnim>();
        }

        _audioSource = GetComponent<AudioSource>();
        if (_laserShoot == null)
        {
            Debug.LogError("The Audio Source on Player is NULL!");
        }
        else
        {
            _audioSource.clip = _laserShoot;
        }

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL!");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        
        if (Input.GetMouseButtonDown(0) && Time.time > _canFire)
        {
            _audioSource.PlayOneShot(_laserShoot, _volumeMin);
            FireLaser();
        }
    }

    void PlayerMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedPowerUpActive == true)
        {         
            transform.Translate(moveDirection * _speedPowerUpSpeed * Time.deltaTime);
        }
        else
        {
            transform.Translate(moveDirection * _speed * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 1.7f), 0);

        if (transform.position.x >= 11.27f)
        {
            transform.position = new Vector3(-11.27f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.27f)
        {
            transform.position = new Vector3(11.27f, transform.position.y, 0);
        }
    }


    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        
        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleLaserPrefab, transform.position + new Vector3(-0.078f, 0.126f, 0), Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.0f, 0), Quaternion.identity);
        }
    }

    public void Damage()
    {
        if (_isShieldActive == true)// we need this so the player does not take damage while in the shield
        {
            return;
        }

        _lives --;

        _audioSource.PlayOneShot(_playerHit, _volumeMax);

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _audioSource.PlayOneShot(_explosion, _volumeMax);

            _spawnManager.OnPlayerDeath();

            Destroy(this.gameObject);
        }


        if (_lives == 2)
        {
            _setRandomPlayerWingDamage[Random.Range(0, _setRandomPlayerWingDamage.Length)].SetActive(true);
        }
        else if (_lives == 1)
        {
            _setRandomPlayerWingDamage[0].SetActive(true);
            _setRandomPlayerWingDamage[1].SetActive(true);
        }     
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Enemy" && _isShieldActive == true)
        {
            
            _shieldLives--;

            if (_shieldLives == 2)
            {
                _audioSource.PlayOneShot(_shieldTakeDamage, _volumeMax);
                _shieldAnimationPrefab.GetComponent<Renderer>().material.color = Color.green;
            }
            else if (_shieldLives == 1)
            {
                _audioSource.PlayOneShot(_shieldTakeDamage, _volumeMax);
                _shieldAnimationPrefab.GetComponent<Renderer>().material.color = Color.red;
            }

            if (_shieldLives < 1)
            {
                _audioSource.PlayOneShot(_shieldTakeDamage, _volumeMax);
                _isShieldActive = false;
                _shieldLives = 3;
                _shieldAnimationPrefab.GetComponent<Renderer>().material.color = Color.white;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        if (other.gameObject.tag == "PowerUp")
        {
            _audioSource.PlayOneShot(_powerUpClip, _volumeMin);
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        if (_isTripleShotActive == true)
        {
            yield return new WaitForSeconds(_activeTripleShotTime);
            _isTripleShotActive = false;
        }
    }

    public void SpeedPowerUpActive()
    {
        _isSpeedPowerUpActive = true;
        StartCoroutine(SpeedPowerUpCoolDown());
    }

    IEnumerator SpeedPowerUpCoolDown()
    {
        if (_isSpeedPowerUpActive == true)
        {
            yield return new WaitForSeconds(_activeSpeedPowerUpTime);
            _isSpeedPowerUpActive = false;
        }
    }

    public void ShieldIsActiveRunAnimation()
    {
        _isShieldActive = true;
        _shieldAnimationPrefab.gameObject.SetActive(true); 
    }
}
