using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleLaserPrefab;
    [SerializeField] private GameObject _shieldAnimationPrefab;
    [SerializeField] private GameObject _ringsAnimationPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _healthPowerUpPrefab;
    [SerializeField] private GameObject _ringsActiveIcon1, _ringsActiveIcon2, _ringsActiveIcon3;
    [SerializeField] private GameObject[] _setRandomPlayerWingDamage;
    [SerializeField] private GameObject _speedThrusters;

    [SerializeField] private AudioClip _laserShoot;
    [SerializeField] private AudioClip _explosion;
    [SerializeField] private AudioClip _powerUpClip;
    [SerializeField] private AudioClip _shieldTakeDamage;
    [SerializeField] private AudioClip _playerHit;
    [SerializeField] private AudioClip _ringsSoundClip;
    [SerializeField] private AudioClip _OutOfLasers;
    [SerializeField] private AudioClip _superThruseterClip;

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _speedPowerUpSpeed = 10f;
    [SerializeField] private float _volumeMax = 1f;
    [SerializeField] private float _volumeMin = 0.25f;
    [SerializeField] private float _volumeNULL = 0f;
    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private float _canFire = -1f;
    [SerializeField] private float _activeTripleShotTime = 5f;
    [SerializeField] private float _activeSpeedPowerUpTime = 5f;
    [SerializeField] private float _activeShieldPowerUpTime = 5;

    [SerializeField] private float _timeLeft = 4f;
    [SerializeField] private float _coolDownTimer;

    [SerializeField] private int _lives = 3;
    [SerializeField] private int _shieldLives = 3;
    [SerializeField] private int _score;
    [SerializeField] private int _laserAmmo = 15;
    [SerializeField] private int _ringsIconCounter;
    [SerializeField] private int _currentIndex = 0;

    [SerializeField] private bool _hasAmmo = true;
    [SerializeField] private bool _isTripleShotActive;
    [SerializeField] private bool _isSpeedPowerUpActive;
    [SerializeField] private bool _isShieldActive;
    [SerializeField] private bool _isRingActive;
    [SerializeField] private bool _timerActive;

    private SpawnManager _spawnManager;//handle to component... then FIND it
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private ShieldAnim _shieldAnim;
    private Canvas _canvas;
    [SerializeField] private Slider _sliderLaserAmmoCount;
    [SerializeField] private Slider _thusterSlider;
    private Camera _camera;

    private void Start()
    {    
        _sliderLaserAmmoCount.value = _laserAmmo;
        _thusterSlider.value = _timeLeft;
        PlayerComponets();       
    }


    // Update is called once per frame
    void Update()
    {        
        PlayerMovement();
        PlayerInput();

        if (_timeLeft < 0)
        {
            _timeLeft = 0;
            _speedThrusters.SetActive(false);
            StartCoroutine(RestartTimeLeft());
            Debug.Log("Counting " + Time.deltaTime);
        }
    }

    private void PlayerComponets() //Called at Start
    {
        _ringsAnimationPrefab.SetActive(false);
        _ringsActiveIcon1.SetActive(false);
        _ringsActiveIcon2.SetActive(false);
        _ringsActiveIcon3.SetActive(false);

        _score = Mathf.Clamp(_score, 0, 9999999);
        _laserAmmo = Mathf.Clamp(_laserAmmo, 0, 100);     

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

        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("Camera is NULL");
        }

        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("Canvas is NULL");
        }
    }

    private void PlayerMovement()
    {


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedPowerUpActive == true)
        {
            transform.Translate(moveDirection * _speedPowerUpSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftShift)  && _timeLeft > 0)
        {
            _thusterSlider.value -= 1 * Time.deltaTime;
            _timeLeft -= Time.deltaTime;
            _speedThrusters.SetActive(true);                            
            transform.Translate(moveDirection * _speed * 3 * Time.deltaTime);
        }
        else
        {
            transform.Translate(moveDirection * _speed * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && _timeLeft > 0)
        {
            _speedThrusters.SetActive(false);
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

    private IEnumerator RestartTimeLeft()
    {
        yield return new WaitForSeconds(10f);
        _thusterSlider.value = 4f;
        _timeLeft = 4f;
    }

    private void PlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > _canFire && _hasAmmo == true)
        {
            _sliderLaserAmmoCount.value -= 1;
            _audioSource.PlayOneShot(_laserShoot, _volumeMin);
            FireLaser();
        }

        if (Input.GetMouseButtonDown(1) && _isRingActive == true)
        {
            _ringsIconCounter--;

            if (_ringsIconCounter == 0)
            {
                _isRingActive = false;
            }

            _audioSource.PlayOneShot(_ringsSoundClip, _volumeMin);
            _ringsAnimationPrefab.SetActive(true);
            StartCoroutine(RingsActiveWaitForSeconds());  // _isRingsActive == flase;

            switch (_ringsIconCounter)
            {
                case 0:
                    _ringsActiveIcon1.SetActive(false);
                    break;
                case 1:
                    _ringsActiveIcon2.SetActive(false);
                    break;
                case 2:
                    _ringsActiveIcon3.SetActive(false);
                    break;
                default:
                    Debug.Log("Its okay");
                    break;
            }
        }
    }

    private void FireLaser()
    {
        _laserAmmo--;

        if (_laserAmmo > 1)
        {
            _hasAmmo = true;
        }
        else if (_laserAmmo == 0)
        {
            _audioSource.PlayOneShot(_OutOfLasers, _volumeMin);
            _hasAmmo = false;
        }

        if (_hasAmmo == false)
        {
            _audioSource.PlayOneShot(_laserShoot, _volumeNULL);
            _laserAmmo = 0;
            return;
        }

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
        _lives = Mathf.Clamp(_lives, 0, 3);
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

    IEnumerator RingsActiveWaitForSeconds()
    {       
        yield return new WaitForSeconds(0.3f);
        _ringsAnimationPrefab.SetActive(false);
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

        if (other.gameObject.tag == "RingsPowerUpCollected")
        {
            _audioSource.PlayOneShot(_powerUpClip, _volumeMin);

            _ringsIconCounter++;

            _ringsIconCounter = Mathf.Clamp(_ringsIconCounter, 0, 3);

            switch (_ringsIconCounter)
            {
                case 1:
                    _ringsActiveIcon1.SetActive(true);
                    break;
                case 2:
                    _ringsActiveIcon2.SetActive(true);
                    break;
                case 3:
                    _ringsActiveIcon3.SetActive(true);
                    break;
                default:
                    Debug.Log("Its okay");
                    break;
            }
        }

        if (other.gameObject.tag == "LaserAmmo")
        {
            _audioSource.PlayOneShot(_powerUpClip, _volumeMin);
            _sliderLaserAmmoCount.value += 15;
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
        _speedThrusters.SetActive(true);
        _audioSource.PlayOneShot(_superThruseterClip, _volumeMin);
        _isSpeedPowerUpActive = true;
        StartCoroutine(SpeedPowerUpCoolDown());
    }

    IEnumerator SpeedPowerUpCoolDown()
    {
        if (_isSpeedPowerUpActive == true)
        {
            yield return new WaitForSeconds(_activeSpeedPowerUpTime);
            _speedThrusters.SetActive(false);
            _isSpeedPowerUpActive = false;
        }
    }

    public void ShieldIsActiveRunAnimation()
    {
        _isShieldActive = true;
        _shieldAnimationPrefab.gameObject.SetActive(true); 
    }

    public void AmmoPowerUpGiveAmmo()
    {
        _hasAmmo = true;
        _laserAmmo += 15;
    }

    public void ActiveRingPowerUp()
    {
        _isRingActive = true;
    }

    public void HealthPowerUp()
    {
        _lives++;
        _setRandomPlayerWingDamage[0].SetActive(false);
        _setRandomPlayerWingDamage[1].SetActive(false);
        _uiManager.UpdateLives(_lives);
    }
}
