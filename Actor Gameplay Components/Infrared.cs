using UnityEngine;
using System.Collections;
[AddComponentMenu("Lady Stardust/Camera/Targetable Object")]
public class Infrared : MonoBehaviour {

    public static Infrared PlayerTarget = null;

    public Transform r;
    public float drtrad;
    public float ArrowOffset;
    public Infrared next;
    Infrared rTarget;
    Infrared lTarget;
    int drtstate;
    Collider[] buff1, buff2;
    Vector3 right;
	// Use this for initialization
	void Start () {
        drtstate = 0;
        r = GetComponent<Transform>();
        rptr = 0;
        lptr = 0;
        mindl = float.PositiveInfinity;
        mindr = float.PositiveInfinity;
        right = r.TransformDirection(Vector3.right);
        buff1 = Physics.OverlapSphere(r.position + drtrad * right, drtrad);
        buff2 = Physics.OverlapSphere(r.position - drtrad * right, drtrad);
        int m = System.Math.Max(buff1.Length, buff2.Length);
        for (int i = 0; i < m; ++i)
        {
            if (i < buff1.Length)
            {
                Infrared rtemp = buff1[rptr].GetComponent<Infrared>();
                float dtemp = GetDistance(buff1[rptr].transform);
                if (rtemp != null)
                {
                    if (rTarget == null)
                    {
                        rTarget = rtemp;
                        mindl = GetDistance(buff1[rptr].transform);

                    }
                    else if (GetDistance(buff1[rptr].transform) < mindr)
                    {
                        rTarget = rtemp;
                        mindl = GetDistance(buff1[rptr].transform);
                    }
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Infrared.PlayerTarget)
        {
            if (Infrared.PlayerTarget.transform.name.Equals(transform.name))
            {
                Frame();
            }
        }
    }
    int rptr, lptr;
    float mindl, mindr;
    void Frame()
    {/*
        switch (drtstate)
        {
            case 0:
                if (buff2.Length>0 && buff2[lptr])
                {
                    Infrared ltemp = buff2[lptr].GetComponent<Infrared>();
                    float dtemp = GetDistance(buff2[lptr].transform);
                    if (ltemp != null)
                    {
                        if (lTarget == null)
                        {
                            lTarget = ltemp;
                            mindl = GetDistance(buff2[lptr].transform);

                        }
                        else if (GetDistance(buff2[lptr].transform) < mindl)
                        {
                            lTarget = ltemp;
                            mindl = GetDistance(buff2[lptr].transform);
                        }
                    }
                    lptr++;
                    if (lptr == buff2.Length)
                        drtstate = 1;
                }
                break;
            case 1:
                if (buff2.Length>0&&buff1[rptr])
                {
                    Infrared rtemp = buff1[rptr].GetComponent<Infrared>();
                    float dtemp = GetDistance(buff1[rptr].transform);
                    if (rtemp != null)
                    {
                        if (rTarget == null)
                        {
                            rTarget = rtemp;
                            mindl = GetDistance(buff1[rptr].transform);

                        }
                        else if (GetDistance(buff1[rptr].transform) < mindr)
                        {
                            rTarget = rtemp;
                            mindl = GetDistance(buff1[rptr].transform);
                        }
                    }
                    rptr++;
                    if (rptr == buff1.Length)
                        drtstate = 1;
                }
                break;
            case 2:
                Refresh();
                break;

        }*/
    }

    public void ReTarget(float dir)
    {

        if (dir < 0)
        {
            if (lTarget != null)
                PlayerTarget = lTarget;
        }
        else if (dir > 0)
        {
            if (rTarget != null)
                PlayerTarget = rTarget;
        }
    }

    void Refresh()
    {
        right = r.TransformDirection(Vector3.right);
        buff1 = Physics.OverlapSphere(r.position + drtrad * right, drtrad);
        buff2 = Physics.OverlapSphere(r.position - drtrad * right, drtrad);
        drtstate = 0;
        if (rTarget == null)
        {
            mindr = float.PositiveInfinity;
        }
        if (lTarget == null)
        {
            mindl = float.PositiveInfinity;
        }
        rptr = 0;
        lptr = 0;
    }

    public float GetDistance(Transform t)
    {
        return Vector3.Distance(r.position, t.position);
    }
}
