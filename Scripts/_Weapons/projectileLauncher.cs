using UnityEngine;
using System.Collections;

public class projectileLauncher : MonoBehaviour {

    public int clipSize = 6;
    public Animation anim;
    public AnimationClip shoot;
    public AnimationClip reload;
    public AnimationClip draw;
    public camRecoil recoilScript;
    [HideInInspector]
    public int ammo = 0;
    [HideInInspector]
    public int oldAmmo = 0;
    public float shootSpeed = 500;
    public GameObject projectile;
    public Transform forward;

    public GameObject groundHit;
    public GameObject bloodHit;

    public Transform muzzle;
    public GameObject muzzleFlash;

    public AudioClip shootSFX;
    public AudioClip reloadSFX;

    public float recoilAmnt = 0.3f;
    public float recoilShake = 13;
    public float recoilSpeed = 5;

    public PhotonView tpEff;

    public int magazines = 3;

    void Awake()
    {
        ammo = clipSize;

        setLayer();
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }

        if (Input.GetKeyUp(KeyCode.R))
        {
            rel();
        }
    }

    public void Fire()
    {
        if (!anim.IsPlaying(shoot.name) && !anim.IsPlaying(reload.name) && !anim.IsPlaying(draw.name))
        {
            if (ammo > 0)
            {
                anim.Play(shoot.name);
                ammo += -1;

                if (muzzleFlash && muzzle != null)
                {
                    GameObject mf = Instantiate(muzzleFlash, muzzle.position, muzzle.rotation) as GameObject;
                    mf.transform.parent = transform;
                    Destroy(mf, 1);
                }
                GameObject proj = PhotonNetwork.Instantiate(projectile.name, forward.position, forward.rotation, 0) as GameObject;
                proj.GetComponent<PhotonView>().RPC("moveFwd", PhotonTargets.All, shootSpeed);

                tpEff.RPC("shootEff", PhotonTargets.All, shootSFX.name);
                if (recoilScript != null)
                {
                    recoilScript.StartRecoil(recoilAmnt, recoilShake, recoilSpeed);
                }
            }
            else
            {
                rel();
            }
        }
    }

    public void rel()
    {
        if (magazines > 0)
        {
            if (ammo < clipSize)
            {
                oldAmmo = ammo;
                anim.CrossFade(reload.name);
                magazines += -1;
                ammo = clipSize;
                tpEff.RPC("reload", PhotonTargets.AllBuffered, reloadSFX.name);
            }
        }
    }

    void OnEnable()
    {
        anim.Play(draw.name);
    }

    void OnGUI()
    {
        if (GameObject.Find("_Room") != null)
        {
            GUI.skin = GameObject.Find("_Room").GetComponent<roomManager>().skin;
        }
        GUIStyle ammoStyle = new GUIStyle(GUI.skin.box);
        ammoStyle.fontSize = 36;
        GUIStyle ammoStyle2 = new GUIStyle(GUI.skin.box);
        ammoStyle2.fontSize = 24;
        ammoStyle2.alignment = TextAnchor.MiddleLeft;
        GUI.Label(new Rect(10, Screen.height - 100, 100, 50), ammo + "/" + clipSize, ammoStyle);
        GUI.Box(new Rect(100, Screen.height - 100, 50, 50), "/" + magazines, ammoStyle2);
    }


    public void setLayer()
    {
        foreach (MeshRenderer mr in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            mr.gameObject.layer = 9;
        }

        foreach (SkinnedMeshRenderer mr in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mr.gameObject.layer = 9;
        }
    }
}

