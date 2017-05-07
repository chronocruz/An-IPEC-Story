using UnityEngine;
using System.Collections;

public class botAi : MonoBehaviour {

    public UnityEngine.AI.NavMeshAgent nma;
    public Transform target;
    public tpEffect[] players;
    PhotonView pv;

    public Animation am;
    public AnimationClip walk;
    public AnimationClip idle;
    public AnimationClip shoot;
    public AnimationClip reload;
    public int clipSize = 30;
    int ammo;
    public int lowDamage = 5;
    public int maxDamage = 15;
    public AudioClip shootSFX;
    float closestDistance = Mathf.Infinity;

    public float range = 150;

    public AudioClip reloadSFX;

    public Texture pfp;

    public GameObject blood;

    public bool drawTracer = true;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        ammo = clipSize;

        if (pv.isMine)
        {
            pv.RPC("setName", PhotonTargets.AllBuffered, "Bot " + Random.Range(0, 999));
            findNearest();
        }      
    }

    [PunRPC]
    public void setName(string nm)
    {
        gameObject.name = nm;
    }

    zombieAi[] zombies;
    tpEffect[] soldiers;

    void Update()
    {

        players = GameObject.FindObjectsOfType<tpEffect>();
        zombies = GameObject.FindObjectsOfType<zombieAi>();


        if (!am.IsPlaying(reload.name))
        {
            if (nma.remainingDistance >= nma.stoppingDistance)
            {
                am.CrossFade(walk.name);
            }
            else
            {
                am.CrossFade(idle.name);                
                if (target != null)
                {
                    fire();
                    Vector3 relativePos1 = target.position - transform.position;
                    Vector3 relativePos = relativePos1 * Time.deltaTime;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = rotation;                   
                }
            }

            nma.Resume();

        }
        else
        {
            nma.Stop();
        }

        if (target != null)
        {
            if (target != transform)
            {
                nma.SetDestination(target.position);
            }
            else
            {
                if (pv.isMine)
                {
                    if (players.Length > 1 || zombies.Length > 0)
                    {
                        //Debug.Log("Finding");
                        findNearest();
                    }
                }
            }

        }      




        if (target == null)
        {

            if (pv.isMine)
            {
                if (players.Length > 1 || zombies.Length > 0)
                {
                    //Debug.Log("Finding");
                    findNearest();
                }
            }

        }

    }


    public void findNearest()
    {
        zombies = FindObjectsOfType<zombieAi>();
        players = FindObjectsOfType<tpEffect>();
        if (zombies.Length > 0)
        {
            closestDistance = 9999;
            foreach (zombieAi pl in zombies)
            {

                if (Vector3.Distance(transform.position, pl.transform.position) <= closestDistance)
                {
                    if (pl.transform != transform)
                    {
                        if (target != pl.transform)
                        {
                            closestDistance = Vector3.Distance(transform.position, pl.transform.position);
                            //Debug.Log("Loop");
                            pv.RPC("setTarget", PhotonTargets.AllBuffered, pl.gameObject.name);
                        }
                    }
                }

            }

            if (target == null)
            {

                pv.RPC("setTarget", PhotonTargets.AllBuffered, players[Random.Range(0, players.Length)].name);

            }

        }
        else {
            if (players.Length > 1)
            {
                closestDistance = 9999;
                foreach (tpEffect pl in players)
                {

                    if (Vector3.Distance(transform.position, pl.transform.position) <= closestDistance)
                    {
                        if (pl.transform != transform)
                        {
                            if (target != pl.transform)
                            {
                                closestDistance = Vector3.Distance(transform.position, pl.transform.position);
                                //Debug.Log("Loop");
                                pv.RPC("setTarget", PhotonTargets.AllBuffered, pl.gameObject.name);
                            }
                        }
                    }

                }

                if (target == null)
                {

                    pv.RPC("setTarget", PhotonTargets.AllBuffered, players[Random.Range(0, players.Length)].name);

                }
            }
        }
    }


   
    [PunRPC]
    public void setTarget(string name)
    {
        if (GameObject.Find(name) != null && name != gameObject.name)
        {
           
                target = GameObject.Find(name).transform;
            
        } else
        {
            if (pv.isMine)
            {
                if (players.Length > 0)
                {
                    findNearest();
                }
            }
        }
    }

    public float innaccuracy = 0.2f;

    public void fire()
    {
        if (!am.IsPlaying(shoot.name) && target != null && Vector3.Distance(transform.position, target.position) <= nma.stoppingDistance)
        {
            if (!am.IsPlaying(reload.name))
            {
                if (ammo >= 0)
                {                   
                    am.Play(shoot.name);
                    ammo += -1;

                    RaycastHit hit;                   
                    Vector3 shootHit = new Vector3(transform.forward.x + Random.Range(-innaccuracy / 4, innaccuracy / 4), transform.forward.y + Random.Range(-innaccuracy / 4, innaccuracy / 4), transform.forward.z);
                    if (Physics.Raycast(transform.position, shootHit, out hit, range))
                    {
                        if (drawTracer)
                        {
                            GameObject line = new GameObject();
                            line.transform.SetParent(GameObject.Find("_Room").transform);
                            line.name = "tracer";
                            LineRenderer ln = line.AddComponent<LineRenderer>();
                            ln.SetWidth(0.01f, 0.005f);
                            ln.material = new Material(Shader.Find("Diffuse"));
                            ln.material.color = Color.yellow;
                            tpEffect tpE = GetComponent<tpEffect>();
                            if (tpE == null)
                            {
                                ln.SetPosition(0, transform.position);
                            } else
                            {
                                ln.SetPosition(0, tpE.muzzle.position);
                            }
                            ln.SetPosition(1, hit.point);
                            Destroy(line, 0.05f);
                        }

                        if (hit.transform.GetComponent<PhotonView>() != null)
                        {
                            if (pv.isMine)
                            {
                                if (hit.transform.GetComponent<tpEffect>() != null)
                                {
                                    hit.transform.GetComponent<PhotonView>().RPC("ApplyDamage", PhotonTargets.AllBuffered, Random.Range(lowDamage, maxDamage), gameObject.name, hit.collider.name);
                                }
                            }
                            if (blood != null)
                            {
                                GameObject bl = Instantiate(blood, hit.point, Quaternion.identity) as GameObject;
                                Destroy(bl, 3);
                            }
                        }
                        
                    }

                    pv.RPC("shootEff", PhotonTargets.All, shootSFX.name);
                }
                else
                {
                    reloadM();
                }
            }
        }
    }

    public void reloadM()
    {
        //Debug.Log("reload");
        am.Play(reload.name);
        ammo = clipSize;
        if (reloadSFX)
        {
            GetComponent<AudioSource>().PlayOneShot(reloadSFX);
        }  
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<tpEffect>() != null)
        {
            
            pv.RPC("setTarget", PhotonTargets.AllBuffered, col.gameObject.name);
            
        }
                
    }

    

    

 
}
