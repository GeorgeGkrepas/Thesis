using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardFunctions : MonoBehaviour
{
    
    TouchScreenKeyboard keyboard;

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void CloseKeyboard()
    {
        keyboard = null;
    }
}
