using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cave;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private List<WeaponComponent> parts = null;

    public void AddPart(WeaponComponent c)
    {
        if (parts == null)
        {
            parts = new List<WeaponComponent>();
        }
        parts.Add(c);
    }

    public void DoDamage(Damage damage, iDamageable other)
    {
        other.TakeDamage(damage);
    }

}
