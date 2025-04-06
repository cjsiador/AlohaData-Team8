using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using System.Linq;
using Whisper.Samples;

public class GeminiAPI : MonoBehaviour
{
    public string apiKey = "YOUR_API_KEY";
    public MicrophoneDemo microphoneDemo;
    public TMP_InputField userInput;

    [TextAreaAttribute]
    public string userPrompt;

    [TextAreaAttribute]
    public string userRole;
    public TMP_Text responseText;
    public bool isTextAnimated = false;
    public float fadeDuration = 0.04f;
    public float delayBetweenChars = 0.02f;
    string endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
    public char trailingChar = '█';
    private string currentStr = "";
    private bool isBlinkingActive = false;
    public bool isBlinkingEnable = true;
    private Coroutine submitCoroutine;

    public List<HistoryPrompt> historyPrompt = new List<HistoryPrompt>();

    [TextAreaAttribute]
    public string historyPromptStr;

    void Update()
    {
        if (!isBlinkingActive && isBlinkingEnable)
        {
            StartCoroutine(BlinkingTrailing());
        }

        userPrompt = microphoneDemo.outputString;
    }

    public void AskGemini(string prompt)
    {
        if (submitCoroutine == null)
        {
            submitCoroutine = StartCoroutine(SendRequest(prompt));
        }
        else
        {
            StopCoroutine(submitCoroutine);
            submitCoroutine = null;
        }
    }

    public void AskGemini()
    {
        if (submitCoroutine == null)
        {
            submitCoroutine = StartCoroutine(SendRequest(microphoneDemo.outputString));
        }
        else
        {
            StopCoroutine(submitCoroutine);
            submitCoroutine = null;
        }
    }

    public void UserInput()
    {
        userPrompt = userInput.text;
    }

    public void UserPromptSubmit()
    {
        if (!string.IsNullOrEmpty(userPrompt))
        {
            AskGemini(userPrompt);
        }
    }

    IEnumerator SendRequest(string prompt)
    {
        historyPromptStr = EscapeJsonString(GetHistoryPrompt());

        string newLine = "\n";

        string insertPrompt = userRole + newLine + historyPromptStr + newLine + prompt;

        var jsonBody = "{\"contents\":[{\"parts\":[{\"text\":\"" + EscapeJsonString(insertPrompt) + "\"}]}]}";
        var request = new UnityWebRequest(endpoint + "?key=" + apiKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log("Response: " + json);

            // Deserialize JSON
            GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(json);

            // Extract and display the response text
            if (response.candidates != null && response.candidates.Length > 0)
            {
                string geminiText = response.candidates[0].content.parts[0].text;

                InsertHistoryPrompt(prompt, geminiText);

                Debug.Log("Gemini said " + geminiText);
                
                if (responseText != null && !isTextAnimated)
                {
                    yield return null;
                    responseText.text = geminiText;
                }
                
                if (responseText != null && isTextAnimated)
                {
                    yield return null;
                    StartCoroutine(AnimateText(geminiText));
                }
            }
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    string GetHistoryPrompt()
    {
        string promptResult = "";

        foreach (HistoryPrompt prompt in historyPrompt)
        {
            promptResult += "The " + prompt.role + "said: ";
            promptResult += prompt.content + "\n";
        }

        return promptResult;
    }

    void InsertHistoryPrompt(string userResponse, string geminiResponse)
    {
        HistoryPrompt User = new HistoryPrompt();
        HistoryPrompt Gemini = new HistoryPrompt();

        User.role = "Visitor";
        User.content = userResponse;

        Gemini.role = "Curator";
        Gemini.content = geminiResponse;

        historyPrompt.Add(User);
        historyPrompt.Add(Gemini);
    }

    string EscapeJsonString(string str)
    {
        return str
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    IEnumerator AnimateText(string responseStr)
    {
        responseText.text = "";
        currentStr = "";

        foreach(char c in responseStr)
        {
            yield return new WaitForSeconds(0.05f);

            isBlinkingEnable = false;

            currentStr += c;

            responseText.text = currentStr + trailingChar;

            if (c == ',')
            {
                yield return new WaitForSeconds(0.1f);
            }

            if (c == '.' || c == '?' || c == '!')
            {
                isBlinkingEnable = true;
                yield return new WaitForSeconds(1.20f);
            }
        }

        isBlinkingEnable = false;
    }

    IEnumerator BlinkingTrailing()
    {
        isBlinkingActive = true;

        responseText.text = currentStr + "<color=#FFFFFFFF>" + trailingChar + "</color>";
        yield return new WaitForSeconds(0.530f);
        responseText.text = currentStr + "<color=#FFFFFF00>" + trailingChar + "</color>";
        yield return new WaitForSeconds(0.530f);
        
        isBlinkingActive = false;
    }

    IEnumerator FadeCharacter(int index)
    {
        TMP_TextInfo textInfo = responseText.textInfo;

        if (index >= textInfo.characterCount)
            yield break;

        var charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible)
            yield break;

        int matIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;
        var colors = textInfo.meshInfo[matIndex].colors32;

        // Start with 0 alpha
        for (int i = 0; i < 4; i++)
            colors[vertexIndex + i].a = 0;

        responseText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            byte a = (byte)(alpha * 255);

            for (int i = 0; i < 4; i++)
                colors[vertexIndex + i].a = a;

            responseText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure fully visible at end
        for (int i = 0; i < 4; i++)
            colors[vertexIndex + i].a = 255;

        responseText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    void AnimateZoomEffectTMP()
    {
        responseText.ForceMeshUpdate();
        TMP_TextInfo textInfo = responseText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) 
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var origin = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = origin + new Vector3(0, 0, -5f);                
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            responseText.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    void AnimateWaveTMP()
    {
        responseText.ForceMeshUpdate();
        TMP_TextInfo textInfo = responseText.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) 
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var origin = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = origin + new Vector3(0, Mathf.Sin(Time.time * 1f + origin.x * 0.01f) * 10f, 0);                
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            responseText.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}

[System.Serializable]
public class GeminiPart
{
    public string text;
}

[System.Serializable]
public class GeminiContent
{
    public GeminiPart[] parts;
}

[System.Serializable]
public class GeminiCandidate
{
    public GeminiContent content;
}

[System.Serializable]
public class GeminiResponse
{
    public GeminiCandidate[] candidates;
}

[System.Serializable]
public class HistoryPrompt
{
    public string role;
    public string content;

    public void AddHistoryPrompt(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}
