using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRunning(float val)
    {
        anim.SetFloat("running", val);
    }

    public void SetFire(bool fire)
    {
        anim.SetBool("no fire", !fire);
        anim.SetBool("fire", fire);
    }
}
