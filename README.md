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
