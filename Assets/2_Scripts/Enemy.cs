using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private GameObject _child;
    [SerializeField] private AudioClip _explosion;

    [SerializeField] private GameObject _enemyLaserPrefab;

    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private float _canFire = -1f;

    [SerializeField] private bool _canShoot = true;

    private Player _player;
    private PolygonCollider2D _polycollider2D;
    private Rigidbody2D _rigidbody2D;
    private Animator _anim;
    private AudioSource _audioSource;
    private Camera _camera;


    private void Start()
    {
        GetComponents();
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>(); //!!!!!!!!!Need to null check!!!!!!!!!
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
        EnemyShootLaser();
    }

    private void GetComponents()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("_player is NULL");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("_anim is NULL");
        }

        _polycollider2D = GetComponent<PolygonCollider2D>();
        if (_polycollider2D == null)
        {
            Debug.LogError("PolygonCollider2D is Null");
        }

        _rigidbody2D = GetComponent<Rigidbody2D>();
        if (_rigidbody2D == null)
        {
            Debug.LogError("Rigidbody2D is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on enemy is NULL");
        }
        else
        {
            _audioSource.clip = _explosion;
        }
    }

    void EnemyMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -5.40f)
        {
            float randomX = Random.Range(-9.21f, 9.21f);
            transform.position = new Vector3(randomX, 8, 0);
        }       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _polycollider2D.enabled = !_polycollider2D.enabled;
            _rigidbody2D.isKinematic = true;

            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                _camera.ShakeTheCam(); //Playes the Camera Shake Anim
                player.Damage();
                _player.AddScore(5);
            }

            _anim.SetTrigger("OnEnemyDeath");

            _speed = .5f;

            _canShoot = false;

            _audioSource.PlayOneShot(_explosion);

            Destroy(this.gameObject, 2f);
            Destroy(_child);

        }

        if (other.gameObject.tag == "Laser")
        {
            _rigidbody2D.isKinematic = true;
            _polycollider2D.enabled = !_polycollider2D.enabled;

            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }

            _anim.SetTrigger("OnEnemyDeath");

            _speed = .5f;

            _canShoot = false;

            _audioSource.PlayOneShot(_explosion);

            Destroy(this.gameObject, 2f);
            Destroy(_child);
        }

        if (other.gameObject.tag == "Rings")
        {
            _rigidbody2D.isKinematic = true;
            _polycollider2D.enabled = !_polycollider2D.enabled;

            if (_player != null)
            {
                _player.AddScore(10);
            }

            _anim.SetTrigger("OnEnemyDeath");

            _speed = .5f;

            _canShoot = false;

            _audioSource.PlayOneShot(_explosion);

            Destroy(this.gameObject, 2f);
            Destroy(_child);
        }
    }

    private void EnemyShootLaser()
    {
        if (Time.time > _canFire && _canShoot == true)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            Instantiate(_enemyLaserPrefab, transform.position + new Vector3(-0.12f, -1.37f, 0), Quaternion.identity);
        }      
    }

}
