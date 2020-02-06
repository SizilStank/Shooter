using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    private Animator _animator;
    private Player _player;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    public void ShakeTheCam()
    {
        _animator.SetTrigger("ShakeCam");
    }

}
