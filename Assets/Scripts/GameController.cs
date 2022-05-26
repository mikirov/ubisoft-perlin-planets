using System.Runtime.InteropServices;
using UnityEngine;

public enum NavigationMode
{
    TopDown,
    WalkingOnPlanet,
    Flying
}

public enum CameraMode
{
    FPS,
    TPS
}

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance { get { return _instance; } }

    public delegate void NavigationDelegate(NavigationMode mode);

    public delegate void CameraDelegate(CameraMode mode);

    public event NavigationDelegate OnNavigationChange;

    public event CameraDelegate OnCameraChange;

    public event CameraDelegate OnShuttleCameraChange;


    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private Transform OrbitingCamera;


    [DllImport("__Internal")]
    private static extern void SceneInitialized();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    private void Start()
    {
        GeneratePlanet(880713271);

#if UNITY_WEBGL == true && UNITY_EDITOR == false
    WebGLInput.captureAllKeyboardInput = false;

    SceneInitialized();
#endif
    }

    public void GeneratePlanet(int seed)
    {
        NFTController.Instance.GeneratePlanet(880713271);
        Player.transform.position = new Vector3(0, NFTController.Instance.GetMaxPoint(), 0);
        OrbitingCamera.position = new Vector3(0, 0, -NFTController.Instance.GetMaxPoint() - 200);
    }


    private NavigationMode _nagivationMode = NavigationMode.TopDown;
    private CameraMode _cameraMode = CameraMode.FPS;

    void UpdateNavigationMode()
    {
        NavigationMode oldMode = _nagivationMode;
        if(Input.GetMouseButtonDown(0) && _nagivationMode != NavigationMode.WalkingOnPlanet)
        {
            _nagivationMode = NavigationMode.WalkingOnPlanet;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && _nagivationMode == NavigationMode.WalkingOnPlanet)
        {
            _nagivationMode = NavigationMode.Flying;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && _nagivationMode == NavigationMode.Flying)
        {
            _nagivationMode = NavigationMode.TopDown;
        }

        if (oldMode != _nagivationMode && OnNavigationChange != null && OnNavigationChange.GetInvocationList().Length > 0)
        {
            OnNavigationChange(_nagivationMode);
        }
    }

    void UpdateCameraMode()
    {
        if(!Input.GetKeyDown(KeyCode.Q))
        {
            return;
        }

        _cameraMode = _cameraMode == CameraMode.FPS ? CameraMode.TPS : CameraMode.FPS;

        if (_nagivationMode == NavigationMode.WalkingOnPlanet && OnCameraChange != null && OnCameraChange.GetInvocationList().Length > 0)
        {
            OnCameraChange(_cameraMode);
        } else if (_nagivationMode == NavigationMode.Flying && OnCameraChange != null && OnCameraChange.GetInvocationList().Length > 0)
        {
            OnShuttleCameraChange(_cameraMode);
        }
    }

    void Update()
    {
        UpdateNavigationMode();
        if(_nagivationMode == NavigationMode.WalkingOnPlanet)
        {
            UpdateCameraMode();
        }
    }

}
