using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserScript : MonoBehaviour
{

    [SerializeField] private float _speed = 3f;
    private Camera _camera;


    private void Start()
    {
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>(); //!!!!!!!!!Need to null check!!!!!!!!!
    }


    // Update is called once per frame
    void Update()
    {
        EnemyLaserMovment();
    }

    private void EnemyLaserMovment()
    {
        //need the laser to move down
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -6.0f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _camera.ShakeTheCam();
            Destroy(this.gameObject);

            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
        }

        if (other.gameObject.tag == "Rings")
        {
            Destroy(this.gameObject);
        }
    }
}
