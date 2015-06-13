using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
//Camera Script for fast-paced 3rd person action games.
//Make it follow and look at the Look Probe in the scene,
//this is smoother than having the camera follow the player
//directly, as the look probe smoothly aligns itself with the player,
//and the player makes fast movements that would cause the camera to jerk around otherwise.
[AddComponentMenu("Nite-Basik/Camera/Action Game Camera")]
public class FASTACTIONCAMERA : MonoBehaviour
{
    public Transform movewith; //The camera will move relative to this object
    public  Transform lookprobe; //The camera will always look at this object.

    //In the examples, I have a supplementary object that follows the location of the player character,
    //but moves more smoothly, as the player character in the examples (Liz), makes a lot of small,
    //quick motions that would cause undesirable effects on the camera otherwise.
    public float[] DistanceToggles;
    int DistanceCurr = 0;
    public float MoveSpeed;
    public float TurnSpeed;
    public float PitchSpeed, minPitch, maxPitch;
    public float idealheight;
    public float speedlimit;
    public float distlimit;
    public bool protecting;

    float OriginDist;
    float CurrDist;
    float Velocity = 30.0f;
    float ClipMoveTime = 0.01f;
    float SCRad = 0.1f;
    float lookAngle;
    float pitchAngle;
    public int GroupsInScene;
    int mode = 0;
    Vector3 pos, targetpos, lizpos;
    Vector3 CameraForward;
    bool targeting, hardtargeting;
    RaycastHit[] rs;
    public void SetMode(int m)
    {
        mode = m;
    }
    //Static method for printing messages to console
    //call this from classes which do not derive from MonoBehaviour
    public static void DBM(string s, string from)
    {
        print(from + ": " + s);
    }

    void Start()
    {

        transform.LookAt(movewith);
        Vector3 eui = transform.eulerAngles;
        lookAngle = eui.y;
        ContextBody.SetupScene(GroupsInScene);
        pitchAngle = eui.x;
        CurrDist = (movewith.position - transform.position).magnitude;
    }
    int rlf;
    void Update()
    {
        //BEGIN POSITION FUNCTIONALITY
        Vector3 origin = transform.position;
        CameraForward = (movewith.position - transform.position).normalized;
        Vector3 PlaneForward = new Vector3(CameraForward.x, 0, CameraForward.y);
        Vector3 LizForward = movewith.forward;
        Vector3 Up = Vector3.up;
        Rigidbody LizRB = movewith.GetComponent<Rigidbody>();
        OriginDist = (movewith.position - transform.position).magnitude;
        float x = Input.GetAxis("UR");
        float y = Input.GetAxis("VR");
        bool b = Input.GetButtonUp("RS");
        bool a = Input.GetButtonUp("LS"); //store needed variables
        if (b)//Set desired distance toggle.
        {
            DistanceCurr++;
            if (DistanceCurr == DistanceToggles.Length)
                DistanceCurr = 0;
        }
        bool c = false;
        //Check for collisions within the immediate vicintity of the Camera
        Collider[] cs = Physics.OverlapSphere(origin, SCRad);
        for (int i = 0; i < cs.Length; ++i)
        {
            if (!(cs[i].transform.name.Equals(transform.name)))
            {
                if (ContextBody.GroupOf(cs[i].transform) == -2)
                {

                    c = true;
                    break;
                }
            }
        }
        float closest = Mathf.Infinity;
        float target = OriginDist;
        //If we've already found a collision, do a smaller check, otherwise a larger one.
        if (c)
        {
            origin += CameraForward * SCRad;
            rs = Physics.RaycastAll(new Ray(origin, CameraForward), OriginDist - SCRad);
        }
        else
        {
            rs = Physics.SphereCastAll(new Ray(origin, CameraForward), SCRad, OriginDist - SCRad);

        }
        bool d = c;
        //Find the closest point of collision between Camera and currently assigned Look Probe
        float ldist = Mathf.Infinity;
        for (int i = 0; i < rs.Length; ++i)
        {
            if (rs[i].distance < closest && ContextBody.GroupOf(rs[i].transform) == -2)
            {
                closest = rs[i].distance;

                target = -movewith.InverseTransformPoint(rs[i].point).z - SCRad;
                d = true;
            }
            else if (rs[i].transform.name.Equals(movewith.name))
            {
                ldist = rs[i].distance;
            }
        }
        RaycastHit revcheck;
        //If we're not avoiding a collision, 
        //Check behind the camera to make sure there is not an object
        //between the camera's current position, and the desired 
        //Distance.
        //If there is, move the camera to jsut in front of the point of collision,
        //otherwise move the camera to the desired distance.
        float dltf = distlimit;
        float dmtf = DistanceToggles[DistanceCurr];
        float ttf = target;
        float wallz; if (!d)
        {
            if (Physics.Raycast(new Ray(transform.position, -CameraForward), out revcheck, DistanceToggles[DistanceCurr] - CurrDist))
            {
                ttf = revcheck.distance - SCRad + 0.1f;
            }
            else
            {
                ttf = DistanceToggles[DistanceCurr];
            }
        }
        else
        {
            if (closest < dltf)
                dltf = closest;
        }
        lookAngle += x * TurnSpeed;
        pitchAngle -= y * PitchSpeed;
        pitchAngle = Mathf.Clamp(pitchAngle, minPitch, maxPitch);
        CurrDist = Mathf.SmoothDamp(CurrDist, ttf, ref Velocity, CurrDist > ttf ? ClipMoveTime : Time.deltaTime);
        CurrDist = Mathf.Clamp(CurrDist, dltf, dmtf);
        
        Vector3 pos = Quaternion.Euler(new Vector3(pitchAngle, lookAngle, 0)) * (new Vector3(0, 0, 1) * CurrDist);
        transform.position = movewith.position - pos;

        transform.LookAt(lookprobe);
    }

    public Vector3 GetAim()
    {
        return transform.forward;
    }

}
