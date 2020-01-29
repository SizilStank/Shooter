using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidScript : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    private Animator _anim;

    [SerializeField] private AudioClip _explosion;

    private AudioSource _audioSource;

    private PolygonCollider2D _polygonCollider2D;

    // Start is called before the first frame update
    void Start()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        if (_polygonCollider2D == null)
        {
            Debug.LogError("Polycollider2D is NULL on Astroid");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on Astroid is NULL");
        }
        else
        {
            _audioSource.clip = _explosion;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 1 * _rotationSpeed * Time.deltaTime);
    }

   private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Laser")
        {
            _audioSource.PlayOneShot(_explosion);

            _polygonCollider2D.enabled = !_polygonCollider2D.enabled;

            SpawnManager spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            spawnManager.StartSpawn();

            _anim.SetTrigger("OnLaserExplode");

            Destroy(other.gameObject);

            Destroy(this.gameObject, 2.7f);
        }
    }

        
}
