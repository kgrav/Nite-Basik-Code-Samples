using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Nite-Basik/Data/Loading Door")]
public class Door : MonoBehaviour
{
    Collider c;
    Vector3 size;
    float offset;
    Vector3 origin;
    public AudioClip openclip;
    bool isMoving;
    public bool isOpen;
    void Start()
    {
        isOpen = false;
        isMoving = false;

        c = GetComponent<Collider>();
        size = GetComponent<Renderer>().bounds.size;
        offset = size.y;

    }

    public void StartMotion()
    {
        StorySounds.Sound(openclip);
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            float invardist;
            if (!isOpen)
            {
                transform.Translate(Vector3.up * 15 * Time.deltaTime);
                invardist = offset - (transform.position.y - origin.y);
                if (invardist < 0)
                {
                    isMoving = false;
                    isOpen = true;
                }
            }
            else if (isOpen)
            {
                transform.Translate(Vector3.down * 10 * Time.deltaTime);
                invardist = transform.position.y - (origin.y + offset/2);
                if (invardist < 0)
                {
                    isMoving = false;
                    isOpen = false;
                }
            }
        }
    }
}
