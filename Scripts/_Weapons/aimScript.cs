using UnityEngine;
using System.Collections;

public class aimScript : MonoBehaviour {

    public float defaultFOV = 60.0F;
    public float aimedFOV = 45.0F;
    public float smoothFOV = 10.0F;

    Vector2 aimSensitivity = new Vector2(1, 1);

    public int aimSensitivityDevisor = 2;    //The new aim Sensitivity will be set to the Mouse Sensitivity divided by this number.

    Vector2 defSensitivity = new Vector2(5, 5);

    public Vector3 hipPosition;
    public Vector3 aimPosition;
    public float smoothAim = 12.5F;
    public Camera cam;

    public GameObject[] objectsToHide;
    public Texture scopeTex;

    public Texture crossHair;

    bool fullyAimed = false;


    void Awake()
    {
        if (cam != null)
        {
            mouseLook ml = cam.GetComponent<mouseLook>();

            if (ml != null)
            {
                defSensitivity = ml.sensitivity;
            }
        }

        if (PlayerPrefs.HasKey("mouse"))
        {
            defSensitivity = new Vector2(PlayerPrefs.GetFloat("mouse"), PlayerPrefs.GetFloat("mouse"));
            aimSensitivity = new Vector2(PlayerPrefs.GetFloat("mouse") / aimSensitivityDevisor, PlayerPrefs.GetFloat("mouse") / aimSensitivityDevisor);
        } else
        {
            aimSensitivity = new Vector2(defSensitivity.x / aimSensitivityDevisor, defSensitivity.y / aimSensitivityDevisor);
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1) && !Input.GetKey(KeyCode.LeftShift))
        {
            
                transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * smoothAim);
                if (cam != null)
                {
                    mouseLook ml = cam.GetComponent<mouseLook>();

                    if (ml != null)
                    {
                        ml.sensitivity = aimSensitivity;
                    }
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, aimedFOV, Time.deltaTime * smoothFOV);
                }
            
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, hipPosition, Time.deltaTime * smoothAim);
            if (cam != null)
            {
                mouseLook ml = cam.GetComponent<mouseLook>();

                if (ml != null)
                {
                    ml.sensitivity = defSensitivity;
                }

                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFOV, Time.deltaTime * smoothFOV);
            }
        }

        if (transform.localPosition == aimPosition)
        {
            fullyAimed = true;
        } else
        {
            fullyAimed = false;
        }

        foreach(GameObject obj in objectsToHide)
        {
            obj.SetActive(!fullyAimed);
        }
    }

    void OnGUI()
    {
        if (scopeTex != null && fullyAimed)
        {
            Color c = Color.white;
            c.a = cam.fieldOfView / aimedFOV * 100;             
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), scopeTex, ScaleMode.StretchToFill);                            
        }

        if (crossHair != null && !Input.GetMouseButton(1))
        {
            Color c = Color.white;
            c.a = cam.fieldOfView / aimedFOV * 100;
            GUI.DrawTexture(new Rect(Screen.width / 2 - crossHair.width / 2, Screen.height / 2 - crossHair.height / 2, crossHair.width, crossHair.height), crossHair);
        }
    }

    }
