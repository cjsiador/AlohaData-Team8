using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpeechSuggestions : MonoBehaviour
{
    public Transform centerTran;
    public List<SpeechBubble> speechBubbleList;
    public List<string> speechSuggestionString;
    public float radius = 5f;
    public Vector3 rotationSpeed;
    int indexCounter;

    bool isSpeechRotationReady = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstanceIntoCircle();
        ShuffleList(speechSuggestionString);
        AssignSpeechText();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
        
        for (int i = 0; i < speechBubbleList.Count; i++)
        {
            speechBubbleList[i].speechBubbleObj.transform.LookAt(centerTran);
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void InstanceIntoCircle()
    {
        if (speechBubbleList == null)
        {
            return;
        }

        for (int i = 0; i < speechBubbleList.Count; i++)
        {
            float angle = i * Mathf.PI * 2 / speechBubbleList.Count;
            speechBubbleList[i].speechBubbleObj.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    void AssignSpeechText()
    {
        for (int i = 0; i < speechBubbleList.Count; i++)
        {
            speechBubbleList[i].suggestionString = speechSuggestionString[i];
        }
    }
}