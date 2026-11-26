using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region SETTINGS
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject arrow;
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color hoverTextColor = Color.black;
    [SerializeField] private Color normalTextColor = Color.white;
    #endregion

    #region STATE
    private Button button;
    #endregion

    #region UNITY
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        SetHover(false);
    }

    private void OnDisable()
    {
        SetHover(false);
    }
    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;
        SetHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHover(false);
    }

    public void SetHover(bool on)
    {
        if (highlight != null) highlight.SetActive(on);
        if (arrow != null) arrow.SetActive(on);
        if (label != null) label.color = on ? hoverTextColor : normalTextColor;
    }
}
