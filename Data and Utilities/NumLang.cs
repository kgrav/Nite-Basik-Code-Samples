using UnityEngine;
using System;
using System.Collections.Generic;

    public class NumLang
    {
        int[] lang;

        public NumLang()
        {
            lang = new int[CharTable.BASE + 2];
            for (int i = 0; i < CharTable.BASE; ++i)
            {
                lang[i] = -1;
            }
        }

        public void AddSyn(int i, char c)
        {
            lang[CharTable.CLUA(c)] = i;
        }

        public int GetSig(char c)
        {
            if (c == '~')
                return -1;
            return lang[CharTable.CLUA(c)];
        }

    }

