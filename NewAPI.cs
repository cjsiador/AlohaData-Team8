using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro; // TMP namespace for TMP_InputField and TMP_Text
using System.Collections.Generic;

public class NewAPI : MonoBehaviour
{
    public string apiUrl = "http://35.232.223.5:5000/generate"; // Flask or ngrok URL
    public TMP_InputField promptInputField;  // TMP UI Input Field
    public TMP_Text responseText;  // TMP UI Text to display the response

    private List<string> sessionHistory = new List<string>(); // Store conversation history

    public void OnGenerateButtonPressed()
    {
        string prompt = promptInputField.text;
        if (string.IsNullOrEmpty(prompt))
        {
            responseText.text = "Please enter a prompt.";
            return;
        }

        sessionHistory.Add("User: " + prompt); // Add user prompt to history
        StartCoroutine(SendRequest(prompt));
    }

    private IEnumerator SendRequest(string prompt)
    {
        // Include conversation history for context
        string conversationContext = string.Join("\n", sessionHistory);
        string json = "{\"prompt\": \"" + conversationContext + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest www = new UnityWebRequest(apiUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            responseText.text = "Error: " + www.error;
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            responseText.text = "Response: " + jsonResponse;

            // Add AI response to session history
            sessionHistory.Add("AI: " + jsonResponse);
        }
    }
}
