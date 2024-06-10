using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // クォータービューのためのカメラセッティング
    [SerializeField] private Transform Target;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        transform.position = Target.position + offset;
    }
}
