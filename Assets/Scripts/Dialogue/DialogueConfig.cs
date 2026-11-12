using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogConfig")]
public class DialogConfig : ScriptableObject
{
    [Serializable]
    public struct Dialog
    {
        public string Speaker;
        public string[] Speaches;
    }
    
    public Dialog[] dialogs;
}
