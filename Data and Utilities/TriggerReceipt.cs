using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class TriggerReceipt
    {
        public float when;
        public Vector3 where;
        public Collider who;

        public TriggerReceipt(Collider c)
        {
            where = c.transform.position;

            who = c;
        }
    }
