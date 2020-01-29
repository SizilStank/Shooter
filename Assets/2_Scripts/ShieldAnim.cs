using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAnim : MonoBehaviour
{

    [SerializeField] private int _shieldLives = 3;

    private Player player;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            
            _shieldLives--;

            if (_shieldLives < 1)
            {
                
                gameObject.SetActive(false);
            }
        }
    }
}
