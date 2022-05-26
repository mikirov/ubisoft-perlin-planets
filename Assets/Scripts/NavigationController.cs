using UnityEngine;
using Cinemachine;

public class NavigationController : MonoBehaviour
{
    [SerializeField]
    private GameObject Player;

    [SerializeField]
    private GameObject TopDownCamera;

    [SerializeField]
    private GameObject Shuttle;


    void SetActiveRecursively(ref GameObject gameObject, bool state)
    {
        foreach (Behaviour childCompnent in gameObject.GetComponentsInChildren<Behaviour>())
        {
            childCompnent.enabled = state;
        }

        gameObject.SetActive(state);


    }

    // Start is called before the first frame update
    void Start()
    {
        if(GameController.Instance != null)
        {
            GameController.Instance.OnNavigationChange += HandleNavigationChange;
        }

        SetActiveRecursively(ref Player, false);
        SetActiveRecursively(ref Shuttle, false);
        SetActiveRecursively(ref TopDownCamera, true);

    }

    void HandleNavigationChange(NavigationMode mode)
    {
        SetActiveRecursively(ref TopDownCamera, mode == NavigationMode.TopDown);
        SetActiveRecursively(ref Shuttle, mode == NavigationMode.Flying);
        SetActiveRecursively(ref Player, mode == NavigationMode.WalkingOnPlanet);

        if(mode == NavigationMode.WalkingOnPlanet && NetworkManager.Instance)
        {
            NetworkManager.Instance.Connect();
        }
    }
}
