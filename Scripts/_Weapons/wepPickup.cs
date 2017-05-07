using UnityEngine;
using System.Collections;

public class wepPickup : MonoBehaviour {

    public wepItem wepInfo;
    public int scoreCost = 0;
    PhotonView pv;
    bool inTrigger = false;
    characterControls cc;
    GUISkin skin;

    public bool destroyOnGet = true;

    public AudioClip pickSFX;
    public AudioClip buySFX;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        skin = GameObject.Find("_Room").GetComponent<roomManager>().skin;

        if (pv.isMine)
        {
            pv.RPC("destroyAfterTime", PhotonTargets.AllBuffered, null);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponentInParent<characterControls>() != null  && col.gameObject.GetComponentInParent<PhotonView>().isMine)
        {
            inTrigger = true;
            cc = col.gameObject.GetComponentInParent<characterControls>();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.GetComponentInParent<characterControls>() != null && col.gameObject.GetComponentInParent<PhotonView>().isMine)
        {
            inTrigger = false;
            cc = null;
        }
    }


    void OnGUI()
    {
        GUI.skin = skin;
        if (cc != null)
        {
            if (inTrigger && cc.gameObject.GetComponent<PhotonView>().isMine)
            {
                if (wepInfo.classFor.name == PlayerPrefs.GetString("class"))
                {
                    if (scoreCost == 0)
                    {
                        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 100, 200, 60), "Pickup: " + wepInfo.displayName + "?" + "\nPress 'E'");
                    }
                    else
                    {
                        GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 100, 200, 60), "Buy: " + wepInfo.displayName + "? $" + scoreCost + "\nPress 'E'");
                    }
                }
                else
                {
                    GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 100, 200, 60), "Pickup: " + wepInfo.displayName + "?" + "\nFor " + wepInfo.classFor.name + " only!");
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (inTrigger && cc.gameObject.GetComponent<PhotonView>().isMine)
            {
                if (scoreCost == 0)
                {
                    if (cc.gameObject.GetComponentInChildren<wepManager>() != null)
                    {
                        cc.gameObject.GetComponentInChildren<wepManager>().pickup(wepInfo.prefabName);
                        if (GetComponent<AudioSource>() != null && pickSFX != null)
                        {
                            GetComponent<AudioSource>().PlayOneShot(pickSFX);
                        }
                        pv.RPC("die", PhotonTargets.AllBuffered, null);
                    }
                } else
                {
                    if(PhotonNetwork.player.GetScore()  >= scoreCost)
                    {
                        PhotonNetwork.player.AddScore(-scoreCost);
                        cc.gameObject.GetComponentInChildren<wepManager>().pickup(wepInfo.prefabName);
                        if(GetComponent<AudioSource>() != null && buySFX != null)
                        {
                            GetComponent<AudioSource>().PlayOneShot(buySFX);
                        }
                        pv.RPC("die", PhotonTargets.AllBuffered, null);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void die()
    {
        if (destroyOnGet)
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void destroyAfterTime()
    {
        Destroy(gameObject, 15);
    }
}
