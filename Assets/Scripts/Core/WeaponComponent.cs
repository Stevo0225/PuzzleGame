using UnityEngine;
using System.Collections;
using Cave;

public class WeaponComponent : MonoBehaviour
{
    private Damage damage;
    private Weapon weapon;

    private void Awake()
    {
        weapon = transform.root.GetComponentInChildren<Weapon>();
        if (weapon)
        {
            weapon.AddPart(this);
        }
        else
        {
            Debug.LogError("No Connected Weapon : "+gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject!= null)
            DoDamage(other.gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null)
            DoDamage(collision.gameObject);
    }

    private void DoDamage(GameObject other)
    {
        iDamageable o = other.transform.root.GetComponentInChildren<iDamageable>();
        if (o != null)
        {
            weapon.DoDamage(damage, o);
        }
      
    }

}
