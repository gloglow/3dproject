using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    public Transform target;
    public bool onChase = false;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (onChase)
            transform.position = Camera.main.WorldToScreenPoint(target.position);   
    }

    public void OnTarget(Transform crtTarget)
    {
        target = crtTarget;
        onChase = true;
    }

    public void OffTarget()
    {
        onChase = false;
        gameObject.SetActive(false);
    }
}
