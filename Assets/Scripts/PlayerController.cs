using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
	[SerializeField]
	private float walkSpeed = 6;

	[SerializeField]
	private float mouseSensitivityX = 1;

	[SerializeField]
	private float mouseSensitivityY = 1;

	public Transform cameraTransform;

	// public vars
	private float verticalLookRotation = 0.0f;

	private float movementDirection;

	[SerializeField]
	private float jumpForce = 220;

	[SerializeField]
	private LayerMask groundedMask;

	// System vars
	private bool grounded = false;

	protected bool shouldJump;

	protected Rigidbody body;

	protected Animator animator;
	void Awake()
	{
		body = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
		cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;


		transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX);

		movementDirection = Input.GetAxis("Vertical");
		animator.SetBool("running", movementDirection != 0);

		if (Input.GetButtonDown("Jump") && grounded)
		{

			body.AddForce(transform.up * jumpForce);
			grounded = false;
			animator.SetBool("jumping", true);
			NetworkManager.Instance.Jump();
		}

		// Grounded check
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
		{
			grounded = true;
			animator.SetBool("jumping", false);
		}

	}

	//locally update position with rigid body
    private void FixedUpdate()
    {
		body.MovePosition(body.position + body.transform.forward * movementDirection * Time.fixedDeltaTime * walkSpeed);
    }

}
