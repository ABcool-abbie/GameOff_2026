using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button btnStartGame;
    [SerializeField] Button btnContinue;
    [SerializeField] Button btnExit; //BBR: add buttons
    [SerializeField] Button btnBook;
    [SerializeField] GameObject bookPanel;

    private void Awake()
    {
        if (btnExit != null) 
            btnExit.onClick.AddListener(OnBtnExit);

        if (btnBook != null)
            btnBook.onClick.AddListener(OnBtnBook);

        bookPanel.SetActive(false);
    }

    void Start()
    {
        if (SaveDataContainer.Instance != null && SaveDataContainer.Instance.HasSaveData(0))
        {
            SaveDataContainer.Instance.LoadOrNewCurrentData(0, false);
        }


        btnContinue.gameObject.SetActive(SaveDataContainer.Instance.HasSaveData(0));

        btnStartGame.onClick.AddListener(OnBtnStart);
        btnContinue.onClick.AddListener(OnBtnContinue);
    }

    void OnDestroy()
    {
        btnStartGame.onClick.RemoveListener(OnBtnStart);
        btnContinue.onClick.RemoveListener(OnBtnContinue);
        btnExit.onClick.RemoveListener(OnBtnExit);
        btnBook.onClick.RemoveListener(OnBtnBook);
    }

    private void OnBtnStart()
    {
        void NewGame()
        {
            SaveDataContainer.Instance.LoadOrNewCurrentData(0, true);
            UILoading.Instance.Load(SceneNames.Office, null);
        }

        if (SaveDataContainer.Instance.HasSaveData(0))
        {
            UIConfirmDialog.Instance.Show(new UIConfirmDialog.Config
            {
                title = "New Shift",
                message = "Clock in as a new hire? \nYour current progress will be erased.",
                onConfirm = NewGame,
                onCancel = null,
            });
        }
        else
        {
            NewGame();
        }
    }

    private void OnBtnContinue()
    {
        SaveDataContainer.Instance.LoadOrNewCurrentData(0, false);
        UILoading.Instance.Load(SceneNames.Office, null);
    }

    //BBR: quit game
    private void OnBtnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    private void OnBtnBook()
    {
        bookPanel.SetActive(true);
    }
}
