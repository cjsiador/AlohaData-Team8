from flask import Flask, request, jsonify
import google.generativeai as genai
import os

app = Flask(__name__)

# Set up Gemini API
genai.configure(api_key="AIzaSyAkAQSpayk-vkq7RJ6RlJvUhcFh4DP6KbY")  # Replace with your actual API key
model = genai.GenerativeModel("models/gemini-1.5-pro-latest")

@app.route('/ask', methods=['POST'])
def ask_gemini():
    data = request.get_json()
    prompt = data.get('prompt', '')

    if not prompt:
        return jsonify({'error': 'No prompt provided'}), 400

    try:
        response = model.generate_content(prompt)
        return jsonify({'response': response.text})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000)  # Listens on all interfaces
