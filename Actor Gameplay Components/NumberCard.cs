using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Visual feedback for damage; calls from the Mortality script when damage is received.
//Including a NumberCard/NumberPanel construct as a child object to a gameobject which
//utilizes Mortality will cause numeric feedback to appear automatically;
//no extra set-up is needed, as Mortality automatically senses whether or not
//a NumberCard/NumberPanel construct exists within the gameobject it describes.
//Additional, non-mortality-based calls can be added to other classes,
//just call the TurnOn method from the Number Card reference you want to show.
//
//The count of Number Panels in children represents the maximum range of this numeric visualization.
//For example, four number cards would have a range of 0-9999.  Numbers > max will show a 9 in each digit.
    public class NumberCard : MonoBehaviour
    {
        NumberPanel[] numbers;
        int digits;
        void Start()
        {
            NumberPanel[] ns = GetComponentsInChildren<NumberPanel>();
            numbers = new NumberPanel[ns.Length];
            for (int i = 0; i < ns.Length; ++i)
            {
                numbers[ns[i].digit] = ns[i];
            }
            digits = numbers.Length;
        }
        void Update()
        {
            //align transform with camera always.  This has no side effects, as rendering is turned off when object is not needed,
            //and object does not require any sort of collision detection.
            transform.LookAt(Camera.main.transform, Camera.main.transform.up);
        }
        public void TurnOn(float number)
        {
            transform.LookAt(Camera.main.transform, Camera.main.transform.up);
            int num = (int)number;
            int[] send = new int[digits];//if number is larger than # of digits allows, display '999....'
            if (number >= Math.Pow(10, digits))
            {
                for (int i = 0; i < digits; ++i)
                {
                    numbers[i].SetDigit(9);
                }
                return;
            }
            //Otherwise, display number, and turn off digits that are not to be used (signified in numeric string by an 'X' char
            string word = "";
            string n = num + "";
            if (n.Length < digits)
            {
                for (int i = 0; i < digits - n.Length; ++i)
                {
                    word += "X";
                }
            }
            word += n;
            print(num + ", " + n);
            bool atplace = false;
            for (int i = 0; i < digits; ++i)
            {//If we're on an X, the number hasn't started yet.
                if (word[i] == 'X')
                {
                    numbers[i].SetDigit(-1);
                    print("setting digit " + i + "to inactive");
                }
                else if (!atplace) //otherwise it has, begin displaying the number
                {
                    atplace = true;
                }
                if (atplace)
                {
                    print(i);
                    string numchar = word[i]+"";
                    int numout = Convert.ToInt32(numchar);
                    numbers[i].SetDigit(numout);
                }
                
            }
        }
    }

