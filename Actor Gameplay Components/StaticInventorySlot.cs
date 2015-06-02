using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//i.e.; banjo-kazooie's notes; Zelda's medallions; and so on

    public class StaticInventorySlot
    {
        ItemInfo item;
        public int k;

        int number;
        int max;
        public StaticInventorySlot(ItemInfo i, int m)
        {
            item = i;
            k = item.typeid;
            max = m;
            number = 0;
        }

        public void AddAmount(int i)
        {
            
            number += i;
            if (number > max)
                number = max;
        }

        public void RemoveAmount(int i)
        {
            number -= i;
            if (number < 0)
                number = 0;
        }

        public int GetCount()
        {
            return number;
        }

        public ItemInfo GetItem()
        {
            return item;
        }
    }