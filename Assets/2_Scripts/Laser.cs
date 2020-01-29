using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField] private float _speed = 8;


    // Update is called once per frame
    void Update()
    {
        LaserMovement();
    }

    private void LaserMovement()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y >= 8f)
        {
            Destroy(this.gameObject);
        }
        else if (transform.position.y >= 8f && transform.parent.gameObject != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
