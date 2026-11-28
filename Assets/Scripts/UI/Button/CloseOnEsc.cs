using UnityEngine;
using UnityEngine.InputSystem;

public class CloseOnEsc : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            gameObject.SetActive(false);
    }
}
