using TMPro;
using UnityEngine;

public class BookTab : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private GameObject highlight;
    [SerializeField] private GameObject arrow; 
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color selectedTextColor = Color.black;
    [SerializeField] private Color normalTextColor = Color.white;
    #endregion

    public void SetSelected(bool selected)
    {
        if (highlight != null) highlight.SetActive(selected);
        if (arrow != null) arrow.SetActive(selected);
        if (label != null) label.color = selected ? selectedTextColor : normalTextColor;
    }
}
