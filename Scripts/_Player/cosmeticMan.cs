using UnityEngine;
using System.Collections;

public class cosmeticMan : MonoBehaviour {

  
    public GameObject[] cosmetics;
    wepManager wepM;
    public string myItem = "None";

    
    [PunRPC]
    public void setCosm(string objName)
    {
        foreach(GameObject cosmetic in cosmetics)
        {
            if(cosmetic.name == objName)
            {
                cosmetic.SetActive(true);
                myItem = objName;
            } else
            {
                cosmetic.SetActive(false);
            }
        }
    }

}
