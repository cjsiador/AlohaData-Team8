import google.generativeai as genai

# 🔑 Replace with your actual Gemini API key
API_KEY = "AIzaSyAkAQSpayk-vkq7RJ6RlJvUhcFh4DP6KbY"

# Configure the client
genai.configure(api_key=API_KEY)

# Just listing the models (optional)
print("Fetching available models...\n")
models = genai.list_models()

print("Available Models:")
for model in models:
    print(f"- {model.name}")

# ✅ Use a supported model from the list
model_name = "models/gemini-1.5-pro-latest"
print(f"\nUsing model: {model_name}")

# Initialize the model
model = genai.GenerativeModel(model_name)

# Send a prompt
prompt = "What is the capital of France?"
print(f"\nSending prompt: {prompt}")
response = model.generate_content(prompt)

# Display response
print("\nResponse:")
print(response.text)

