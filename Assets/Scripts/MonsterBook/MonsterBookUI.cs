using UnityEngine;
public class MonsterBookUI : MonoBehaviour
{
    #region SETTINGS
    [Header("Tabs")]
    [SerializeField] private BookTab monsterTab;
    [SerializeField] private BookTab tutorialTab;

    [Header("Pages")]
    [SerializeField] private GameObject monsterPage;
    [SerializeField] private GameObject tutorialPage;
    #endregion

    #region STATE
    private enum BookPage { Monsters, Tutorial }
    private BookPage currentPage = BookPage.Monsters;
    #endregion


    void OnEnable() => ShowPage(currentPage);


    #region TABS
    // Tab buttons
    public void ShowMonsters() => ShowPage(BookPage.Monsters);
    public void ShowTutorial() => ShowPage(BookPage.Tutorial);

    void ShowPage(BookPage page)
    {
        currentPage = page;
        bool monsters = page == BookPage.Monsters;

        monsterPage.SetActive(monsters);
        tutorialPage.SetActive(!monsters);

        monsterTab.SetSelected(monsters);
        tutorialTab.SetSelected(!monsters);
    }
    #endregion
}
