using UnityEngine;

public class TankViewNotifier : MonoBehaviour
{
    [SerializeField] private MonitorManager monitorManager;
    [SerializeField] private TankState[] tanks;

    private void OnEnable() => monitorManager.ViewChanged += OnViewChanged;
    private void OnDisable() => monitorManager.ViewChanged -= OnViewChanged;


    private void OnViewChanged(int view)
    {
        int index = monitorManager.CurrentCameraIndex;
        for (int i = 0; i < tanks.Length; i++)
        {
            if (tanks[i] == null) continue;

            bool watched = (i == index);
            tanks[i].View(watched);
        }
    }
}
