using UnityEngine;
using System.Collections;

public class wepManager : MonoBehaviour
{

    public string className = "Infantry";
    public GameObject[] primaries;
    GameObject primary;
    public GameObject secondary;
    public PhotonView tpEffect;
    public Animation knifeAm;
    public AnimationClip knifeSwipe;
    public AnimationClip grenadeThrow;
    public int knifeDmg = 200;
    public float knifeRange = 3;
    public Transform throwPosition;
    public float grenadeSpeed = 200;
    public GameObject grenade;
    public float throwDelay = 0.5f;

    public float knifeRecoil = -0.2f;
    public camRecoil recoilScript;

    public GameObject bloodHit;

    public AudioClip knifeSound;

    public int grenadeAmmo = 3;


    float gTime = 0;

    GUISkin skin;


    void Awake()
    {

        primary = primaries[0];

        if (GameObject.Find("_Room") != null)
        {
            skin = GameObject.Find("_Room").GetComponent<roomManager>().skin;
        }

        foreach (GameObject pr in primaries)
        {
            if (pr.name == PlayerPrefs.GetString(className + "wep"))
            {
                primary = pr;
            }
            else {
                pr.SetActive(false);
            }
        }

        choosePrim();
    }


    public void choosePrim()
    {
        gunScript gS = secondary.GetComponent<gunScript>();
        projectileLauncher pL = secondary.GetComponent<projectileLauncher>();
        curIsPrim = true;
        if (gS != null)
        {
            if (gS.anim.IsPlaying(gS.reload.name))
            {
                gS.ammo = gS.oldAmmo;
                gS.magazines += 1;
                gS.anim.Play(gS.shoot.name);
            }
        }

        if (pL != null)
        {
            if (pL.anim.IsPlaying(pL.reload.name))
            {
                pL.ammo = pL.oldAmmo;
                pL.magazines += 1;
                pL.anim.Play(pL.shoot.name);
            }
        }

        primary.SetActive(true);
        secondary.SetActive(false);
        if (tpEffect != null)
        {
            tpEffect.RPC("updateModel", PhotonTargets.AllBuffered, primary.name);
        }
    }

    public void chooseSec()
    {
        gunScript gS = primary.GetComponent<gunScript>();
        projectileLauncher pL = primary.GetComponent<projectileLauncher>();
        curIsPrim = false;
        if (gS != null)
        {
            if (gS.anim.IsPlaying(gS.reload.name))
            {
                gS.ammo = gS.oldAmmo;
                gS.magazines += 1;
                gS.anim.Play(gS.shoot.name);
            }
        }

        if (pL != null)
        {
            if (pL.anim.IsPlaying(pL.reload.name))
            {
                pL.ammo = pL.oldAmmo;
                pL.magazines += 1;
                pL.anim.Play(pL.shoot.name);
            }
        }
        primary.SetActive(false);
        secondary.SetActive(true);
        if (tpEffect != null)
        {
            tpEffect.RPC("updateModel", PhotonTargets.AllBuffered, secondary.name);
        }
    }

    void Update()
    {

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            choosePrim();
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            chooseSec();
        }

        gTime -= Time.deltaTime;

        if(gTime > 0 && gTime <= 0.03f)
        {
            spawnNade();
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (primary.activeInHierarchy)
            {
                chooseSec();
            }
            else
            {
                choosePrim();
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {          
            meleeAttack();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            throwGrenade();
        }

        if (knifeAm.isPlaying)
        {
            primary.SetActive(false);
            secondary.SetActive(false);
        } else
        {
            if(!primary.activeInHierarchy || !secondary.activeInHierarchy)
            {
                if (curIsPrim)
                {
                    choosePrim();
                } else
                {
                    chooseSec();
                }
            }
        }

        
    }

    bool curIsPrim = true;

    public void pickup(string nme)
    {

        foreach (GameObject pr in primaries)
        {
            if (pr.name == nme)
            {
                if (primary != pr)
                {
                    primary = pr;
                }
                else
                {
                    if (primary.GetComponent<projectileLauncher>() == null)
                    {
                        primary.GetComponent<gunScript>().magazines += 5;
                    }
                    else
                    {
                        primary.GetComponent<projectileLauncher>().magazines += 3;
                    }
                }
            }
            else {
                pr.SetActive(false);
            }
        }

        choosePrim();
    }

    public void meleeAttack()
    {
        if (knifeAm != null && knifeSwipe != null)
        {
            if (!knifeAm.isPlaying)
            {
                knifeAm.Play(knifeSwipe.name);

                if (recoilScript != null)
                {
                    recoilScript.StartRecoil(knifeRecoil, -5, 5);
                }

                if(knifeSound != null)
                {
                    tpEffect.RPC("shootEff", PhotonTargets.All, knifeSound.name);
                }
                RaycastHit hit;

                if (Physics.Raycast(transform.position, transform.forward, out hit, knifeRange))
                {
                    if (hit.transform.GetComponent<PhotonView>() != null)
                    {
                        if (hit.transform.GetComponent<tpEffect>() != null || hit.transform.GetComponent<zombieAi>() != null)
                        {
                            hit.transform.GetComponent<PhotonView>().RPC("ApplyDamage", PhotonTargets.AllBuffered, knifeDmg, PhotonNetwork.playerName, hit.collider.name);
                            GameObject par = Instantiate(bloodHit, hit.point, Quaternion.identity) as GameObject;
                            Destroy(par, 5f);
                        }
                        else
                        {
                            if(hit.transform.GetComponentInParent<PhotonView>() != null)
                            {
                                if (hit.transform.GetComponentInParent<tpEffect>() != null || hit.transform.GetComponentInParent<zombieAi>() != null)
                                {
                                    hit.transform.GetComponentInParent<PhotonView>().RPC("ApplyDamage", PhotonTargets.AllBuffered, knifeDmg, PhotonNetwork.playerName, hit.collider.name);
                                    GameObject par = Instantiate(bloodHit, hit.point, Quaternion.identity) as GameObject;
                                    Destroy(par, 5f);
                                }
                            }
                        }
                    } 
                }
            }
        }
    }

    public void throwGrenade()
    {
        if (grenadeAmmo > 0)
        {
            if (knifeAm != null && grenadeThrow != null && throwPosition != null && grenade != null)
            {
                if (!knifeAm.isPlaying)
                {
                    gTime = throwDelay;
                    if (recoilScript != null)
                    {
                        recoilScript.StartRecoil(knifeRecoil, 5, 5);
                    }
                    knifeAm.Play(grenadeThrow.name);
                }
            }
        }
        
    }

    void spawnNade()
    {
        gTime = 0;
        if (recoilScript != null)
        {
            recoilScript.StartRecoil(knifeRecoil, -7, 10);
        }
        GameObject proj = PhotonNetwork.Instantiate(grenade.name, throwPosition.position, throwPosition.rotation, 0) as GameObject;
        proj.GetComponent<PhotonView>().RPC("moveFwd", PhotonTargets.All, grenadeSpeed * 2);
        grenadeAmmo -= 1;
    }

    void OnGUI()
    {
        GUI.skin = skin;
        GUI.Box(new Rect(Screen.width - 150, 10, 140, 30), "Nades: " + grenadeAmmo);

    }

}
