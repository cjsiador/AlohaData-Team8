using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiAPI : MonoBehaviour
{
    private string apiKey = "AIzaSyAkAQSpayk-vkq7RJ6RlJvUhcFh4DP6KbY"; // Your API key here
    private string model = "models/gemini-pro"; // Or your specific model name

    void Start()
    {
        StartCoroutine(GenerateText());
    }

    IEnumerator GenerateText()
    {
        string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateText?key={apiKey}";
        string jsonData = "{\"prompt\":\"Hello, Gemini!\"}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Request failed: " + request.error);
        }
    }
}

