using UnityEngine;
using System.Collections;

public class zombieAi : MonoBehaviour {

    public UnityEngine.AI.NavMeshAgent nma;
    public Transform target;
    public tpEffect[] players;
    PhotonView pv;

    public Animation am;
    public AnimationClip walk;
    public AnimationClip idle;
    public AnimationClip shoot;
    public int lowDamage = 5;
    public int maxDamage = 15;
    public AudioClip shootSFX;
    float closestDistance = Mathf.Infinity;

    public float findRadius = 10;

    public float range = 150;

    public Texture pfp;


    void Awake()
    {
        pv = GetComponent<PhotonView>();
        

        if (pv.isMine)
        {
            pv.RPC("setName", PhotonTargets.AllBuffered, "Zombie " + Random.Range(0, 999));
            findNearest();
        }

        nma = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    [PunRPC]
    public void setName(string nm)
    {
        gameObject.name = nm;
    }

   

    void Update()
    {

        players = GameObject.FindObjectsOfType<tpEffect>();

        

       
            if (nma.remainingDistance >= nma.stoppingDistance)
            {
                am.CrossFade(walk.name);
            }
            else
            {
                am.CrossFade(idle.name);
                if (target != null)
                {
                    Vector3 relativePos = target.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    transform.rotation = rotation;

                    fire();
                }
            }

            nma.Resume();      

        if (target != null)
        {
            nma.SetDestination(target.position);
            if (target != transform)
            {
                nma.SetDestination(target.position);
            }
            else
            {
                if (pv.isMine)
                {
                    findNearest();
                }
            }

        }      




        if (target == null)
        {
            if (pv.isMine && players.Length > 0)
            {

                //Debug.Log("Finding");
                findNearest();
            }

        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, findRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject.name != "Cube")
            {
                if (hit.gameObject.tag == "Player")
                {
                    if (hit.gameObject.GetComponent<tpEffect>() != null && hit.gameObject.GetComponent<PhotonView>() != null)
                    {
                        if (hit.gameObject.transform != target)
                        {
                            pv.RPC("setTarget", PhotonTargets.AllBuffered, hit.gameObject.name);
                        }
                    }
                    else
                    {
                        if (hit.gameObject.GetComponentInParent<tpEffect>() != null && hit.gameObject.GetComponentInParent<PhotonView>() != null)
                        {
                            if (hit.gameObject.transform != target)
                            {
                                pv.RPC("setTarget", PhotonTargets.AllBuffered, hit.gameObject.name);
                            }
                        }
                    }
                }
                else
                {
                    if (hit.transform.parent != null)
                    {
                        if (hit.transform.parent.tag == "Player")
                        {
                            if (hit.gameObject.GetComponentInParent<tpEffect>() != null && hit.gameObject.GetComponentInParent<PhotonView>() != null)
                            {
                                if (hit.gameObject.transform.parent != target)
                                {
                                    pv.RPC("setTarget", PhotonTargets.AllBuffered, hit.transform.parent.name);
                                }
                            }                            
                        }
                    }
                }
            }
        }
    } 
    

    public void findNearest()
    {
        closestDistance = 9999;
        if (players.Length > 0)
        {

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
                    pv.RPC("setTarget", PhotonTargets.AllBuffered, players[Random.Range(0, players.Length)].gameObject.name);
                }
            }
        }
    }

    public void fire()
    {
        if (!am.IsPlaying(shoot.name) && target != null && Vector3.Distance(transform.position, target.position) <= nma.stoppingDistance)
        {
            am.Play(shoot.name);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, range))
            {
                if (hit.transform.GetComponent<PhotonView>() != null)
                {
                    if (pv.isMine && hit.transform.GetComponent<PhotonView>() != null)
                    {
                        if (hit.transform.GetComponent<tpEffect>() != null)
                        {
                            hit.transform.GetComponent<PhotonView>().RPC("ApplyDamage", PhotonTargets.AllBuffered, Random.Range(lowDamage, maxDamage), gameObject.name, hit.collider.name);
                        }
                    }
                }
            }

            pv.RPC("shootEff", PhotonTargets.All);

        }   
    }
   

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<tpEffect>() != null)
        {
            
            pv.RPC("setTarget", PhotonTargets.AllBuffered, col.gameObject.name);
            
        }
                
    }

    [PunRPC]
    public void shootEff()
    {
        GetComponent<AudioSource>().PlayOneShot(shootSFX);
    }


}
