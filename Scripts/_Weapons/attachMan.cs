using UnityEngine;
using System.Collections;

public class attachMan : MonoBehaviour {

    public GameObject defaultSight;
    public GameObject[] barrelAttachments;
    public GameObject[] sightAttachments;


    void Awake()
    {
        setAttachment();

        if(!PlayerPrefs.HasKey(gameObject.name + "sight") && defaultSight != null){
            defaultSight.SetActive(true);
        }

        setLayer();
    }

    void setAttachment()
    {
        foreach(GameObject attachment in barrelAttachments)
        {
            if(attachment.name == PlayerPrefs.GetString(gameObject.name + "barrel"))
            {
                attachment.SetActive(true);
            } else
            {
                attachment.SetActive(false);
            }
        }

        foreach (GameObject attachment in sightAttachments)
        {
            if (attachment.name == PlayerPrefs.GetString(gameObject.name + "sight"))
            {
                attachment.SetActive(true);
            }
            else
            {
                attachment.SetActive(false);
            }
        }

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
