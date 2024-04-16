A readme for Holos team 1 for CS639 repository

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
