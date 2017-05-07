using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour {

    public GameObject impactPrefab;

    [PunRPC]
    public void moveFwd(float spd)
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * spd);
    }

    void OnCollisionEnter()
    {
        if (GetComponent<PhotonView>() != null)
        {
            GetComponent<PhotonView>().RPC("explode", PhotonTargets.All, null);
        }
    }

    [PunRPC]
    public void explode()
    {
        GameObject impact = Instantiate(impactPrefab, transform.position, transform.rotation) as GameObject;
        Destroy(impact, 3);
        Destroy(gameObject);
    }
}
