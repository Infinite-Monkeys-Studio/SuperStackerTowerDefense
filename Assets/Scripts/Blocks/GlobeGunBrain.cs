using UnityEngine;
using UnityEngine.Assertions;

public class GlobeGunBrain : BaseTurret
{
    private AudioSource audioSrc;
    private Animator anim;
    
    private new void Start()
    {
        base.Start();
        anim = GetComponentInChildren<Animator>();
        audioSrc = GetComponent<AudioSource>();
        Assert.IsNotNull(anim);
    }

    
    private new void Update()
    {
        base.Update();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        currentTarget = FindNewTarget();    //Find a target to shoot

        if (currentTarget != null)  // if there is a target
        {
            if (Time.fixedTime - lastShotTime > 1 / rateOfFire) // check that the turret isn't reloading
            {
                attack(); //shoot her!
                audioSrc.Play();
            }
        } 
    }

    public void attack()
    {
        anim.SetTrigger("Fire");
        currentTarget.GetComponent<Health>().damage(damage);
        lastShotTime = Time.fixedTime;
    }

}
