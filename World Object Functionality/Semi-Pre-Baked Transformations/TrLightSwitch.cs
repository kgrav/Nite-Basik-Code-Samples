using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TrLightSwitch : Transformation
    {

        public TrLightSwitch()
        {
        }
        public bool Exec(Transform t)
        {
            Light l = t.GetComponent<Light>();
            if (l != null)
            {
                l.enabled = true;
            }
            return true;
        }

        public bool ExecInverse(Transform t)
        {
            Light l = t.GetComponent<Light>();
            if (l != null)
            {
                l.enabled = false;
            }
            return true;
        }

        
    }
