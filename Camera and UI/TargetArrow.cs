using UnityEngine;
using System.Collections;

//Lock-on arrow: provides graphics and meta-camera control (through its
//effect on the Target Helper, and the main camera's transform pseudo-parent)
[AddComponentMenu("Nite-Basik/Camera/Target Arrow")]
public class TargetArrow : MonoBehaviour {

    public static TargetArrow PlayersArrow;
    
    public bool players;
    public Transform owner;
    public Transform t;
    public Transform th;
    public FASTACTIONCAMERA maincam;
    public AudioClip Sound;
    public bool turnedon;
    bool OK = false;

	// Use this for initialization
    void Start()
    {
        turnedon = false;
        maincam = Camera.main.GetComponent<FASTACTIONCAMERA>();
        t = GetComponent<Transform>();
        if (players)
            PlayersArrow = this;
	}
    bool tolf = false;
	// Update is called once per frame
    void Update()
    {
        //If player is targeting an object,
        if (turnedon && Infrared.PlayerTarget != null)
        {
            if (!tolf) //and was not targeting last frame, play target sound from GUI sounds.
            {
                StorySounds.Sound(Sound);
                tolf = true;
            }
            //Set the main camera to follow the player.  This is ideal,
            //as the camera will be focused on the midpoint between the player
            //and the target, which, if the camera is moving with the look probe as is default,
            //may cause the player to be off-screen.
            maincam.movewith = owner;
            th.GetComponent<TargetHelper>().SetActivity(true);
            Vector3 tryharder = Infrared.PlayerTarget.r.position;
            tryharder.y += Infrared.PlayerTarget.ArrowOffset;
            t.position = tryharder;
            GetComponent<SpriteRenderer>().enabled = true;
            t.LookAt(new Vector3(Camera.main.transform.position.x, t.position.y, Camera.main.transform.position.z), Camera.main.transform.up);
            
        }
        else
        {
            //if target isn't on, reset the main camera's motion parent to default (the look probe)
            maincam.movewith = th;
            GetComponent<SpriteRenderer>().enabled = false;
            tolf = false;
            th.GetComponent<TargetHelper>().SetActivity(false);
        }
        if (players)
            PlayersArrow = this;
	}
}
