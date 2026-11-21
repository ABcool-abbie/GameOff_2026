using UnityEngine;
using UnityEngine.UI;

public class CamOverheat : MonoBehaviour
{
    [SerializeField] private MonitorManager monitorManager;
    [SerializeField] private NightVisionController nightVision;

    [SerializeField] private float workingTime = 10f;
    [SerializeField] private float cdTime = 15f;
    [SerializeField, Range(0f, 1f)] private float resumeAt = 0.3f;

    [SerializeField] private Slider heatSlider;
    [SerializeField] private Image fillImage;

    [SerializeField] private MonitorLamp heatSliderFill;
    [SerializeField] private float overheatBlink = 0.3f;
    [SerializeField] private Color overheatColor = Color.red;
    [SerializeField] private Color coolColor = Color.white;


    private float[] heat;
    private bool[] overheated;

    public bool IsOverheated(int index) => overheated[index];
    public float GetHeat(int index) => heat[index];



    private void Awake()
    {
        int count = nightVision.CameraCount;
        heat = new float[count];
        overheated = new bool[count];

        if (heatSliderFill == null && fillImage != null)
        {
            heatSliderFill = fillImage.GetComponent<MonitorLamp>();
        }
    }

    private void Update()
    {
        int current = monitorManager.CurrentCameraIndex;
        float dt = Time.deltaTime;

        for (int i = 0; i < heat.Length; i++)
        {
            bool active = nightVision.IsNightOn(i) && i == current;

            if (active)
            {
                heat[i] += dt / workingTime;
                if (heat[i] >= 1f)
                {
                    heat[i] = 1f;
                    overheated[i] = true;
                    nightVision.SetNight(i, false);
                }
            }
            else
            {
                heat[i] = Mathf.Max(0f, heat[i] - dt / cdTime);
                if (overheated[i] && heat[i] <= resumeAt)
                    overheated[i] = false;
            }
        }

        UpdateSlider(current);
    }

    private void UpdateSlider(int index)
    {
        if (index < 0 || heatSlider == null) return;
        heatSlider.value = heat[index];

        if (heatSliderFill == null) return;

        if (overheated[index])
        {
            float cooled = Mathf.InverseLerp(1f, resumeAt, heat[index]);
            Color c = Color.Lerp(overheatColor, coolColor, cooled);
            heatSliderFill.SetBlink(c, overheatBlink);
        }
        else
        {
            heatSliderFill.SetSteady(coolColor);
        }
    }
}
