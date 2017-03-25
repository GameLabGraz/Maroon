Hi there! Thank you for downloading the Custom Teleporter Asset. :)
I made it as simple as I could, to be used in any project.

There is an example prefab set up with an example model, so you see how its suppose to look.The sounds that come with the asset are from Unity's Stealth project asset, as example. 
Also the CustomTeleporter script is heavily commented so you don't have any problems understanding how it works. :)

Instructions:

1. Just drag and drop the CustomTeleport script onto an object
2. Set it up in the inspector the way you want it to work
3. Add a particle system to it, if you want one
4. Add two audio source components, one for the teleportation sound and one for the teleport pad constant sound (humming or something)
5. The teleport uses a trigger, so add that too. 

And thats it.

I think the Inspector is pretty self explanatory but I'll explain each setting here anyway:

- Instant Teleport: do you want it to teleport instantly upon entering it?
- Random Teleport: teleport to a random pad from the list?
- Button Teleport: Use a button to activate the teleportation?
- Button Name: whats the name of the button you want to use?
- Delayed Teleport: Do you want to delay the teleportation by X time?
- Object Tag: Does an object have a tag? If left empty, any object will be allowed to teleport, otherwise, type in the tag you wanna use for that teleport
- Destination Pad: builtin array list of all pads you want to use, for random teleportation. if the array is not random, it will use the first pad in the list. Just drag and drop pads from your screen you want to use.
- Teleportation height: In some cases you might wanna teleport a bit higher or lower, thats what this offset setting is for
- Teleport Sound: sound source comopnent that plays upon teleportation
- Teleport Pad Sound: sound source component that always plays from the teleport pad. I've reduced the 3D sound distance so you only hear it standing near, but you might wanna tweak that to fit your game better. 

Thats it! If you have any problems or suggestions please let me know and I'll see if I can fix/do it!

Chris
Damaged Grounds