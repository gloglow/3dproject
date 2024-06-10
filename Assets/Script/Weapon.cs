using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider atkArea;
    public TrailRenderer atkEffect;
    public float reach;

    public void Use()
    {
        if(type == Type.Melee)
        {
            StartCoroutine("Swing");
        }
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        atkArea.enabled = true;
        atkEffect.enabled = true;

        yield return new WaitForSeconds(0.1f);
        atkArea.enabled = false;

        yield return new WaitForSeconds(0.1f);
        atkEffect.enabled = false;
    }
}
