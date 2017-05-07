using UnityEngine;
using System.Collections;

public class randomTexture : MonoBehaviour {

    public SkinnedMeshRenderer bodyMesh;
    public Texture[] possibleTextures;
    [HideInInspector]
    public string activeTexture = "";

    [PunRPC]
    public void applyNewTexture(string tex)
    {
        foreach (Texture posTex in possibleTextures)
        {
            if (posTex.name == tex)
            {
                activeTexture = tex;
                bodyMesh.material.mainTexture = posTex;
            }
        }
    }


}
