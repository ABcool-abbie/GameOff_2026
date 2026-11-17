using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button btnStartGame;
    [SerializeField] Button btnContinue;

    void Start()
    {
        btnContinue.gameObject.SetActive(SaveDataContainer.Instance.HasSaveData(0));

        btnStartGame.onClick.AddListener(OnBtnStart);
        btnContinue.onClick.AddListener(OnBtnContinue);
    }

    void OnDestroy()
    {
        btnStartGame.onClick.RemoveListener(OnBtnStart);
        btnContinue.onClick.RemoveListener(OnBtnContinue);
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
                title = "New Game",
                message = "Are you sure you want to start a new game?",
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
}
