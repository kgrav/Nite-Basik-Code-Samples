using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

[AddComponentMenu("Nite-Basik/Action Coordination/MAIN ONLY/Inventory")]
    public class Inventory : MonoBehaviour
    {
        static int[] itemkeys;
        public static ItemInfo[] itemptrs;
        static int glbptr;

        static List<ItemInfo> statics;

        static bool init = false;
        public static void ItemRollCall(int us, ItemInfo r)
        {
            Init();
            if (r.staticCollection)
                statics.Add(r);
            itemptrs[glbptr] = r;
            itemkeys[us] = glbptr;
            
        }

        public bool off;

        static void Init()
        {
            if(!init)
            {
                glbptr = 0;
                statics = new List<ItemInfo>();
            itemkeys = new int[100];
            itemptrs = new ItemInfo[100];
                init = true;
            }
        }

        public int WeightCap;
        public int StackCap;
        public int CountCap;
        public int GrabSnd;
        public bool sound;
        public int[] SIKeys;
        StaticInventorySlot[] PermInv;
        InventorySlot[] slots;
        InventorySlot hand;
        int[] stlu;
        int invp, sipt;
        void Start()
        {
            slots = new InventorySlot[CountCap];
            stlu = new int[100];
            for (int i = 0; i < CountCap; ++i)
            {
                slots[i] = new InventorySlot(StackCap);
            }

            invp = 0; 
            for (int i = 0; i < 100; ++i)
            {
                stlu[i] = -1;
            }
        }

        void Update()
        {
            if (statics != null)
            {
                if (firstframe && !off && statics.Count >= SIKeys.Length)
                {
                    PermInv = new StaticInventorySlot[SIKeys.Length];

                    for (int i = 0; i < SIKeys.Length; ++i)
                    {
                        for (int j = 0; j < statics.Count; ++j)
                        {
                            if (statics[j].typeid == SIKeys[i])
                                PermInv[i] = new StaticInventorySlot(statics[j], 120);
                        }
                    }
                    firstframe = false;
                    print("Static Inventory Init");
                }
            }
        }

        bool firstframe = true;


        public bool Pickup(Item r)
        {
            bool re = false;
            if (r.IR.staticCollection)
            {
                for (int i = 0; i < PermInv.Length; ++i)
                {
                    if (r.IR.typeid == PermInv[i].k)
                    {
                        PermInv[i].AddAmount(1);
                        re = true;
                        break;
                    }
                }
            }
            else
            {
                bool scndpass = false;
                int lastfree = -1;
                for (int i = 0; i < slots.Length; ++i)
                {
                    if (slots[i].Free() && lastfree == -1)
                    {
                        lastfree = i;
                        continue;
                    }
                    else if(r.IR.typeid == slots[i].itemref)
                    {
                        int q = slots[i].AddAmount(1, r.IR);
                        if (q == 0)
                        {
                            re = true;

                        }
                        else
                        {
                            scndpass = true;
                        }
                    }
                }
                if (scndpass && lastfree != -1)
                {
                    slots[lastfree].AddAmount(1, r.IR);
                    re = true;
                }
            }
            print("Liz picked up a " + r);
            return re;
        }
    }

