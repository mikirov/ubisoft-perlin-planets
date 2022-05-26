using Cinemachine;
using UnityEngine;

public class ShuttleCameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera firstPersonCamera;

    [SerializeField]
    private CinemachineVirtualCamera thirdPersonCamera;


    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnShuttleCameraChange += HandleCameraChange;
        }

    }

    private void OnEnable()
    {
        CameraSwitcher.Register(firstPersonCamera);
        CameraSwitcher.Register(thirdPersonCamera);
        CameraSwitcher.SwitchCamera(thirdPersonCamera);
    }

    private void OnDisable()
    {

        CameraSwitcher.Unregister(firstPersonCamera);
        CameraSwitcher.Unregister(thirdPersonCamera);
    }

    void HandleCameraChange(CameraMode mode)
    {
        if (mode == CameraMode.FPS)
        {
            if (!CameraSwitcher.IsActiveCamera(firstPersonCamera))
            {
                CameraSwitcher.SwitchCamera(firstPersonCamera);
            }
        }
        else if (mode == CameraMode.TPS)
        {
            if (!CameraSwitcher.IsActiveCamera(thirdPersonCamera))
            {
                CameraSwitcher.SwitchCamera(thirdPersonCamera);
            }
        }
    }
}
