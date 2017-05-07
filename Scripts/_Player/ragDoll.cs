using UnityEngine;
using System.Collections;

public class ragDoll : MonoBehaviour {

    public GameObject fpCam;
    public float lifeSpan = 15;
    public AudioClip[] grunts;
    bool dead = false;

    void Awake()
    {
        if (fpCam != null)
        {
            Destroy(fpCam, 3);

            if (fpCam.GetComponent<AudioListener>() != null)
            {
                fpCam.GetComponent<AudioListener>().enabled = false;
            }
        }

        GetComponent<AudioSource>().PlayOneShot(grunts[Random.Range(0, grunts.Length)]);      
    }

    void Update()
    {        

        if (lifeSpan < 0)
        {
            if (gameObject != null)
            {
                if (GetComponent<PhotonView>() != null)
                {
                    if (!dead)
                    {

                        if (PhotonNetwork.inRoom)
                        {
                            if (GetComponent<PhotonView>().isActiveAndEnabled)
                            {
                                GetComponent<PhotonView>().RPC("Die", PhotonTargets.AllBuffered, null);
                            }
                        }
                    }
                }
            }
            
        } else
        {
            lifeSpan += -Time.deltaTime;                                                                                                                        
        }
    }

    [PunRPC]
    public void Die()
    {
        dead = true;
        Destroy(gameObject);
    }

  
}
