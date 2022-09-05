It looks like .gitignore needs to be at the root folder of each unity project - can't put it in this root folder

Note that a lot of the projects have a lib folder with dlls from PerfectlyNormalUnity project

https://github.com/charlierix/PerfectlyNormalUnity


## VR Flight Projects

These are attempts to make ironman style flight in vr.  Each of the projects focuses on a different style of  flight.  The final result would be something that blends them, choosing the appropriate flight style based on speed/orientation

- **Egg Balancing**

KeepUpright is always trying to keep the up pointed along 0,1,0 (but is weak to allow strong enough torques to roll/pitch/yaw the player)

The player holds hands in a neutral position, activates flight.  Then the current position/orientation of hands relative to zero applies various forces on the body

No aerodynamics, just a rigid body with simple drag






