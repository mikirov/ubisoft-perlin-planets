using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{

    [SerializeField]
    private CinemachineVirtualCamera firstPersonCamera;

    [SerializeField]
    private CinemachineVirtualCamera thirdPersonCamera;

    private Transform originalCameraTransform;

    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnCameraChange += HandleCameraChange;
        }
        playerController = GetComponentInChildren<PlayerController>();
        originalCameraTransform = playerController.cameraTransform;

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
            if(!CameraSwitcher.IsActiveCamera(firstPersonCamera))
            {
                CameraSwitcher.SwitchCamera(firstPersonCamera);
                playerController.cameraTransform = firstPersonCamera.transform;
                
            }
        }
        else if (mode == CameraMode.TPS)
        {
            if (!CameraSwitcher.IsActiveCamera(thirdPersonCamera))
            {
                CameraSwitcher.SwitchCamera(thirdPersonCamera);
                playerController.cameraTransform = originalCameraTransform;
            }
        }

    }
}
