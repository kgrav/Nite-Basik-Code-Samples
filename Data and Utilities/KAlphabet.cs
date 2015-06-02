using UnityEngine;
using System;
using System.Collections.Generic;
 
    struct KAlphabet
    {
        char fall;
        string alpha;

        public KAlphabet(char f)
        { fall = f; alpha = "~abcdefghijklmonpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ~"; }

        public char pull()
        {
            char r = fall;
            if(alpha[0] != alpha[1])
            {
                r = alpha[1];
                string[] t = alpha.Split(r);
                alpha = t[0] + t[1];
            }
            return r;
        }
    }

