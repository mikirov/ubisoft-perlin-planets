using UnityEngine;

public class NetworkedPlayerController : MonoBehaviour
{

	[SerializeField]
	private float jumpForce = 220;

	[SerializeField]
	private LayerMask groundedMask;

	// System vars
	private bool grounded = false;

	private bool shouldJump;

	private Rigidbody body;

	private Animator animator;

	private Vector3 lastPosition;
	private Vector3 newPosition;

	private Vector3 lastRotation;
	private Vector3 newRotation;


	void Awake()
	{
		body = GetComponent<Rigidbody>();
		animator = GetComponent<Animator>();
		lastPosition = transform.position;
		newPosition = transform.position;
	}

	public void SetShouldJump(bool value)
    {
		shouldJump = value;
    }

	public void SetPosition(Vector3 value)
    {
		lastPosition = newPosition;
		newPosition = value;
	}

	public void SetRotation(Vector3 rotation)
    {
		lastRotation = newRotation;
		newRotation = rotation;

	}

	void Update()
	{
		Vector3 velocity = new Vector3() ;
		transform.position = Vector3.SmoothDamp(lastPosition, newPosition, ref velocity, 1.0f);

		transform.eulerAngles = Vector3.SmoothDamp(lastPosition, newPosition, ref velocity, 1.0f);

		// Jump
		if (shouldJump && grounded)
		{
			body.AddForce(transform.up * jumpForce);
			grounded = false;
			animator.SetBool("jumping", true);
			shouldJump = false;
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

}
