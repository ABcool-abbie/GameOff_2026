using UnityEngine;
using UnityEngine.InputSystem;

public class BookToggle : MonoBehaviour
{
    #region SETTINGS
    [SerializeField] private GameObject book;
    #endregion

    #region STATE
    public static bool IsOpen { get; private set; }
    #endregion


    private void Start()
    {
        Close();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame) Toggle();
    }

    private void OnDestroy()
    {
        IsOpen = false;
    }


    #region OPEN / CLOSE
    public void Toggle()
    {
        if (IsOpen) Close();
        else Open();
    }

    public void Open()
    {
        book.SetActive(true);
        IsOpen = true;
    }

    public void Close()
    {
        book.SetActive(false);
        IsOpen = false;
    }
    #endregion
}
