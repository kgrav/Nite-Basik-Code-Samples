using UnityEngine;
using System;
using System.Collections.Generic;


using UnityEngine;
    public class ControllerAdmin
    {
        ULabeledButton[] Buttons;
        int i = 0, j = 0, k = 0;
        CharTable ButtonRef;
        string h;
        char r = '~', rh = '~';
        ULabeledStick[] Sticks;
        ULabeledTrigger[] Trigs;
        
        public ControllerAdmin()
        {
            h = "";
            j = 0;
            Sticks  = new ULabeledStick[5];
            Buttons = new ULabeledButton[16];
            Trigs = new ULabeledTrigger[4];
            ButtonRef = new CharTable('Z');
            for (int k = 0; k < Buttons.Length; ++k)
            {
                if (k < 3)
                {
                    Trigs[k] = null;
                    Sticks[k] = null;
                }
                Buttons[k] = null;
            }
        }

        //IN EDITOR: ADD IN ORDER OF PRIORITY;

        

        public void AddButton(string name, char id, bool b)
        {
            if (i == 16)
                return;
            Buttons[i] = new ULabeledButton(name, id, b);
            ButtonRef.Add(id);
            ++i;
            if (b)
                h += id;
        }

        public void AddTrigger(string name, char id, float thresh)
        {
            Trigs[k] = new ULabeledTrigger(name, thresh, id);
        }

        public void Frame()
        {
            r = '~';
            rh = '~';
            string s = ButtonRef.GetSchema();
            for (int i = 0; i < Math.Max(s.Length, h.Length); ++i)
            {

                int bf = ButtonRef.Lookup(s[i]);
                if (i < 3 && Sticks[i] != null)
                    Sticks[i].Frame();
                Buttons[bf].Frame(); 
                if(i < s.Length){
                if (Buttons[bf].tap && !Buttons[bf].held)
                {    r = Buttons[ButtonRef.Lookup(s[i])].ident; break;}
                }
            }
            for (int i = 0; i < h.Length; ++i)
            {
                if (Buttons[ButtonRef.Lookup(h[i])].down)
                {
                    rh = h[i];
                    break;
                }
            }
        }

        public char GetTrigger()
        {
            char r = '~';
            foreach (ULabeledTrigger u in Trigs)
            {
                if (u != null)
                {
                    if (u.Down())
                    {
                        r = u.ident;
                        break;
                    }
                }
            }
            return r;
        }

        public char GetButton()
        {
            return r;
        }

        public char GetExtButton()
        {
            return rh;
        }

        public UStickReceipt[] GetAxes()
        {
            List<UStickReceipt> l = new List<UStickReceipt>();
            for (int i = 0; i < Sticks.Length; ++i)
            {
                if (Sticks[i] != null)
                    l.Add(Sticks[i].GetMotion());
            }
            return l.ToArray();
        }

        public string AddAxis(string hname, string vname, char id, bool a, bool b, float tol)
        {
            Sticks[j] = new ULabeledStick(hname, vname, tol, id, a, b);
            ++j;
            return j + ", " + hname + ", " + vname + ", " + id;
        }
    }

