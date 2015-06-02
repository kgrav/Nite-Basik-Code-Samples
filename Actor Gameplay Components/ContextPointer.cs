using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
    public class ContextPointer : MonoBehaviour
    {
        int Group;
        int Index;

        void Start()
        {
            Invoke("GetContext", 2.0f);
        }

        public int GroupNo()
        {
            return Group;
        }

        public int IndexNo()
        {
            return Index;
        }

        void GetContext()
        {
            Group = ContextBody.GroupOf(transform);
            Index = ContextBody.IndexOf(transform);
        }

    }

