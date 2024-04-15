using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDriver : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _speed = 10f;

    private void FixedUpdate()
    {
        _rigidBody.velocity = transform.forward * _speed;
    }
}
