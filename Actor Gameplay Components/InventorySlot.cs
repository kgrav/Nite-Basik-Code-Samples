using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

    public class InventorySlot
    {
        public int itemref;
        bool freeze;
        int stackmax;
        int stack;

        public InventorySlot(int stackm)
        {
            itemref = -1;
            stackmax = stackm;
            stack = 0;
            freeze = false;
        }

        public bool Free()
        {
            return itemref == -1;
        }

        public void Discard(int amnt)
        {
            if(freeze)
                return;
            if (stack > 0)
            {
                stack -= amnt;
                Inventory.itemptrs[itemref].DropAction();
            }
            if (stack < 0)
            {
                stack = 0;
                itemref = -1;
            }
        }

        public bool Use(int amnt, Transform t)
        {
            if (itemref == -1)
            {
                return false;
            }
            else if (stack - amnt <= 0)
            {
                return false;
            }

            for (int i = 0; i < amnt; ++i)
            {
                
                Inventory.itemptrs[itemref].Use(t);
                if (Inventory.itemptrs[itemref].DESTROYonuse)
                    stack--;  
            }
            if(stack <= 0)
            {
                stack = 0;
                itemref = -1;
            }
            return true;
        }

        public int GetForHold()
        {if(freeze)
            return -1;
            if(stack <= 0)
                return -1;
            else
                stack--;

            freeze = true;
            return itemref;
        }

        public void DropFromHold()
        {
            if(stack <= 0){
                stack = 0;
                itemref = -1;
                freeze = false;
            }
        }

        public int AddFromHold(int i)
        {
            stack++;
            freeze = false;
            return stack;
        }

        public int AddAmount(int i, ItemInfo k)
        {
            if(itemref == k.typeid)
            {
                int j= i;
                for(int u = stack; u < stackmax && j >=0; ++u)
                {
                    stack++;
                    j--;
                }
                return j;
            }
            else if(itemref == -1)
            {
                itemref = k.typeid;
                stack = i;
                if(stack > stackmax){
                    stack = stackmax;
                return stackmax - i;}
                return i;
            }
            return -1;

        }

        public void DropAll()
        {
            itemref = -1;
            stack = 0;
        }
        public int GetAmnt()
        {
            return stack;
        }
    }

