using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

[RequireComponent(typeof(Image))]
public class CoolDownEffect : MonoBehaviour
{
    public bool CoolingDown = false;

    private float PlacementCoolDown;
    private float CoolDownBeginTime;
    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
        Assert.IsNotNull(image);
    }

    private void Update()
    {
       if(CoolingDown)
        {
            float time = Time.fixedTime - CoolDownBeginTime;
            float amount = time / PlacementCoolDown;
            image.fillAmount = amount;
        } else
        {
            image.fillAmount = 1;
        }
    }

    void FixedUpdate()
    {
        if(CoolingDown && Time.fixedTime - CoolDownBeginTime > PlacementCoolDown)
        {
            CoolingDown = false;
        }
    }

    public void StartCoolDown()
    {
        CoolingDown = true;
        CoolDownBeginTime = Time.fixedTime;
    }

    public void SetCoolDownTime(float time)
    {
        PlacementCoolDown = time;
    }
}
