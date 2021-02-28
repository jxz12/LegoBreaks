You have 5 bricks to construct the most breakable creation possible! Inspired by the 'Drop test' challenge in Lego Masters: series 2 episode 2.

Try it yourself at https://jxz12.github.io/LegoBreaks

This git repo does not contain the Lego-specific assets. To get everything together, FIRST clone this repo and open in Unity. Unity will automatically populate a bunch of other folders, e.g. Packages and Library, on open.

Then import the assets at [link](https://assetstore.unity.com/packages/templates/lego-microgame-179847), WITHOUT overwriting the ProjectSettings folder. This will include the 'LEGO' and 'LEGO Data' folders under Assets. Everything required should now be included in your project.
(Note that Unity will warn you that it will overwrite, but you can later choose not to by unchecking a box on import. Also note that importing these assets takes 20-30 minutes on my sad laptop.)

Controls:
 - move mouse to choose where to place brick
 - left-click places a brick, right-click spins camera 90 degrees
 - A and D spin the brick anti-clockwise and clockwise, respectively
 - S and W undo and redo placed bricks, respectively
