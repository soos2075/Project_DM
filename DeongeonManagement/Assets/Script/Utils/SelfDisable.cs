using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisable : MonoBehaviour
{
    public float disableTime = 2;
    void Start()
    {
        Invoke("Disable", disableTime);
    }



    void Disable()
    {
        Managers.Resource.Destroy(this.gameObject);
    }

}
