using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MonitorManager : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private GameObject overviewPanel;
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private RawImage mainView;
    [SerializeField] private RenderTexture[] cameraTextures;
    #endregion

    #region STATS
    private const int ViewCount = 5;
    private int currentView;

    public int CurrentView => currentView;
    public bool IsOverview => currentView == 0;
    public int CurrentCameraIndex => currentView - 1; 

    public event Action<int> ViewChanged;
    #endregion

    private void Start()
    {
        ShowView(0);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.dKey.wasPressedThisFrame)
            ShowView((currentView + 1) % ViewCount);

        if (keyboard.aKey.wasPressedThisFrame)
            ShowView((currentView - 1 + ViewCount) % ViewCount);
    }

    private void ShowView(int index)
    {
        currentView = index;

        bool isOverview = index == 0;
        overviewPanel.SetActive(isOverview);
        detailPanel.SetActive(!isOverview);

        if (!isOverview)
            mainView.texture = cameraTextures[index - 1];

        ViewChanged?.Invoke(currentView);
    }
}
