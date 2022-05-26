using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
	[SerializeField]
	private GravityAttractor planet;
	
	Rigidbody body;

	public void SetPlanet(GravityAttractor planet)
    {
		this.planet = planet;
    }

	void Awake()
	{
		body = GetComponent<Rigidbody>();

		// Disable rigidbody gravity and rotation as this is simulated in GravityAttractor script
		body.useGravity = false;
		body.constraints = RigidbodyConstraints.FreezeRotation;
	}

	void FixedUpdate()
	{
		// Allow this body to be influenced by planet's gravi
		if(planet != null)
        {
			planet.Attract(body);
		}
	}
}