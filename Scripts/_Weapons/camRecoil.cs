using UnityEngine;
using System.Collections;

public class camRecoil : MonoBehaviour {

    private float recoil = 0.0f;
    private float maxRecoil_x = -20f;
    private float maxRecoil_y = 20f;
    private float recoilSpeed = 2f;

    public void StartRecoil(float recoilParam, float maxRecoil_xParam, float recoilSpeedParam)
    {
        recoil = recoilParam;
        maxRecoil_x = maxRecoil_xParam;
        recoilSpeed = recoilSpeedParam;
        maxRecoil_y = Random.Range(0, recoilParam);
    }

    void recoiling()
    {
        if (recoil > 0f)
        {
            Quaternion maxRecoil = Quaternion.Euler(-maxRecoil_x, maxRecoil_y, 0);         
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            recoil -= Time.deltaTime;
        }
        else {
            recoil = 0f;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * recoilSpeed / 2);
        }
    }

    void Update()
    {
        recoiling();
    }
}
