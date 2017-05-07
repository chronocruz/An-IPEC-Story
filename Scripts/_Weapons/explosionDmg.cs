using UnityEngine;
using System.Collections;

public class explosionDmg : MonoBehaviour {

    public float dmg = 120;
    public float rd = 10;
    public float force = 100;

    void Awake()
    {
        if (GetComponent<PhotonView>().isMine)
        {
            AreaDamageEnemies(transform.position, rd, dmg);
        }
        
    }

    void AreaDamageEnemies(Vector3 location, float radius, float damage)
    {
        
            Collider[] objectsInRange = Physics.OverlapSphere(location, radius);
        foreach (Collider col in objectsInRange)
        {
            if (GetComponent<PhotonView>().isMine && col != null)
            {
                float proximity = (location - col.transform.position).magnitude;
                float effect = 1 - (proximity / radius);
                float dam = effect * damage;

                PhotonView enemy = col.GetComponent<PhotonView>();

                if (enemy != null)
                {
                    if (enemy.gameObject.GetComponent<tpEffect>() != null || col.gameObject.GetComponent<zombieAi>() != null)
                    {
                        if (enemy != null)
                        {
                            if (col.gameObject != null)
                            {
                                enemy.RPC("ApplyDamage", PhotonTargets.AllBuffered, (int)dam, PhotonNetwork.playerName, col.name);
                            }
                        }
                    }               

                } else {

                    PhotonView enemy2 = col.GetComponentInParent<PhotonView>();
                    if (enemy2 != null)
                    {
                        if (enemy2.gameObject.GetComponent<tpEffect>() != null || col.gameObject.GetComponent<zombieAi>() != null)
                        {
                            if (enemy2 != null)
                            {
                                if (col.gameObject != null)
                                {
                                    enemy2.RPC("ApplyDamage", PhotonTargets.AllBuffered, (int)dam, PhotonNetwork.playerName, col.name);
                                }
                            }
                        }

                    }

                }
            }

            if(col.GetComponent<Rigidbody>() != null)
            {
                float dis = Vector3.Distance(transform.position, col.transform.position);
                //col.GetComponent<Rigidbody>().AddExplosionForce(force, transform.position, radius);
                col.GetComponent<Rigidbody>().AddForce(Vector3.up * force * Random.Range(0, 7) * -dis);
                col.GetComponent<Rigidbody>().AddForce(Vector3.forward * force * Random.Range(-7, 7) * -dis);
                col.GetComponent<Rigidbody>().AddForce(Vector3.right * force * Random.Range(-7, 7)  * -dis);
            }
        }
        


    }
}
