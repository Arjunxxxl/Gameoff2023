using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Animation Tag")]
    [SerializeField] private string STAND;
    [SerializeField] private string FALL;
    [SerializeField] private string CROUCH;
    [SerializeField] private string RUNNING;
    [SerializeField] private string DASH;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem dashEffect;

    [Space]
    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetUp()
    {
        PlayDashEffect(false);
    }

    #region Animations
    public void SetStandingAnimation(bool active)
    {
        anim.SetBool(STAND, active);
    }

    public void SetCrouchAnimation(bool active)
    {
        anim.SetBool(CROUCH, active);
    }

    public void SetFallingAnimation(bool isActive)
    {
        anim.SetBool(FALL, isActive);
    }

    public void SetDashAnimation(bool isActive)
    {
        anim.SetBool(DASH, isActive);
    }

    public void SetRunningAnimation(float val)
    {
        anim.SetFloat(RUNNING, val);
    }
    #endregion

    #region Particle Effect
    public void PlayDashEffect(bool play)
    {
        if(play)
        {
            dashEffect.Play();
        }
        else
        {
            dashEffect.Stop();
        }
    }
    #endregion
}
