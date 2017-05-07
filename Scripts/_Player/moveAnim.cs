using UnityEngine;
using System.Collections;

public class moveAnim : MonoBehaviour {

    public Rigidbody rb;
    public Animation anim;
    public AnimationClip walk;
    public AnimationClip idle;
    public AnimationClip reload;
    public AnimationClip walkingReload;
    public AnimationClip sprint;
    public AnimationClip crouch;
    public AnimationClip crouchWalk;
    float sprintMult = 1;
    public bool pauseWalkOnAim = true;
    void Awake()
    {
        if(rb.GetComponent<characterControls>() != null)
        {
            sprintMult = rb.GetComponent<characterControls>().sprintMultiplier;
        }
    }

    void FixedUpdate()
    {
        characterControls cc = rb.GetComponent<characterControls>();
        if (crouch != null)
        {
            if (cc.body.height == cc.normalHeight)
            {
                if (reload != null)
                {
                    if (!anim.IsPlaying(reload.name) && !anim.IsPlaying(walkingReload.name))
                    {
                        if (rb.velocity.magnitude >= 0.1f)
                        {
                            float sprintSpeed = rb.GetComponent<characterControls>().speed * sprintMult;
                            if (rb.GetComponent<characterControls>().curSpeed < sprintSpeed)
                            {
                                if (pauseWalkOnAim)
                                {
                                    if (!Input.GetMouseButton(1))
                                    {
                                        anim.CrossFade(walk.name);
                                    }
                                } else
                                {
                                    anim.CrossFade(walk.name);
                                }
                            }
                            else
                            {
                                if (sprint != null)
                                {
                                    anim.CrossFade(sprint.name);
                                }
                            }
                        }
                        else
                        {
                            anim.CrossFade(idle.name);
                        }
                    }
                    else
                    {
                        if (reload != null)
                        {
                            if (anim.IsPlaying(reload.name) && rb.velocity.magnitude > 0.1f)
                            {
                                anim.CrossFade(walkingReload.name);
                            }

                            if (anim.IsPlaying(walkingReload.name) && rb.velocity.magnitude == 0)
                            {
                                anim.CrossFade(reload.name);
                            }
                        }
                    }
                }
                else
                {
                    if (rb.velocity.magnitude > 0.1f)
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            anim.CrossFade(sprint.name);
                        }
                        else
                        {
                            if (pauseWalkOnAim)
                            {
                                if (!Input.GetMouseButton(1))
                                {
                                    anim.CrossFade(walk.name);
                                }
                            }
                            else
                            {
                                anim.CrossFade(walk.name);
                            }
                        }
                    }
                    else
                    {
                        anim.CrossFade(idle.name);
                    }

                }
            }
            else
            {
                if (rb.velocity.magnitude <= 0.1f)
                {
                    anim.CrossFade(crouch.name);
                }
                else
                {
                    anim.CrossFade(crouchWalk.name);
                }
            }
        } else
        {
            if (reload != null)
            {
                if (!anim.IsPlaying(reload.name) && !anim.IsPlaying(walkingReload.name))
                {
                    if (rb.velocity.magnitude >= 0.1f)
                    {
                        float sprintSpeed = rb.GetComponent<characterControls>().speed * sprintMult;
                        if (rb.GetComponent<characterControls>().curSpeed < sprintSpeed)
                        {
                            if (pauseWalkOnAim)
                            {
                                if (!Input.GetMouseButton(1))
                                {
                                    anim.CrossFade(walk.name);
                                }
                            }
                            else
                            {
                                anim.CrossFade(walk.name);
                            }
                        }
                        else
                        {
                            if (sprint != null)
                            {
                                anim.CrossFade(sprint.name);
                            }
                        }
                    }
                    else
                    {
                        anim.CrossFade(idle.name);
                    }
                }
                else
                {
                    if (reload != null)
                    {
                        if (anim.IsPlaying(reload.name) && rb.velocity.magnitude > 0.1f)
                        {
                            anim.CrossFade(walkingReload.name);
                        }

                        if (anim.IsPlaying(walkingReload.name) && rb.velocity.magnitude == 0)
                        {
                            anim.CrossFade(reload.name);
                        }
                    }
                }
            }
            else
            {
                if (rb.velocity.magnitude > 0.1f)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        anim.CrossFade(sprint.name);
                    }
                    else
                    {
                        if (pauseWalkOnAim)
                        {
                            if (!Input.GetMouseButton(1))
                            {
                                anim.CrossFade(walk.name);
                            }
                        }
                        else
                        {
                            anim.CrossFade(walk.name);
                        }
                    }
                }
                else
                {
                    anim.CrossFade(idle.name);
                }

            }
        }

        if(pauseWalkOnAim)
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                if (rb.velocity.magnitude > 0)
                {
                    anim.CrossFade(walk.name);
                } else
                {
                    anim.CrossFade(idle.name);
                }              
            }
           
        }

        anim[walk.name].speed = rb.velocity.magnitude / rb.GetComponent<characterControls>().speed;
    }
}
