# 🦸‍♂️ Unity + Google Gemini 1.5 Integration

This project demonstrates how to connect a Unity application to Google's Gemini 1.5 AI model to send text prompts and receive AI-generated responses in real-time. It includes the implementation of a custom Web API that handles the communication between Unity and the Gemini API, all while featuring a fun, interactive turtle that speaks to you through Gemini-AI.

## 📌 Overview

The goal is to allow Unity to send text prompts to **Gemini 1.5 Pro** and receive AI-generated responses in real-time. This is accomplished using a custom Web API that handles communication between Unity and the Gemini API. The interaction involves a super cool turtle that engages with users through AI-generated dialogue.

## 🧱 Project Components

- **Unity Project**: The frontend interface where users can enter prompts and see AI-generated responses from Gemini.
- **Web API**: The backend service that securely connects to Gemini Pro and sends back responses to Unity.
- **Google Gemini API**: Google’s large language model that generates the AI responses based on the input.
- **Google Compute Engine**: A cloud service (running on a VM with Ubuntu) that handles the AI generation and offloads heavy processing from the local machine.

## 🏗️ Our Process

- **Setting Up the Back-End**: The back-end was set up using **Google Cloud Compute Engine** with an **Ubuntu LTS** VM. The main task was to ensure fast, efficient AI response generation by connecting to the Google Gemini 1.5 API via a secure, custom Web API.
  
- **Connecting the VM to Unity**: The biggest hurdle was ensuring proper communication between Unity and the Google Cloud VM. We faced several challenges, especially in displaying the AI responses correctly in Unity. The solution involved sending a request to the VM and receiving the raw response directly, bypassing the complex JSON parsing. Unity was quite tricky in terms of managing external API responses, but eventually, we found a way to present the results cleanly.

- **Handling Insecure Connections**: One of the earlier challenges involved dealing with insecure connections between Unity and the web server (Google Cloud). We eventually resolved this by configuring Unity to accept connections that didn’t require HTTPS, given the context of a local development environment, but we recommend securing connections for production use.

- **VM Deployment on Google Cloud**: We initially ran the back-end using **NGROK** for secure tunneling, but that wasn’t ideal for long-term deployment. We successfully moved everything over to Google Cloud and set up a static external IP to allow Unity to communicate directly with the VM without needing to rely on NGROK.

- **Dealing with Authentication and API Issues**: After setting up the VM and connecting it to Gemini, we ran into issues with API authentication and scopes. Specifically, Google Cloud had strict permission requirements, and at one point, we hit a 403 error due to insufficient authentication scopes. After a lot of troubleshooting and adjusting API credentials, the server finally started accepting requests.

## 🚀 Getting Started

Follow the steps below to set up this project on your local machine or a cloud server like **Google Cloud**.

### 1. Clone the Repository

```bash
git clone https://github.com/your-team/repo-name.git
cd repo-name
git clone https://github.com/evanmorg/AlohaData-Team8
cd AlohaData-Team8.

