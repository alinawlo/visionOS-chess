# Requirements:
- Apple Silicon CPU
- UnityHub + UnityEditor version 2022.3.14f1
- Xcode 15.1 beta 3 + visionOS simulator
- Blender (we used 4.0.0 but as long as Unity recignizes it its ok)
- Python 3.12.1 + cv2 + cvzone + socket library

# Instructions:
- Open the Project using UnityHub
- Press Play to use in UnityEditor
- Goto Build Settings and select visionOS as the target
- Select the Xcode beta version (or latest)
- Press Build & Run (This launches Xcode, builds in Xcode and launches the application in visionOS)

# Hand tracking:
- To enble the hand tracking run the python file in the 3D_Hand_Tracking folder (requires camera access)
- In the application press the button with the hand icon. (This connects to localhost:5032 so make sure its available or change the port in both applications)

- There are two gestures: 
- point: stretch index finger and tuck away the rest
- pinch: put thumb and index finger close together
- Note that the depth tracking of this module is very bad
