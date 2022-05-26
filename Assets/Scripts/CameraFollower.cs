using UnityEngine;
using Cinemachine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField]
    private GameObject pivot;

    [SerializeField]
    private float RotationSpeed = 50.0f;

    [SerializeField]
    private float TranslationSpeed = 50.0f;

    [SerializeField]
    private float MinHeight = -100.0f;

    [SerializeField]
    private float MaxHeight = 100.0f;

    [SerializeField]
    private float DragRotationSpeed = 100.0f;

    private Vector3 DragOrigin = Vector3.zero;

    [SerializeField]
    private CinemachineVirtualCamera TopDownCamera;

    private void OnEnable()
    {
        CameraSwitcher.SwitchCamera(TopDownCamera);
    }

    private void OnDisable()
    {

        CameraSwitcher.Unregister(TopDownCamera);
    }

    private void Start()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    DragRotationSpeed = 1;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical")))
        {
            transform.RotateAround(pivot.transform.position, Vector3.up, -Input.GetAxis("Horizontal") * RotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + Input.GetAxis("Vertical") * TranslationSpeed * Time.deltaTime , MinHeight, MaxHeight), transform.position.z);
        }

        transform.LookAt(pivot.transform);

        //Drag to move around
        if (!Input.GetMouseButton(1))
        {
            return;
        }


        float MouseX = Mathf.Abs(Input.GetAxis("Mouse X"));
        float MouseY = Mathf.Abs(Input.GetAxis("Mouse Y"));
        if(MouseX == 0 && MouseY == 0)
        {
            return;
        }
        // left/right and up/down camera rotation
        if (MouseY > MouseX)
        {
            pivot.transform.RotateAround(-TopDownCamera.transform.right, Input.GetAxis("Mouse Y") * DragRotationSpeed * Time.deltaTime);
            //pivot.transform.eulerAngles = new Vector3(transform.eulerAngles.x - (Input.GetAxis("Mouse Y") * DragRotationSpeed * Time.deltaTime), transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            pivot.transform.RotateAround(TopDownCamera.transform.up, Input.GetAxis("Mouse X") * DragRotationSpeed * Time.deltaTime);

            //pivot.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - (Input.GetAxis("Mouse X") * DragRotationSpeed * Time.deltaTime), transform.eulerAngles.z);
        }

    }
}
