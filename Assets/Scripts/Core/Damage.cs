using UnityEngine;
using System.Collections;



public class Damage
{
    private float amount = 1;

    public float Amount
    {
        get { return amount; }
        set { amount = Mathf.Abs(value); }
    }
}
