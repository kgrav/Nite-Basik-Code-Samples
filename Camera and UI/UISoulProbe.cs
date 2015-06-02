using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine.UI;
using UnityEngine;

public class UISoulProbe : MonoBehaviour
{

    Slider k;
    Transform whos;
    //Simple functionality; adjusts length of GUI Soul Bar (blue bar, represents Mana) to match player's current
    //statistic.
    void Start()
    {

        k = GetComponent<Slider>();
    }

    void Update()
    {
        if (!whos)
            whos = Liz.StaticLiz.transform;
        float sc = whos.GetComponent<VitalBody>().GetPCon(1);
        k.value = sc;

    }

}