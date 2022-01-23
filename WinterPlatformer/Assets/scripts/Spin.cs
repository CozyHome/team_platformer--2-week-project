using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteAlways]
public class Spin : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 axis;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.AngleAxis(speed * Time.deltaTime, axis) * transform.rotation;
    }
}
