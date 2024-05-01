# A README to write the summary of the progress.

## Saksham Jain

- Before 18th March: Have been working on the Unity project creating the scene. Took models created by Jordan and Nishanth for the scene. Worked with Weibing to test it before the demo.
- After 18th March to April 8: Have been working on getting the Grab objets working. Asked the other team for any help and got a resource from Nishanth being used by the other team. Used the provided XR Integration Toolkit to implement but that did not work. Worked through a lot of youtube tutorials and tried a lot of different methods before pivoting to Oculus Integration SDK and finally using Meta Integration SDK. Needed to buy a new Skeleton Model from the Unity Asset Store and implemented Controller and Hand Grab on new Skeleton Model.
- From April 8 - 14: Successfully implemented Hand Grab which wasn't working after packaging the app for Meta Quest.
- Implemented a reset button to move the bones back to their initial state by just clicking one button.
- Implemented a speech-to-text functionality using WhisperGPT and used ChatGPTManager made by Nishanth to send API request to ChatGPT. Jordan also worked on this but was encountering bugs so started side-by-side but he was able to fix the bugs!
- Created a Canvas to display the user question and the ChatGPT response in front of the user.
- Implemented a text-to-speech functionality as well (as a backup to Jordan's implementation) that doesn't use OpenAI and instead uses the Meta SDK. Also added functionality to pause, resume, and stop the voice.
- Implemented the POST request to our discussion forum on our website with Jordan and Weibing. User can speak their post in the VR and then we run Speech-to-text and send a POST request.
- Looked at the possibility of implementing lip sync in the skeleton jaw.
- Debugged the build on the Meta Quest not working for OpenAI.

## Weibing Wang
- Before 18th March: Focused primarily on completing and supplementing our project's website and its deployment. Worked alongside Saksham Jain on modifying and deploying our Unity project to the Meta Quest 3, contributing significantly to both the development and operational phases.
- After 18th March: Collaborated closely with Saksham Jain to finalize the deployment of our application to the Meta Quest 3. A key achievement during this period was the implementation of a draggable model feature, which enhanced user interaction within our virtual environment. Conducted comprehensive testing with Saksham Jain to ensure the seamless functionality and user experience of our application on the Meta Quest 3.
- From April 8 Onwards: Focused on the expansion of 3D models within our project, adding more depth and variety to our virtual environment.
- Initiated the integration of external GPT capabilities into our project, aiming to enhance the interactive experience through advanced AI-driven interactions.
- Recent Updates: Successfully implemented an interactive API through AWS, enabling robust database interactions. Utilized Netlify for web deployment, achieving a fully functional deployment of our website. Additionally, I have fully implemented interactive quizzes and forums on the website, significantly enhancing user engagement and providing dynamic learning tools. These features allow for real-time user interactions and feedback, fostering a more interactive and educational experience.

## Jordan Alper
- Before March 18th: Created models for the hospital room scene, most notable model is heart monitor. Applied textures to models and exported them to unity files.
- After March 18th: Worked on 3D room created by Saksham and Weibing to get interactable skeleton to work. Experimented with virtual hands, as well as different methods. Eventually, my teammates switched to a different VR method, away from XR to OXR. Continuously tested functionality to test if it was working correctly.
- From April 8th: Focusing on implementing talking function and integrating OpenAI into that.
- Implemented voice recognition and combined it with Nishanth's OpenAI request to get a response from AI.
- Implemented text-to-speech functionality (working out bugs still) that will automatically take AI response and use whisperGPT to convert the text response to an audio file. This audio clip then automatically gets played. This functionality was eventually replaced with a more efficient one, Wit.AI.
- Implemented functionality to stop the AI voice response, instead of pausing and playing it. Also reworking open ai code slightly to be more efficient and worked out some bugs.
- Implemented the POST request from the VR environment to the website forum with Weibing and Saksham, so that you can speak a discussion post and it posts it to the forum.
- Tried to fix bugs for deployment to Metaquest but was unsuccessful.
- Formatted, using CSS and Javascript in React, some components of the website like the About Us page and the Quiz Page. Layout and styling was not consitent so I fixed it.

## Nishanth Naik
- Before March 18th: Focused primarily on what models we could use for our Unity project. Worked with Saksham to explore models and scenes. Worked on splitting of the models to make them their own individual parts. Tested with Saksham on how it would look in our enviornment.
- After 18th March: Finding resources and reading packages to figure out the best way to Grab objects. Provided Saksham with the resources on which package to use to try to have the objects being Grabbable.
- From April 8th: Going to start working on the backend to get prompts and questions up to enhance the learning experience and interactivity.
- Integrated ChatGPT into our Unity project where users can ask any questions to ChatGPT using voice and gets the answer back in the VR enviornment in the form of text.

## Yuyang Liu
- Before March 18: Modeled the ward scene, created a basic hospital low profile and hospital exterior scene build and exported it as a unity file. Used the model created by Jordan and Nishanth for the scene.
- After March 18th Continued to modify the hospital floor plan model, but confirmed through communication to focus on an initial interactive room to complete the interaction between the models. Continued to test for functionality. Learned how to capture from YouTube and reviewed resource packs that needed to be prepared.
- Starting April 8: Focused on expanding the website functionality in the project with Weibin to add variety and comprehensiveness to the virtual environment learning. The focus will be on designing and developing quizzes, organizing them, and uploading them on your own. There are also student communication platforms for each topic category, which have been created to create a more helpful and immersive learning experience.
