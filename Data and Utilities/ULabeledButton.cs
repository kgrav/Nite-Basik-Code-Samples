using UnityEngine;
using System;
using System.Collections.Generic;

public class ULabeledButton : IControl
{

    public string name;
    public char ident;

    public bool held;

    public bool down;
    public bool tap;
    public bool off;

    public bool lockhold;

    public ULabeledButton(string n, char t, bool b)
    {
        held = b;
        down = false;
        tap = false;
        off = true;
        lockhold = false;
        name = n;
        ident = t;
    }

    public void Frame()
    {
        if (held)
        {
            if (Input.GetButton(name))
            {
                down = true;
                off = false;
            }
            else
            {
                down = false;
                off = true;
            }
        }
        else
        {
            down = false;
            if (Input.GetButtonDown(name))
            {
                tap = true;
                off = false;
            }
            else
            {
                tap = false;
                off = true;
            }
        }
    }
}

