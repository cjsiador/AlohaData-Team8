using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Net;
using System.Text;
using TMPro;
using System.Collections;

public class AskQuestion : MonoBehaviour
{
    public Button askButton; // Reference to the button
    public TMP_InputField questionInputField; // TMP Input field for the question
    public TMP_Text responseText; // TMP Text to display the response
    public TMP_Text loadingText; // Optional: A TMP Text to show when loading
    private string flaskServerUrl = "http://35.232.223.5:5000/generate"; // HTTP API URL (non-HTTPS)

    // Start is called before the first frame update
    void Start()
    {
        // Disable SSL/TLS validation for development/testing purposes
        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        // Add the listener for the button click event
        askButton.onClick.AddListener(OnAskButtonClick);

        // Optionally, hide the loading text at the start
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
        }
    }

    // Button click handler
    public void OnAskButtonClick()
    {
        string question = questionInputField.text;
        if (!string.IsNullOrEmpty(question))
        {
            // Show loading text and disable the button to avoid multiple requests
            SetLoadingState(true);
            StartCoroutine(SendQuestionToFlaskAPI(question));
        }
        else
        {
            responseText.text = "Please enter a question!";
        }
    }

    // Coroutine to send the question to Flask API and get the response
    IEnumerator SendQuestionToFlaskAPI(string question)
    {
        string jsonData = "{\"prompt\": \"" + question + "\"}";

        using (UnityWebRequest webRequest = new UnityWebRequest(flaskServerUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Wait until the web request is completed
            yield return webRequest.SendWebRequest();

            // Handle response based on result
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string response = webRequest.downloadHandler.text;

                // Clean the response (remove unwanted parts)
                string cleanedResponse = CleanJsonResponse(response);

                // Set the cleaned response to the UI text
                responseText.text = cleanedResponse;
            }
            else
            {
                // If there was an error, display the error message
                responseText.text = "Error: " + webRequest.error;
            }

            // Hide the loading text and enable the button
            SetLoadingState(false);
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

    // Function to toggle loading state UI elements (show/hide loading text)
    private void SetLoadingState(bool isLoading)
    {
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(isLoading);
        }

        // Disable/enable the button to avoid multiple clicks during loading
        askButton.interactable = !isLoading;
    }
}