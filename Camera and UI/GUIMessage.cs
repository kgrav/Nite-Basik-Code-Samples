using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class GUIMessage : MonoBehaviour
    {
        public float textspeed;
        string currmessage;
        string runningmessage;
        int mp;
        float tp, spd;

        bool show;

        void Start()
        {
            mp = 0;
            tp = 0;
            spd = textspeed;
            currmessage = "";
            runningmessage = "";
        }

        void Update()
        {
            if (!runningmessage.Equals(currmessage))
            {
                tp += Time.deltaTime;
                if (tp > spd)
                {

                    tp -= spd;
                    runningmessage += currmessage[mp];
                    mp++;
                    //SHOW THE TEXT
                    //PLAY THE SOUND
                }
            }
        }

        public void SetTextAndShow(string s)
        {
            runningmessage = "";
            currmessage = s;
            tp = 0;
            mp = 0;
            spd = textspeed;
        }

        public void FFWD()
        {
            spd = 0;
            
        }

        public void Close()
        {
        }
    }
