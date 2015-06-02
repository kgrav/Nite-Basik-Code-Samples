using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class ULabeledTrigger : IControl
{
    float trig, threshold;
    string name;
    bool hard;
    public char ident;

    public ULabeledTrigger(string na, float tol, char c)
    {
        name = na;
        threshold = tol;
        ident = c;
        trig = 0.0f;
    }

    public void Frame()
    {
        trig = Input.GetAxis(name);
    }

    public bool Down()
    {
        if (threshold < trig)
            return true;
        return false;
    }
}