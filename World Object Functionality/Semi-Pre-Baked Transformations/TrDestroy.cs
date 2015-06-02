using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TrDestroy : Transformation
    {
        GameObject gmem;
        Vector3 pmem;
        Quaternion rmem;
        public TrDestroy(Transform t)
        {
            gmem = t.gameObject;
            pmem = t.position;
            rmem = t.rotation;
        }

        public bool Exec(Transform t)
        {
            GameObject.Destroy(t.gameObject);
            return true;
        }

        public bool ExecInverse(Transform t)
        {
            GameObject.Instantiate(gmem, pmem, rmem);
            return true;
        }
    }

