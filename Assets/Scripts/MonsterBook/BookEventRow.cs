using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookEventRow : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private Image eventIcon;
    [SerializeField] private TMP_Text appearanceText;
    [SerializeField] private TMP_Text[] solutionTexts;
    #endregion


    #region DISPLAY
    public void Show(BookEventEntry data, int unlockMask)
    {
        gameObject.SetActive(true);
        eventIcon.sprite = data.icon;
        eventIcon.enabled = (data.icon != null);
        appearanceText.text = data.appearance;

        for (int i = 0; i < solutionTexts.Length; i++)
        {
            bool unlocked = (unlockMask & (1 << i)) != 0;

            string line = unlocked
                ? GetLine(data.solutionLines, i)
                : GetLine(data.lockedLines, i);

            solutionTexts[i].text = line;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private string GetLine(string[] lines, int index)
    {
        if (lines == null || index >= lines.Length)
            return "";

        return lines[index];
    }
    #endregion
}
