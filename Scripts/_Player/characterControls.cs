using UnityEngine;
using System.Collections;

public class characterControls : MonoBehaviour
{

    public float speed = 10.0f;
    public float crouchMultiplier = -2f;
    public float sprintMultiplier = 1.5f;
    [HideInInspector]
    public float curSpeed = 0;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public float jumpHeight = 2.0f;
    private bool grounded = false;
    public GameObject fpsCam;
    public SkinnedMeshRenderer mesh;
    public MeshRenderer[] accessories;
    public float crouchHeight = 1;
    public CapsuleCollider body;
    public AudioClip promotion;
    public AudioClip gotKillSound;
    public AudioClip gotHeadShotSFX;

    public GameObject[] wepDrops;

    [HideInInspector]
    public rankItem curRank;

    public Collider head;

    public GameObject ragdoll;

    public PhotonView pv;

    Texture rankIcn;

    public int health = 100;

    string killer = "World";

    float bloodTime = 0;

    float fragTime = 0;

    float hsTime = 0;

    public AudioClip[] killStreaksInOrder;

    Texture bloodyScreen;

    public string instructions = "Pause - Tab \nF - Knife \nT - Chat";

    public Texture[] bloodyScreens;

    [HideInInspector]
    public float normalHeight = 2;

    public int xpRewardLow = 5;
    public int xpRewardHigh = 15;

    bool proned = false;

    int kills = 0;

    
    bool paused = false;

    [HideInInspector]
    public Texture pfp;

    void Awake()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        GetComponent<Rigidbody>().useGravity = false;
        curSpeed = speed;
        normalHeight = body.height;
        checkPromo();

    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().freezeRotation = true;
        if (grounded)
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity1 = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            float inputModifyFactor = (targetVelocity1.x != 0.0f && targetVelocity1.z != 0.0f) ? .7071f : 1.0f;
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal") * inputModifyFactor, 0, Input.GetAxis("Vertical") * inputModifyFactor);
            targetVelocity *= curSpeed;
            targetVelocity = transform.TransformDirection(targetVelocity);
            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = GetComponent<Rigidbody>().velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);          
            
            // Jump
            if (canJump && Input.GetButtonDown("Jump"))
            {
                GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }

            if (Input.GetKey(KeyCode.C))
            {
                body.height = crouchHeight;
            }
            else
            {
                if (body.height != normalHeight)
                {
                    body.height = normalHeight;
                }
            }




            if (!proned)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    curSpeed = speed * sprintMultiplier;
                }
                else {
                    curSpeed = speed;
                }
            }
        }

        if (bloodTime > 0)
        {
            bloodTime += -Time.fixedDeltaTime;
        }

        if (fragTime > 0)
        {
            fragTime += -Time.fixedDeltaTime;
        }

        if (hsTime > 0)
        {
            hsTime += -Time.fixedDeltaTime;
        }

        if (promoTime > 0)
        {
            promoTime += -Time.fixedDeltaTime;
        }
        // We apply gravity manually for more tuning control
        GetComponent<Rigidbody>().AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));

        grounded = false;

        


        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.P))
        {
            if (!paused)
            {
                paused = true;
            }
            else
            {
                paused = false;
            }
        }

        Cursor.visible = paused;

        if (paused)
        {
            Cursor.lockState = CursorLockMode.None;
            if (fpsCam.GetComponent<mouseLook>() != null)
            {
                fpsCam.GetComponent<mouseLook>().enabled = false;
            }          
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            if (fpsCam.GetComponent<mouseLook>() != null)
            {
                fpsCam.GetComponent<mouseLook>().enabled = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            if (!pv.isMine)
            {
                pv.RPC("proneUp", PhotonTargets.AllBuffered, null);
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            pv.RPC("prone", PhotonTargets.AllBuffered, null);
        }

        if (body.height == crouchHeight)
        {
            proned = true;
            curSpeed = speed / -crouchMultiplier;
        }
        else
        {
            proned = false;
        }

        
    }

    Vector2 scroll = Vector2.zero;

    void OnCollisionStay()
    {
        grounded = true;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    [PunRPC]
    public void ApplyDamage(int dmg, string name, string col)
    {
        killer = name;
        if (col != head.name)
        {
            health += -dmg;
        }
        else
        {
            health += -dmg * 4;
        }

        if (health <= 0 && pv.isMine)
        {
            pv.RPC("Die", PhotonTargets.AllBuffered, null);
        }

        camRecoil cR = fpsCam.GetComponentInParent<camRecoil>();
        if(cR != null)
        {
            cR.StartRecoil(0.3f, Random.Range(-9, 9), 10);
        }         
        bloodyScreen = bloodyScreens[Random.Range(0, bloodyScreens.Length)];
        bloodTime = 1;
    }

    bool rdSpawned = false;

    [PunRPC]
    public void Die()
    {
        if (!rdSpawned)
        {
            
            if (GetComponent<PhotonView>().isMine)
            {
                rdSpawned = true;
                PlayerPrefs.SetInt("deaths", PlayerPrefs.GetInt("deaths") + 1);
                GameObject rd = PhotonNetwork.Instantiate(ragdoll.name, transform.position, transform.rotation, 0) as GameObject;               
                rd.GetComponent<ragDoll>().fpCam.SetActive(true);
                rd.GetComponent<PhotonView>().RPC("setCosm", PhotonTargets.AllBuffered, PlayerPrefs.GetString(PlayerPrefs.GetString("class") + "cosm"));
                GameObject.Find("_Room").SendMessage("Died", killer);
                GameObject.Find("_Network").GetComponent<PhotonView>().RPC("addFeed", PhotonTargets.AllBuffered, killer, PhotonNetwork.playerName);
                if (GameObject.Find(killer) != null && GameObject.Find(killer).GetComponent<PhotonView>() != null)
                {
                    GameObject.Find(killer).GetComponent<PhotonView>().RPC("gotKill", PhotonTargets.AllBuffered, PhotonNetwork.playerName);

                }

                foreach(GameObject drop in wepDrops)
                {
                    if(drop.GetComponent<wepPickup>().wepInfo.prefabName == PlayerPrefs.GetString(PlayerPrefs.GetString("class") + "wep"))
                    {
                        PhotonNetwork.Instantiate(drop.name, transform.position, transform.rotation, 0);
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void setName(string nm)
    {
        gameObject.name = nm;
    }

    string victim = "";

    void OnGUI()
    {

        
        if (bloodTime > 0)
        {
            Color c = Color.white;
            c.a = bloodTime;
            GUI.color = c;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bloodyScreen, ScaleMode.StretchToFill);
        }

        if (GameObject.Find("_Room") != null)
        {
            GUI.skin = GameObject.Find("_Room").GetComponent<roomManager>().skin;
        }
        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 300, 300), instructions);

        GUIStyle popUp = new GUIStyle("Label");
        popUp.alignment = TextAnchor.UpperCenter;
        popUp.fontSize = 24;

        if (fragTime > 0)
        {
            Color c = Color.white;
            c.a = fragTime;
            GUI.color = c;
            GUI.Label(new Rect(Screen.width / 2 - 375, Screen.height - 50, 750, 50), "Fragged " + victim + "  +" + rew + " XP +$" + cashReward, popUp);
        }

        if(hsTime > 0)
        {
            Color c = Color.white;
            c.a = hsTime;
            GUI.color = c;
            GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height - 100, 500, 50), "Headshot +10 XP +$20", popUp);
        }

        if (promoTime > 0)
        {
            Color c = Color.white;
            c.a = hsTime;
            GUI.color = c;
            GUI.Label(new Rect(Screen.width / 2 - 100, 50, 200, 50), "Promoted!", popUp);
            if(curRank != null)
            {
                GUI.Label(new Rect(Screen.width / 2 - 75, 75, 150, 150), curRank.icon, popUp);
            }
        }


        GUI.color = Color.white;
        GUI.Box(new Rect(10, Screen.height - 40, 140, 30), "HP | " + health);

        if (paused)
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 300)); //Pause Menu
            GUILayout.BeginVertical("Box");
            GUILayout.Label("Paused");
            scroll = GUILayout.BeginScrollView(scroll);

            foreach (PhotonPlayer pl in PhotonNetwork.playerList)
            {
                GUILayout.BeginHorizontal("Box");
                GUILayout.Label(pl.NickName);
                GUILayout.Label(" | " + pl.GetScore());
                if (PhotonNetwork.isMasterClient)
                {
                    if (GUILayout.Button("Kick"))
                    {
                        PhotonNetwork.CloseConnection(pl);
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            GUILayout.Label("Bots: " + GameObject.FindObjectsOfType<botAi>().Length);
            GUILayout.Label("Zombies: " + GameObject.FindObjectsOfType<zombieAi>().Length);
            if (GUILayout.Button("Suicide"))
            {
                GetComponent<PhotonView>().RPC("ApplyDamage", PhotonTargets.AllBuffered, 99999, "World", "head");
            }
            if (GUILayout.Button("Resume"))
            {
                paused = false;
            }
            if (GUILayout.Button("Disconnect"))
            {
                GetComponent<PhotonView>().RPC("ApplyDamage", PhotonTargets.AllBuffered, 99999, "World", "head");
                GameObject[] plObjs = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject plObj in plObjs)
                {
                    Destroy(plObj);
                }
                PhotonNetwork.LeaveRoom();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }

    int rew = 1;
    int cashReward = 1;

    public int cashRewardLow = 20;
    public int cashRewardHigh = 50;
    Collider colHit;

    [PunRPC]
    public void gotKill(string vic)
    {
        if (pv.isMine)
        {
            PhotonNetwork.player.AddScore(1);
            fragTime = 3;
            victim = vic;
            kills += 1;
            checkPromo();
            rew = Random.Range(xpRewardLow, xpRewardHigh);
            cashReward = Random.Range(cashRewardLow, cashRewardHigh);
            PlayerPrefs.SetInt("kills", PlayerPrefs.GetInt("kills") + 1);
            PlayerPrefs.SetInt("xp", PlayerPrefs.GetInt("xp") + rew);
            PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") + cashReward);
            if (colHit != null)
            {
                if(colHit.name == "head" || colHit.name == "Head" || colHit.name == "Sphere")
                {
                    gotHeadShot();
                    if(gotHeadShotSFX != null)
                    {
                        GetComponent<AudioSource>().PlayOneShot(gotHeadShotSFX);
                    }
                }
            }
            if (gotKillSound != null)
            {
                GetComponent<AudioSource>().PlayOneShot(gotKillSound);
            }
            if (kills > 1 && kills <= killStreaksInOrder.Length - 1) 
            {
                if (killStreaksInOrder != null && killStreaksInOrder.Length >= kills - 2)
                {
                    if (killStreaksInOrder != null && killStreaksInOrder[kills - 2] != null)
                    {
                        GetComponent<AudioSource>().PlayOneShot(killStreaksInOrder[kills - 2]);
                    }
                }
            }

        }
    }

    float promoTime = 0;

    public void checkPromo()
    {
        roomManager rM = GameObject.Find("_Room").GetComponent<roomManager>();
        curRank = rM.curRank;
        rM.updateRank();
        if(curRank != rM.curRank)
        {            
            Debug.Log("Promoted!");
            curRank = rM.curRank;       
            pv.RPC("setName", PhotonTargets.AllBuffered, PhotonNetwork.playerName);
            if(promotion != null && GetComponent<AudioSource>() != null){
                GetComponent<AudioSource>().PlayOneShot(promotion);
            }
            if (rM.curRank != null && PhotonNetwork.inRoom)
            {
                GameObject.Find("_Network").GetComponent<PhotonView>().RPC("promo", PhotonTargets.AllBuffered, GameObject.Find("_Room").GetComponent<roomManager>().playerName, rM.curRank.rankName);
            }
            promoTime = 5;
        }
    }

    public void gotHeadShot()
    {
        hsTime = 3;
        PlayerPrefs.SetInt("xp", PlayerPrefs.GetInt("xp") + 10);
        PlayerPrefs.SetInt("cash", PlayerPrefs.GetInt("cash") + 15);
        PlayerPrefs.SetInt("headshots", PlayerPrefs.GetInt("headshots") + 1);
    }

    public void hitCol(Collider col)
    {
        //Debug.Log(col.name);
        colHit = col;
    }

    [PunRPC]
    public void setPFP(string url)
    {
        StartCoroutine(updatePFP(url));
    }

    IEnumerator updatePFP(string url)
    {
        while (true)
        {
            WWW www = new WWW(url);
            yield return www;
            pfp = www.texture;
        }
    }

    [PunRPC]
    public void prone()
    {
        body.height = crouchHeight;

          
    }

    [PunRPC]
    public void proneUp()
    {
        body.height = normalHeight;
        if (!pv.isMine)
        {
            float displace = normalHeight - crouchHeight - 1;
            transform.Translate(Vector3.up * displace);
        }      
    }

}
