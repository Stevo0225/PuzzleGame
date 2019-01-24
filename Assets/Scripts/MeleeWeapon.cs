using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour
{
    public int damage = -100;

    void OnTriggerEnter(Collider other)
    {
        if( other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Health>().AddjustCurrentHealth(damage);
        }
    }

}
