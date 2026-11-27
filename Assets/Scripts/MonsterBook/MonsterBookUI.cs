using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MonsterBookUI : MonoBehaviour
{
    #region SETTINGS
    [Header("Tabs")]
    [SerializeField] private BookTab monsterTab;
    [SerializeField] private BookTab tutorialTab;

    [Header("Pages")]
    [SerializeField] private GameObject monsterPage;
    [SerializeField] private GameObject tutorialPage;

    [Header("Monster Data")]
    [SerializeField] private MonsterBookEntry[] entries;
    [SerializeField] private Image portrait;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private GameObject[] dangerStars;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private BookEventRow[] eventRows;

    [Header("Monster Pager")]
    [SerializeField] private GameObject prevBtn;
    [SerializeField] private GameObject nextBtn;
    [SerializeField] private TMP_Text pageText;
    #endregion

    #region STATE
    private enum BookPage { Monsters, Tutorial }
    private BookPage currentPage = BookPage.Monsters;
    private int currentPageIndex = 0;
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

        if (monsters) RefreshMonsterPage();
    }
    #endregion

    #region MONSTER PAGE
    void RefreshMonsterPage()
    {
        if (entries == null || entries.Length == 0) return;

        MonsterBookEntry entry = entries[currentPageIndex];
        if (entry == null) return;
        MonsterConfig monster = entry.monster;

        if (monster != null)
        {
            portrait.sprite = monster.sprDetail;
            codeText.text = "[ID] " + monster.code;
        }

        for (int i = 0; i < dangerStars.Length; i++)
        {
            dangerStars[i].SetActive(i < entry.danger);
        }

        descriptionText.text = entry.background;

        for (int i = 0; i < eventRows.Length; i++)
        {
            bool hasEvent = entry.events != null && i < entry.events.Length;

            if (hasEvent)
                eventRows[i].Show(entry.events[i], GetUnlockMask(entry, i));
            else
                eventRows[i].Hide();
        }

        RefreshPager();
    }


    int GetBehaviourKey(MonsterBookEntry entry, int eventIndex)
    {
        if (entry == null || entry.monster == null) return -1;
        MonsterConfig monster = entry.monster;
        if (monster.behaviours == null) return -1;

        return (int)monster.behaviours[eventIndex].behaviour;
    }

    int GetUnlockMask(MonsterBookEntry entry, int eventIndex)
    {
        int key = GetBehaviourKey(entry, eventIndex);
        if (key < 0) return 0;

        if (SaveDataContainer.Instance == null) return 0;
        SaveData data = SaveDataContainer.Instance.CurrentData;
        if (data == null) return 0; 

        return MonsterBookSave.GetMask(data, key);
    }
    #endregion

    #region INPUT
    private void Update()
    {
        if (!BookToggle.IsOpen) return;
        if (currentPage != BookPage.Monsters) return;
        if (Keyboard.current == null) return;

        if (Keyboard.current.dKey.wasPressedThisFrame)
            NextPage();

        if (Keyboard.current.aKey.wasPressedThisFrame)
            PrevPage();
    }
    #endregion

    #region PAGER
    public void NextPage()
    {
        if (entries == null || entries.Length == 0) return;

        currentPageIndex = (currentPageIndex + 1) % entries.Length;
        RefreshMonsterPage();
        //RefreshPager();
    }
    public void PrevPage()
    {
        if (entries == null || entries.Length == 0) return;

        currentPageIndex = (currentPageIndex - 1 + entries.Length) % entries.Length;
        RefreshMonsterPage();
        //RefreshPager();
    }

    void RefreshPager()
    {
        int total = (entries != null) ? entries.Length : 0;

        prevBtn.SetActive(total > 1);
        nextBtn.SetActive(total > 1);

        pageText.text = (currentPageIndex + 1) + "/" + total;
    }
    #endregion
}
