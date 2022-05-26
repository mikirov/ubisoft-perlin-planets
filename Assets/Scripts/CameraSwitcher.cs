using System.Collections;
using System.Collections.Generic;
using Cinemachine;
public static class CameraSwitcher
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    public static CinemachineVirtualCamera ActiveCamera = null;

    public static void Register(CinemachineVirtualCamera camera)
    {
        cameras.Add(camera);
    }

    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return ActiveCamera == camera;
    }

    public static void SwitchCamera(CinemachineVirtualCamera camera)
    {
        camera.Priority = 10;
        ActiveCamera = camera;

        foreach(CinemachineVirtualCamera c in cameras)
        {
            if(c != camera && c.Priority != 0)
            {
                c.Priority = 0;
            }
        }
    }

    public static void Unregister(CinemachineVirtualCamera camera)
    {
        if (cameras.Contains(camera))
        {
            cameras.Remove(camera);
        }
    }
}
