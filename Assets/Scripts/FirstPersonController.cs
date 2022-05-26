using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GravityBody))]
public class FirstPersonController : MonoBehaviour
{
	[SerializeField]
	private float mouseSensitivityX = 1;

	[SerializeField]
	private float mouseSensitivityY = 1;

	[SerializeField]
	private Transform cameraTransform;

	// public vars
	private float verticalLookRotation = 0.0f;

    private void OnEnable()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

    private void OnDisable()
    {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
    }

    void Update()
	{
		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;
	}
}