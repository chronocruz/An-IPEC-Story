using UnityEngine;
using System.Collections;

public class wepSway : MonoBehaviour {

    public float tiltAngle = 4.0f;
    public float tiltAmount = 0.001f;
    public float smoothComplexity = 3.0f;

    void Update()
    {
        float tiltX = Input.GetAxis("Mouse X") * tiltAngle;
        float tiltY = Input.GetAxis("Mouse Y") * tiltAngle;
        Quaternion target = Quaternion.Euler(tiltY, tiltX, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * smoothComplexity);
    }

}
