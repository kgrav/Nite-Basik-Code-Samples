using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Nite-Basik/UI/Char Sprite Loader")]
    public class GUICharLoader : MonoBehaviour
    {
        public static Sprite[] numbers;
        public static Sprite[] letters;
        public static Sprite[] symbols;
        public static Sprite[] icons;
        static bool init = false;
        public Sprite[] numberray;
        void Start()
        {
            if (!init)
            {
                numbers = new Sprite[10];
                for (int i = 0; i < 10; ++i)
                {
                    numbers[i] = numberray[i];
                }
            }
        }
    }

