using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIItemEvent : MonoBehaviour
{
    [SerializeField] Image imgIcon;
    [SerializeField] TextMeshProUGUI txtMessage;

    internal void Refresh(string code, int idxBehaviour)
    {
        MonsterConfig config = null;
        foreach (var cfg in GameConfigContainer.Instance.MonsterConfigs)
        {
            if (cfg.code == code)
            {
                config = cfg;
                break;
            }
        }
        Debug.Assert(config != null);

        imgIcon.sprite = config.bookEntry.events[idxBehaviour].icon;
        txtMessage.text = config.bookEntry.events[idxBehaviour].appearance;
    }
}