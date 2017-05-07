using UnityEngine;
using System.Collections;

public class dummyAi : MonoBehaviour {

    public int health = 100;
    public Collider head;
    public GameObject ragdoll;
    string killer = "World";
    PhotonView pv;
    public string prefName = "Bot";

    public GameObject wepDrop;

    

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            if (pv.isMine && pv.owner.IsMasterClient)
            {
                /* Deprecated Bot Safe Gaurd for Max Spawn Count
                int botCount = GameObject.FindObjectsOfType<botAi>().Length;
                int allowedBots = PlayerPrefs.GetInt("bots");
                int zombieCount = GameObject.FindObjectsOfType<zombieAi>().Length;
                int allowedZombies = PlayerPrefs.GetInt("zombies");

                if (prefName == "Bot")
                {
                    if (botCount > allowedBots)
                    {
                        Debug.Log("Too many bots! Current: " + botCount + "  Allowed: " + allowedBots);
                        pv.RPC("killMe", PhotonTargets.AllBuffered, null);
                    }
                }
                if (prefName == "Zombie")
                {
                    if (zombieCount > allowedZombies)
                    {
                        pv.RPC("killMe", PhotonTargets.AllBuffered, null);
                    }
                }

                if (zombieCount <= allowedZombies)
                {
                    GameObject.Find("_Room").SendMessage("spawnBot", "Zombie");
                }
                if (botCount <= allowedBots)
                {
                    GameObject.Find("_Room").SendMessage("spawnBot", "Bot");
                }*/

            }
        }
    }

    void FixedUpdate()
    {
        if (health <= 0)
        {
            pv.RPC("Die", PhotonTargets.AllBuffered, null);
        }
    }

    [PunRPC]
    public void ApplyDamage(int dmg, string name, string col)
    {
        killer = name;
        if (col != head.name)
        {
            health += -dmg;
        } else
        {
            health += -dmg * 4;
        }      

        if (pv.isMine)
        {
            pv.RPC("setTarget", PhotonTargets.AllBuffered, name);

           
        }
        

    }


    bool ragDollSpawned = false;

    [PunRPC]
    public void Die()
    {
        if (pv.isMine && pv.owner.IsMasterClient)
        {
            if (!ragDollSpawned)
            {
                GameObject rd = PhotonNetwork.Instantiate(ragdoll.name, transform.position, transform.rotation, 0) as GameObject;
                ragDollSpawned = true;
                cosmeticMan cM = GetComponent<cosmeticMan>();
                if (cM != null)
                {
                    if (rd.GetComponent<cosmeticMan>() != null)
                    {
                        rd.GetComponent<PhotonView>().RPC("setCosm", PhotonTargets.AllBuffered, cM.myItem);
                       
                    }
                }
                randomTexture rT = GetComponent<randomTexture>();
                if(rT != null)
                {
                    randomTexture ragDollRT = rd.GetComponent<randomTexture>();
                    if(ragDollRT != null)
                    {
                        if (ragDollRT.bodyMesh != null && ragDollRT.possibleTextures.Length > 0)
                        {
                            rd.GetComponent<PhotonView>().RPC("applyNewTexture", PhotonTargets.AllBuffered, rT.activeTexture);
                        }
                    }
                }

                if (GameObject.Find(killer) != null)
                {
                    if (GameObject.Find(killer).GetComponent<PhotonView>() != null)
                    {
                        if (GameObject.Find(killer).GetComponent<tpEffect>() != null)
                        {
                            GameObject.Find(killer).GetComponent<PhotonView>().RPC("gotKill", PhotonTargets.AllBuffered, gameObject.name);
                            GameObject.Find("_Network").GetComponent<PhotonView>().RPC("addFeed", PhotonTargets.AllBuffered, killer, gameObject.name);
                        }
                    }
                }

            }

            if (pv.isMine && pv.owner.IsMasterClient && pv.owner.IsLocal)
            {
                GameObject.Find("_Room").SendMessage("spawnBot", prefName);
            }

            if (wepDrop != null)
            {
                PhotonNetwork.Instantiate(wepDrop.name, transform.position, transform.rotation, 0);
            }

            pv.RPC("killMe", PhotonTargets.AllBuffered, null);

        }
    }

    [PunRPC]
    public void killMe()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void gotKill(string vic)
    {
        //
    }
}
