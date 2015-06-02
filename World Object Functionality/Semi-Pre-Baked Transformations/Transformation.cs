using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public interface Transformation
    {
        bool Exec(Transform t);
        bool ExecInverse(Transform t);

    }
