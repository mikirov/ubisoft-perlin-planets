using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuttleController : MonoBehaviour
{
    [SerializeField]
    private Transform Planet;

    [SerializeField]
    private float HorizontalRotationSpeed = 100.0f;

    [SerializeField]
    private float VerticalTranslationSpeed = 100.0f;
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Planet.position, Vector3.up, Input.GetAxis("Horizontal") * HorizontalRotationSpeed * Time.deltaTime);
        transform.position += transform.up * Input.GetAxis("Vertical") * VerticalTranslationSpeed * Time.deltaTime;
    }
}
