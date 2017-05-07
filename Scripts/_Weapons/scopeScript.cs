using UnityEngine;
using System.Collections;

public class scopeScript : MonoBehaviour {

    public aimScript wepAim;
    public Vector3 newAimPos;
    public float newFOV = 50f;
    public Texture scopeTexOptional;
    public GameObject[] objectsToDisable;
  

    void OnEnable()
    {

        if (wepAim != null)
        {
            wepAim.aimPosition = newAimPos;
        }


        if (wepAim != null)
        {
            wepAim.aimedFOV = newFOV;
        }
        

        if (scopeTexOptional != null)
        {
            if (wepAim != null)
            {
                wepAim.scopeTex = scopeTexOptional;
            }
        }

        if(objectsToDisable.Length > 0)
        {
            if (wepAim != null)
            {
                wepAim.objectsToHide = objectsToDisable;
            }
        }
    }
    
}
