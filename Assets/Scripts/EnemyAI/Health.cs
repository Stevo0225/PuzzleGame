using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

    public int maxHealth = 100;
    public int curHealth = 100;
    public float healthBarLength;
    public bool isAlive = true;


    void Start()
    {
        healthBarLength = Screen.width / 2;
    }


    void Update()
    {
        //AddjustCurrentHealth(0);
    }


    void OnGUI()
    {
        //GUI.Box(new Rect(10, 10, healthBarLength, 20), curHealth + "/" + maxHealth);
    }


    public void AddjustCurrentHealth(int adj)
    {
        if (!isAlive)
            return;

        if(adj < 0)
        {
            SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);
        }

        curHealth += adj;
        if (curHealth <= 0)
        {
            curHealth = 0;
            isAlive = false;
            SendMessage("OnDeath",SendMessageOptions.DontRequireReceiver);
        }

        if (curHealth > maxHealth)
        { 
            curHealth = maxHealth;
        }

        if (maxHealth < 1)
        { 
            maxHealth = 1;
        }

        healthBarLength = (Screen.width / 2) * (curHealth / (float)maxHealth);
    }
}
