using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Bullet : Attack
{
    public Vector3 dirVec;
    public float spd;
    public float duration;

    public void Shoot(Vector3 vec)
    {
        dirVec = vec;
        gameObject.SetActive(true);
        StartCoroutine(ShootMove());
        Invoke("Deactivate", duration);
    }

    public IEnumerator ShootMove()
    {
        while (true)
        {
            transform.position += dirVec * spd * Time.deltaTime;
            yield return null;
        }
    }

    public void Deactivate()
    {
        StopCoroutine(ShootMove());
        gameObject.SetActive(false);
    }
}
