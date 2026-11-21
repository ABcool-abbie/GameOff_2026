using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MonitorLamp : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float offAlpha = 0.2f;

    private Image image;
    private Color color = Color.white;
    private float interval;
    private float timer;
    private bool lit = true;

    [ContextMenu("Test Blink")]
    private void TestBlink() => SetBlink(Color.red, 0.2f);

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if (interval > 0f)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                lit = !lit;
            }
        }

        Color c = color;
        c.a = lit ? 1f : offAlpha;
        image.color = c;
    }

    public void SetSteady(Color c)
    {
        color = c;
        interval = 0f;
        lit = true;
    }

    public void SetBlink(Color c, float blinkInterval)
    {
        color = c;
        interval = Mathf.Max(0.03f, blinkInterval);
    }

}
