# UnityLabanotation

This is a Unity script that utilizes Labanotation to animate body parts in a 3D environment. It reads a JSON file containing Labanotation data, parses it, and uses it to rotate various body parts in the scene. The body parts that can be animated are the left and right arm and leg.
Features

- Parses a JSON file containing Labanotation data
- Uses the data to animate body parts in a 3D environment
- Utilizes IEnumerator for smooth animation

## Getting Started

1. Download the repository and open it in Unity
2. Drag the Labanotation script onto a GameObject in the Unity scene
3. Assign the corresponding body parts to the leftArm, rightArm, leftLeg, and rightLeg fields in the Unity Inspector
4. Assign the rotation parameters in the Unity Inspector
5. Run the scene in Unity

## Prerequisites

- Unity game engine version 2020.2.5f1 or higher
- SimpleJSON library

Note

This script assumes that the JSON file containing Labanotation data is located at "Assets/Labans/laban_sample.json" and that it follows a specific format. Make sure to adjust the script accordingly if you are using a different file location or format.
Author

This code is inspired by the Labanotation system, a method for analyzing and recording movement.

Also the code is using SimpleJSON library for parse the json file.
