using UnityEngine;
using System.Collections;

public class headBobber : MonoBehaviour
{

    private float timer = 0.0f;
    public float bobbingSpeed = 0.15f;
    public float sprintMultiplier = 1.5f;
    public float bobbingAmount = 0.1f;
    float curSpeed = 0.18f;
    public float midpoint = 0.6f;

    void Awake()
    {
        curSpeed = bobbingSpeed;
    }

    void FixedUpdate()
    {
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float sprint = bobbingSpeed * sprintMultiplier;

        Vector3 cSharpConversion = transform.localPosition;

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else {
            waveslice = Mathf.Sin(timer);
            timer = timer + curSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer = timer - (Mathf.PI * 2);
            }
        }
        if (waveslice != 0)
        {
            float translateChange = waveslice * bobbingAmount;
            float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            cSharpConversion.y = midpoint + translateChange;
        }
        else {
            cSharpConversion.y = midpoint;
        }

        transform.localPosition = cSharpConversion;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            curSpeed = sprint;
        } else
        {
            curSpeed = bobbingSpeed;
        }
    }



}
