using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Visual feedback for damage; calls from the Mortality script when damage is received.
//Including a NumberCard/NumberPanel construct as a child object to a gameobject which
//utilizes Mortality will cause numeric feedback to appear automatically;
//no extra set-up is needed, as Mortality automatically senses whether or not
//a NumberCard/NumberPanel construct exists within the gameobject it describes.
[RequireComponent(typeof(SpriteRenderer))]
    public class NumberPanel : MonoBehaviour
    {
        public static float numbertime = 1.3f;
        bool visible;
        public int digit;
        SpriteRenderer sp;
        Vector3 origin;
        void Start()
        {
            origin = transform.localPosition;
            visible = false;
            sp = GetComponent<SpriteRenderer>();
            sp.enabled = false;
        }
        void Update()
        {
            if (visible)
            {
                transform.localPosition += Vector3.up * Time.deltaTime;
                
            }
        }
    //Numbers are loaded to the GUICharLoader number table,
    //they are indexed transparently (indexes 0-9 of the array correspond to the numbers 0-9)
    //In this case, number sprites are used for their versatility and for aesthetic reasons.
    //
    //Upon activation, each Number Panel that is part of the current number (for ex. '8' would only have one active panel,
    //while '85' would have two, and '856' would have three, and so on until we've reached the max range) will display
    //the sprite of the integer it represents.
        public void SetDigit(int i)
        {
            transform.localPosition = origin;

            if (i != -1)
            {
                print("Digit " + digit + " got On signal");
                if (visible)
                    CancelInvoke("TurnOff");
                sp.sprite = GUICharLoader.numbers[i];
                sp.enabled = true;
                visible = true;
            }
            else
            {
                print("Digit " + digit + " got off signal");
                sp.enabled = false;
            }
            Invoke("TurnOff", numbertime);
        }
        public void TurnOff()
        {
            sp.enabled = false;
            visible = false;
        }
    }

