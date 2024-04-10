An FPS game concept I worked on in my free time. 

I experimented with basic AI using unity built in pathfinding , movement mechanics such as sliding , dashing , jumping and sprinting

I also created a shooting system with melee weapons , projectile blocking , projectile and hitscan based weapons as well as aiming, recoil and ammunition 

The most impressive part of the system is a grid based inventory system of replasable organs

Grid based inventory and player upgrade system for an RPG first person shooter

The system is inspired by "Wrought Flesh"

It is split into multiple classes each responsible for its own element of getting , storing and replacing organs , as well as showing their effects and applying their effects to a player class that is not included in this repository

All classes without "Organ" in the name are responsible for UI showcase , or dragging the organs from inventory to body parts.

There are 2 grid types, 1 for organs as they take up place that can be non-rectangular , and another for body parts and inventory that is exclusively rectangular.

As such there are 2 tile types for body and for individual organs.

The organs are placed on body parts by dragging them on top of unocuppied body part tiles, when it is done those tiles are not occupied and cant be occupied by other organs.

Used this library to create non-rectangular grids: https://github.com/Eldoir/Array2DEditor/tree/master

To test the system and see any additional features you can drop in an organ object with a dash organ scriptable object and pick it up adding it to your inventory system.
Then interact with the object with text above it using "F" and add it to your body from your inventory (from right to left)

Organs spawned by default in the scene are just general organs that change stats but dont affect gameplay yet.

Behind the watch tower you can see 4 debug triggers one of which drops random organs.

Using right click you can pick up weapons and add them to your inventory and cycle through them using 1-4 buttons. 
