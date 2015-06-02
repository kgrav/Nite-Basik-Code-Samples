using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class ItemInfo
    {
        public static ItemInfo GetFromKey(int key)
        {
            switch (key)
            {
                case 0:
                    return new GoldFlower();
                case 1:
                    return new MeatChunk();
                case 2:
                    return new Potion();
                case 3:
                    return new Rock();
            }
            return null;
        }


        public string name;
        public int typeid;
        public int instanceid;
        public bool collectible;
        public bool floats;
        public bool holdable;
        public bool DESTROYonuse;
        public bool DESTROYonpickup;
        public bool heavy;
        public bool INVHeavy;
        public bool staticCollection;


        public virtual GameObject CreateInstance(Vector3 pos, Quaternion rot) { return null; }
        public virtual GameObject CreateParentedInstance(Vector3 relpos, Quaternion rot, Transform par) { return null; }
        public virtual void Equip() { }
        public virtual void Use(Transform onb) { }
        public virtual void Release(Vector3 dir) { }
        public virtual void Throw(Transform instance, Vector3 dir) { }
        public virtual void HitFromThrow(Transform instance, Transform target, Collision hitinfo) { }
        public virtual void HitFromThrow(Transform instance, Transform target) { }
        public virtual void Pickup(Transform t) { }
        public virtual void DropAction() { }
        public virtual void Hold() { }
        public virtual void BubbleIn(Inventory i) { }
    }

