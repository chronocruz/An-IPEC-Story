using UnityEngine;
using System.Collections;

public class skinMan : MonoBehaviour {

    public MeshRenderer[] meshesToPaint;
    public wepItem myItem;
    skinItem skinToApply;


    void Awake()
    {
        updateSkin();
    }


    public void updateSkin()
    {
        skinToApply = null;
        if (myItem != null)
        {
            if (myItem.customSkins.Length > 0)
            {
                
                foreach (skinItem skin in myItem.customSkins)
                {
                    if (skin.textureToApply.name == PlayerPrefs.GetString(gameObject.name + "skin"))
                    {
                        skinToApply = skin;
                    }
                }
            }
        }

        if(skinToApply != null)
        {
            foreach(MeshRenderer pMesh in meshesToPaint)
            {
                pMesh.material.mainTexture = skinToApply.textureToApply;
                pMesh.material.mainTextureScale = skinToApply.tileSize;
            }
        }
    }
}
