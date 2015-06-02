A small library of code that takes care of all sound-related needs in a 3D game.
Audio Switcher and Audio Animatronix create dynamic background music, that can be changed externally
through method calls, or internally through Audiomations.  See examples and in-code documentation
for tips on setting up manifests, masks, and audiomations.

Story Sounds plays sound clips from an audio source which is parented to the camera, and
is best used for GUI-related sound effects.

World Audio sends a pre-instantiated audio source to a specific location in space,
and is best used for 3D in-world sounds.

It may seem simple, but these classes allow for efficient, complete, and accurate sound
design, with very little setup and even less of a CPU/Memory load.