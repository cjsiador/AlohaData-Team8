using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    public GameObject speechBubbleObj;
    private string SuggestionString;
    public TMP_Text suggestionText;

    public string suggestionString
    {
        get {return SuggestionString;}
        set 
        {
            SuggestionString = value; 
            suggestionText.text = SuggestionString;        
        }
    }
}