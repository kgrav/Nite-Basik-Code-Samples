using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
[AddComponentMenu("Nite-Basik/UI/Use Icon")]
//Icon that appears over useable objects when player is in range; has no 
    //active effect, and is considered part of the UI.
    public class WorldInteractButton : MonoBehaviour
    {
        public WorldObject HoverOver;
        bool on;
        SpriteRenderer image;
        void Start()
        {
            image = GetComponent<SpriteRenderer>();
            image.enabled = false;
        }

        public void SetOn(bool c)
        {
            if (on && !c)
            {
                image.enabled = false;
            }
            else if (!on && c)
            {
                image.enabled = true;
            }
            on = c;
        }


        void Update()
        {
            if (on && !HoverOver)
            {
                image.enabled = false;
                return;
            }
            else if(on && HoverOver){
                if (!image.enabled)
                    image.enabled = true;
                transform.position = HoverOver.transform.position + Vector3.up * HoverOver.BOffset;
                transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z), Camera.main.transform.up);
            }
        }

        public void Position(WorldObject par)
        {
            HoverOver = par;
        }
    }

