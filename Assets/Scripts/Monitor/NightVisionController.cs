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
    [SerializeField] private CamOverheat overheat;
    #endregion


    #region STATS
    private bool[] nightOn;
    private int nightMask;
    public event Action<int, bool> NightChanged;

    public bool IsNightOn(int camIndex) => nightOn[camIndex];
    public int CameraCount => securityCameras.Length;
    #endregion

    private void Awake()
    {
        nightOn = new bool[securityCameras.Length];

        int layer = LayerMask.NameToLayer(nightLayerName);
        if (layer < 0) return;
        else
            nightMask = 1 << layer;

        for (int i = 0; i < securityCameras.Length; i++)
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

    private void Toggle(int index)
    {
        if (!nightOn[index] && overheat != null && overheat.IsOverheated(index)) return;

        SetNight(index, !nightOn[index]);
    }

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
            bool show = nightOn[i] && i == current;

            if (show)
                securityCameras[i].cullingMask |= nightMask;
            else
                securityCameras[i].cullingMask &= ~nightMask;
        }
    }

    //private void ApplyLayer(int index)
    //{
    //    Camera cam = securityCameras[index];
    //    if (nightOn[index])
    //        cam.cullingMask |= nightMask;
    //    else
    //        cam.cullingMask &= ~nightMask;
    //}

    public void SetNight(int index, bool on)
    {
        if (nightOn[index] == on) return;

        nightOn[index] = on;
        RefreshLayers();
        RefreshMainView();
        NightChanged?.Invoke(index, on);
    }
}