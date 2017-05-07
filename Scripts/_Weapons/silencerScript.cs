using UnityEngine;
using System.Collections;

public class silencerScript : MonoBehaviour {

    public gunScript weapon;
    public AudioClip silencedGunshot;
    public GameObject silencedMuzzleFlash;

    void OnEnable()
    {
        weapon.shootSFX = silencedGunshot;
        weapon.muzzleFlash = silencedMuzzleFlash;
    }
}
