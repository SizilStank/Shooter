using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackgroundScript : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _newYPos;


    // Update is called once per frame
    void Update()
    {
        ScrollingBackGround();
    }

    private void ScrollingBackGround()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -14.26f)
        {
            transform.position = new Vector3(transform.position.x, _newYPos, 0);
        }
    }
}
