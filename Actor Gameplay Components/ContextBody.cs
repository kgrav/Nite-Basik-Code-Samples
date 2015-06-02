using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[AddComponentMenu("Nite-Basik/Meta-functionality/Context Body")]
    public class ContextBody : MonoBehaviour
    {
        static int[] SceneIDCounts;
        static List<Transform>[] trances;
        static bool setup;
        public static int WeaponContext = -3;

        //reliable and (relatively) efficient method for checking equivalences in
        //systems with complex parent-child heirarchies.
        //Each animate object has a group number, which represents its type category (Editor-defined)
    //(fig 1-An example type schema:
    //0-LIZ
    //1-HOMUNCULUS
    //2-WILLIAM
    //and an index number, which is a unique (within group) number assigned to the context body at load time.
    //Declare maximum Context index in UMainCam, Editor-side.
    //World Objects will always return -2.  The reasoning behind this is that World Objects
    //are easily functionally differentiated by type, but all of them serve a core group
    //of purposes, therefore, they return a constant number, and based on this information,
    //item type may be differentiated further within the component/header functions.
    
    
        //Any object in a GameObject's heirarchy will return its parent's group # and index #
        //a world object, will always return a context of -2.
    //If a character is searching for a specific world object, that can be determined through the transform.
    //Tabbing World Objects to a constant creates a useful shorthand, meaning
    //scripts can dynamically interact with changing environments (since all solid pieces of the environment are
    //tied to some derivation of the World Object class)

        //If an object in a heirarchy attempts to interact with another object from the same heirarchy,
        //they will return identical group and instance numbers
        //Useful for many things:
        //Quickly and reliably determining the "type" of an object to be interacted with
        //Guide raycasting/physical search algorithms
        //Keep objects from the same heirarchy from affecting each other/the parent (i.e., danger fluids, weapons, avoid algorithms)

        public static bool IsActor(Transform t)
        {
            return (t.root.GetComponent<AICharacter>() || t.root.GetComponent<MAIN_CHARACTER>());
        }
        public static bool Same(int group, int instance, Transform t2)
        {
            ContextPointer f2 = t2.root.GetComponent<ContextPointer>();
            int i1 = group;
            int j1;
            int i2 = instance;
            int j2;
            if (f2)
            {
                j1 = f2.GroupNo();
                j2 = f2.IndexNo();
            }
            else
            {
                j1 = GroupOf(t2);
                j2 = IndexOf(t2);
            }
            if (j1 == -1 || i1 == -1)
                return false;
            if ((j1 == i1) && (j2 == i2))
                return true;
            return false;
        }
    public static bool Same(Transform t, Transform t2)
        {
            ContextPointer f = t.root.GetComponent<ContextPointer>();
            ContextPointer f2 = t2.root.GetComponent<ContextPointer>();
            int i1;
            int j1;
            int i2;
            int j2;
            if (f)
            {
                i1 = f.GroupNo();
                i2 = f.IndexNo();
            }
            else
            {
                i1 = GroupOf(t);
                i2 = IndexOf(t);
            }
            if (f2)
            {
                j1 = f2.GroupNo();
                j2 = f2.IndexNo();
            }
            else
            {
                j1 = GroupOf(t2);
                j2 = IndexOf(t2);
            }
            if (j1 == -1 || i1 == -1)
                return false;
            if ((j1 == i1) && (j2 == i2))
                return true;
            return false;
        }
        public static int GroupOf(Transform t)
        {

            if (!t)
                return -1;
            if (t.GetComponent<WorldObject>())
                return -2;
            else if (t.GetComponent<ContextPointer>())
                return t.GetComponent<ContextPointer>().GroupNo();
            int r = -1;
                ContextBody cx = t.root.GetComponent<ContextBody>();
                if (cx)
                    r = cx.GroupID;
        return r;
        }


        public static int IndexOf(Transform t)
        {
            if (t.GetComponent<WorldObject>() != null)
                return -2;
            else if (t.GetComponent<ContextPointer>())
                return t.GetComponent<ContextPointer>().IndexNo();
            int r = -1;
            ContextBody cx = t.root.GetComponent<ContextBody>();
            if (cx)
                r = cx.LocalID;
            return r;
        }

        public static void SetupScene(int typecount)
        {
            SceneIDCounts = new int[typecount];
            trances = new List<Transform>[typecount];
            for (int i = 0; i < typecount; ++i)
            { SceneIDCounts[i] = 0;
            trances[i] = new List<Transform>();
            }
            setup = true;
        }

        public static Transform FirstInGroup(int i)
        {
            Transform r = null;
            try
            {
                r = trances[i][0];
            }
            catch (Exception e)
            {
                print("CB - FIG EXCEPTION");
            }
            return r;
        }

        public static Transform NthInGroup(int i, int n)
        {
            Transform r = null;
            try
            {
                r = trances[i][n];
            }
            catch (Exception e)
            {
                print("CB - NIG EXCEPTION");
            }
            return r;
        }

        public static int RollCall(int GID, Transform t)
        {
            int i = SceneIDCounts[GID];
            trances[GID].Insert(i, t);
            SceneIDCounts[GID]++;
            return i;
        }


        public int GroupID;
        public int LocalID;
        //masks: context group x reads this body as y
        public Vector2[] masks;
        bool localsetup; 
            bool droogy;
        private ActionContext visibleAction;

        public void ReLoad()
        {
            
        }


        public void PostAction(ActionContext x)
        {
            visibleAction = x;
        }

        public ActionContext ReadAction()
        {
            return visibleAction;
        }

        void DeleteAction()
        {
            visibleAction = null;
        }


        AICharacter buddyCop;
        void Start()
        {
            droogy = false;
            buddyCop = GetComponent<AICharacter>();
            if(buddyCop)
                droogy = true;
            visibleAction = null;
            localsetup = false;
        }
        Droogs xx;
        public void WatchThisForMe(Droogs x)
        {
            xx = x;
        }

        public void OkImBack()
        {
            xx = null;
        }
        public bool Onit = false;
        void Update()
        {
            if (setup&&!localsetup)
            {
                LocalID = RollCall(GroupID, transform);
                localsetup = true;
            }
            if(droogy)
            {
                if (xx)
                {
                    if (xx.IsActive())
                    {
                        xx = null;
                        buddyCop.SetActive(true);
                    }
                }
            }
        }


    }

