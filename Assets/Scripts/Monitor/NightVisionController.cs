using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NightVisionController : MonoBehaviour
{
    #region SETTING
    [SerializeField] private MonitorManager monitorManager;
    [SerializeField] private Camera[] securityCameras;
    [SerializeField] private RawImage mainView;
    [SerializeField] private Material nightMaterial;
    [SerializeField] private string nightLayerName = "NightVisionOnly";
    [SerializeField] private string darkLayerName = "DarkLayer";
    [SerializeField] private CamOverheat overheat;
    #endregion


    #region STATS
    private bool[] nightOn;
    private int nightMask;
    private int darkMask;
    public event Action<int, bool> NightChanged;

    public bool IsNightOn(int camIndex) => nightOn[camIndex];
    public int CameraCount => securityCameras.Length;
    #endregion

    private void Awake()
    {
        nightOn = new bool[securityCameras.Length];
        nightMask = GetMask(nightLayerName);
        darkMask = GetMask(darkLayerName);

        RefreshLayers();
    }

    private void OnEnable() => monitorManager.ViewChanged += OnViewChanged;
    private void OnDisable() => monitorManager.ViewChanged -= OnViewChanged;

    private void Update()
    {
        if (monitorManager.IsOverview) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.shiftKey.wasPressedThisFrame)
            Toggle(monitorManager.CurrentCameraIndex);
    }

    #region SETUP
    private void Toggle(int index)
    {
        if (!nightOn[index] && overheat != null && overheat.IsOverheated(index)) return;

        SetNight(index, !nightOn[index]);
    }

    public void SetNight(int index, bool on)
    {
        if (nightOn[index] == on) return;

        nightOn[index] = on;
        RefreshLayers();
        RefreshMainView();
        NightChanged?.Invoke(index, on);
    }
    #endregion

    #region VISUAL
    private void OnViewChanged(int view)
    {
        RefreshLayers();
        RefreshMainView();
    }


    private void RefreshMainView()
    {
        int index = monitorManager.CurrentCameraIndex;
        if (index < 0) return;
        mainView.material = nightOn[index] ? nightMaterial : null;
    }

    private void RefreshLayers()
    {
        int current = monitorManager.CurrentCameraIndex;

        for (int i = 0; i < securityCameras.Length; i++)
        {
            Camera cam = securityCameras[i];
            bool night = nightOn[i] && i == current;

            if (night)
            {
                cam.cullingMask |= nightMask;
                cam.cullingMask &= ~darkMask;
            }
            else
            {
                cam.cullingMask &= ~nightMask;
                cam.cullingMask |= darkMask;
            }
        }
    }

    private int GetMask(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer < 0)
        {
            return 0;
        }
        return 1 << layer;
    }

    //private void ApplyLayer(int index)
    //{
    //    Camera cam = securityCameras[index];
    //    if (nightOn[index])
    //        cam.cullingMask |= nightMask;
    //    else
    //        cam.cullingMask &= ~nightMask;
    //}
    #endregion
}