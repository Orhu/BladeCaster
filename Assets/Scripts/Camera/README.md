# Camera Implementation

1. Install Cinemachine from unity packages; located under manage packages 2D section
2. While there is a room prefab already saved, and implore you to look over how it works, I will explain in simple how to make a camera and how to implement it
  1. Place a Cinemachine down onto the design workplace.
  2. Have the camera target be the player by dragging it on to it, this allows without any other parameters, will have the main camera follow the player.
  3. Create a "room" to house the camera onto; use polycollider with 4 points to make a square room
  4. Attach to the room object the "cameraMovement.cs" script to it.
  5. In the parameter of "virtualCamera" from cameraMovement, put our CM camera onto it.
  6. Go back to our CM; add the extension "Cinemachine Confiner" (Do not choose 2D, the base version will do that automatically.)
  7. Drag the room object down to "Bounding Shape" on the new Confiner

3. If this is followed properly, we now have a sliding camera that will follow the play to each room
- Additional rule to understand that CM's do not replace the main camera, and simple give a place for it to go when a certain condition is made.
