using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

public class AskQuestion : MonoBehaviour
{
    public Button askButton; // Reference to the button
    public TMP_InputField questionInputField; // TMP Input field for the question
    public TMP_Text responseText; // TMP Text to display the response

    private string flaskServerUrl = "https://8846-35-232-223-5.ngrok-free.app/generate"; // Flask API URL

    void Start()
    {
        askButton.onClick.AddListener(OnAskButtonClick);
    }

    public void OnAskButtonClick()
    {
        string question = questionInputField.text;
        if (!string.IsNullOrEmpty(question))
        {
            StartCoroutine(SendQuestionToFlaskAPI(question));
        }
        else
        {
            responseText.text = "Please enter a question!";
        }
    }

    IEnumerator SendQuestionToFlaskAPI(string question)
    {
        string jsonData = "{\"prompt\": \"" + question + "\"}";

        using (UnityWebRequest webRequest = new UnityWebRequest(flaskServerUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;

                // Here we remove the unwanted characters from the response string
                string cleanedResponse = CleanJsonResponse(response);

                // Set the cleaned response to the UI text
                responseText.text = cleanedResponse;
            }
            else
            {
                responseText.text = "Error: " + webRequest.error;
            }
        }
    }

    // Function to clean up the response string by removing unwanted characters
    private string CleanJsonResponse(string response)
    {
        // Remove '{"response":' and the closing '}' at the end
        string cleanedResponse = response.Replace("{\"response\":\"", "").Replace("\"}", "");

        // Optionally, remove any special characters or escape sequences (like \n, \t, etc.)
        cleanedResponse = cleanedResponse.Replace("\\n", " ").Replace("\\t", " ").Trim();

        return cleanedResponse;
    }
}
