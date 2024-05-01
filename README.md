# A readme for Holos team 1 repository for CS639

#### Start with downloading the Unity project.

##### Test in VR through Unity
For more detailed instructions you may follow the steps here https://learn.unity.com/tutorial/vr-project-setup#65c511fbedbc2a263ed98728.
1.  Install the OpenXR Plugin for desktop testing:
From the top menu, select Edit > Project Settings, then select the XR Plug-in Management panel from the sidebar.
In the Windows, Mac, Linux tab, select OpenXR from the list of available Plug-in Providers to install that plug-in.
2.  Resolve any warnings by setting up an interaction profile.
After adding the OpenXR plugin, there may be a warning or error icon that appears next to the plugin Name. 
If there is a warning, click on the warning icon to open the OpenXR Project Validation window, which will tell you that you need to add an interaction profile for the device you’re using. 
Select the Edit button to open the OpenXR settings panel.
In the Windows, Mac, Linux tab, make sure the Oculus Touch Controller Profile appears in the list of Interaction Profiles, then enable all available OpenXR Feature Groups. 
There should no longer be any warnings in the XR Plugin Management panel. If there are, select them and follow the recommended steps to resolve them.
3.  Connect your device through the Quest Link Software: 
Make sure your device is plugged into your computer using a compatible cable. 
Make sure the Quest App is running and has successfully recognized your device.
4.  Test the project on your device:
Select Play in the Unity Editor and put on your headset

<strong>Instructions for connecting to ChatGPT: Make sure you have an OpenAI account.</strong>

After creating an account go to https://platform.openai.com/account/api-keys to create an API key.

To make requests to the OpenAI API, you need to use your API key and organization name (if applicable). To avoid exposing your API key in your Unity project, you can save it in your device's local storage.

To do this, follow these steps:

Create a folder called .openai in your home directory (e.g. C:User\UserName\ for Windows or ~\ for Linux or Mac)
Create a file called auth.json in the .openai folder
Add an api_key field and a organization field (if applicable) to the auth.json file and save it
Here is an example of what your auth.json file should look like:
{
    "api_key": "sk-...W6yi",
    "organization": "org-...L7W"
}

For more information you can visit this repository https://github.com/srcnalt/OpenAI-Unity?tab=readme-ov-file

# How does ChatGPTManager.cs works.

-Class and Event Definition

ChatGPTManager: This is the main class that manages interactions with the OpenAI API.

OnResponseEvent: A custom Unity event that takes a single string parameter. It's used to trigger actions when a response from the chat model is received.
Fields

OnResponse: An instance of OnResponseEvent which other components can subscribe to in order to react when the chat model produces a response.

openAI: An instance of OpenAIApi, which is presumably a class responsible for handling API requests to OpenAI.

messages: A list of ChatMessage objects, representing the conversation history. ChatMessage seems to be a custom class which isn't defined here but likely stores message content and the role of the message sender (user or AI).

-Methods

AskChatGPT(string newText): This method is called to send a new text message to the chat model.
It first creates a new ChatMessage for the user's input and adds it to the messages list.
A new CreateChatCompletionRequest object is constructed with the current conversation history. This request is then sent to the chat model specified (gpt-4).
The response from the API is checked to ensure it contains valid data. The first response message is then added to the conversation history and logged in the Unity console. The content of the response is also passed to any subscribers of the OnResponse event.

Start(): A Unity lifecycle method called before the first frame update. It's empty here, indicating no initialization behavior is needed at the start.

Update(): Another Unity lifecycle method called once per frame. It's also empty, indicating no per-frame behavior is necessary.

-Key Aspects of the Code

Asynchronous Communication: The AskChatGPT method is asynchronous (marked with async), which allows the Unity application to remain responsive while waiting for the network response from OpenAI's servers.

Event Handling: The use of UnityEvent allows other parts of the Unity application to react to chat responses without tightly coupling components, following an event-driven architecture.

Debugging and Feedback: The Debug.Log call in AskChatGPT helps in logging the response for debugging purposes.

#### Website Setup for Holos Team 1

To set up the website for the Holos project, follow these steps:

1. **Navigate to the project directory**:
```bash
cd holos1website
```

2. **Install dependencies**:
   Before running the website locally or building it for deployment, you need to install the necessary dependencies.
   Run the following command to install all the dependencies:
```bash
npm install
```

3. **Run the website locally**:
   To run the website locally, use the following command:
```bash
npm run dev
```
This will start the website on your local machine at http://localhost:3000.

4. **Build the website for deployment**:
   To build the website for deployment, use the following command:
```bash
npm run build
```
This will create a production-ready build of the website in the build folder.

5. **Deploy the website**:
   To deploy the website, you can use any hosting platform of your choice.
   For example, you can use Vercel or Netlify to deploy the website.
   Please refer to their documentation for specific instructions.

   Here is our website https://holos1med.com/

# What works in the project
- The skeleton can be interacted with the press of the buttons or just by using your hands in the VR enviornment.
- Asking questions to ChatGPT with voice from the Meta Quest.
- Uploading questions to the online forum through voice.
- The skeleton can be set back to its original position on the press of a button.
- In the website there are multiple quizs which can be taken to test your knowledge.

# What doesnt works
- Changing of voice for text to speech doesnt works.

# What would we work on next
- Adding text fields to the skeleton showing the parts of the skeleton.
- Miniming and zooming of the parts of the skeleton on grabbing them.
- Interactive human modelled bot in the enviornment to make the enviornment more imperssive.
- Interactive quiz modules in the VR enviornment which will be called from the website by using voice.



