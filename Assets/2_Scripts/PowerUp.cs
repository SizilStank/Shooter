using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
    [SerializeField] private int powerupID;//0 = TripleShot, 1 = Speed, 2 = Shields


    // Update is called once per frame
    void Update()
    {
        PowerUpMovement();
    }

    void PowerUpMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -7)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.transform.GetComponent<Player>();

        if (other.gameObject.tag == "Player")
        {
            switch (powerupID)
            {
                case 0:
                    player.TripleShotActive();
                    break;
                case 1:
                    player.SpeedPowerUpActive();
                    break;
                case 2:
                    player.ShieldIsActiveRunAnimation();
                    break;
                default:
                    Debug.Log("Default Value");
                    break;
            }

            Destroy(this.gameObject);
        }
    }
}
