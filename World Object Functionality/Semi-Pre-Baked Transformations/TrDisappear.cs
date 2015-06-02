using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TrDisappear : Transformation
    {
        public TrDisappear()
        {
        }

        public bool Exec(Transform t)
        {
            t.GetComponent<MeshRenderer>().enabled = false;
            t.GetComponent<Collider>().enabled = false;
            return true;
        }

        public bool ExecInverse(Transform t)
        {
            t.GetComponent<MeshRenderer>().enabled = true;
            t.GetComponent<Collider>().enabled = true;
            return true;
        }
    }

