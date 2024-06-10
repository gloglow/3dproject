using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Melee : Attack
{
    protected BoxCollider atkRange;

    protected void Awake()
    {
        atkRange = GetComponent<BoxCollider>();
    }

    public void Attack()
    {
        atkRange.enabled = true;
    }

    public void EndAttack()
    {
        atkRange.enabled = false;
    }
}
