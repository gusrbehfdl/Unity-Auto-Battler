Thank you for purchasing IK Avatar Poser!

Please send support requests to stephen@turnthegameon.com

This asset allows you to pose rigged humanoid models by positioning IK control point objects.

To get started, check out the video tutorial:
Getting started: https://youtu.be/B0QujhaeB14

Or follow these steps:

1. Add a rigged humanoid model to the scene.
   A sample prefab is preconfigured for demo/testing, and located at:
   Assets\TurnTheGameOn\DemoAssets\Characters\Unity_Armature\Prefabs\Armature

   The source Armature.fbx, which is the rigged humanoid model, is located at
   Assets\TurnTheGameOn\DemoAssets\Characters\Unity_Armature\Models\Armature.fbx

   This model has the following 'Import Settings' configured:
   Rig - Animation Type - Humanoid

   This model is from Starter Assets - Third Person Character Controller, which can be found at:
   https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-196526

   

2. Attach an animation controller with IK Pass enabled to the animator, one with animation, and one without animnation.
   Two sample animation controllers are included at the following path:
   Assets\TurnTheGameOn\IKAvatarPoser\AnimatorControllers\AvatarPoserNoAnim
   Assets\TurnTheGameOn\IKAvatarPoser\AnimatorControllers\AvatarPoserIdle



3. Attach the IKAvatarPoser script to the model in the scene, it's located at:
   Assets\TurnTheGameOn\IKAvatarPoser\Scripts\IKAvatarPoser



4. Press the 'Configure IK Avatar Poser' button on the IKAvatarPoser script.
   This will do the following:
   - Create a new parent for the model named IKAvatarPoser, placing the prefab model as a child
   - Add head, hands, elbows, knees, and feet IK Control Point objects as children
   - Configure IKAvatarPoser script references



5. Select the model prefab with the IKAvatarPoser script attached.
   You will have the option to view and move/rotate the following control points in the scene view to pose the avatar:
   - Head
   - Left Hand
   - Left Elbow
   - Right Hand
   - right Elbow
   - Left Foot
   - Left Elbow
   - Right Foot

   Switch between the builtin Unity Move/Rotate tools as desired

   You can also select the individual control point objects in the scene for more control, or have scripts control these transforms at runtime.

   As long as the script and animator is enabled, these control points can be moved in real-time to apply movement to the avatar.



6. Once you've completed posing your avatar, if you don't need the avatar to be animated during runtime,
   you can disable the animator and IKAvatarPoser components, and mark the IKAvatarPoser and children objects as static.

   This will allow you to have the best performance possible.

   If you don't need to edit the pose in the future, to finalize this state, you can do the following:
   - Remove the IKAvatarPoser and Animator components
   - Unparent the prefab model from the IKAvatarPoser parent
   - Delete the IKAvatarPoser object
   - Set the prefab model to static
   - save the prefab model as a prefab varient by drag/droping it into the Project folder.



Scriptable object profiles:
Using profiles: https://youtu.be/5bBBqGAy0NY

1. A ProfileData scriptable object can be assigned via the inspector.
2. Select Assets/Create/IKAvatarPoser/ProfileData to create the object.
3. Select the IKAvatarPoser component and assign the ProfileData.
4. Set the Profile Name, then click the Add button to create a profile to save the current transform and IK settings as a profile.
5. Make changes and press the Update button to modify the profile.
6. Press the Load button to load the saved profile settings.