<img src="/Assets/Images/logo.png" width="400"/>

> ### Maroon
> A interactive physics laboratory and experiment environment implemented in Unity3D, designed for active learning in the classroom or at home. It visualizes and simulates various physics experiments

[![Maroon VR Video](https://img.youtube.com/vi/iGuzgi-hfcM/0.jpg)](https://www.youtube.com/watch?v=iGuzgi-hfcM)

## Design and Implementation of Maroon
Maroon is an interactive immersive physics laboratory developed in [Unity], a
game engine which allows to build and deploy a high-quality 3D environment across
mobile, desktop and VR platforms. The lab is designed to support the flexible integration
of different interactive learning experiences.
To start a specific learning experience, the user would approach one
of those stations (illustrated with a pink marker) and would get teleported to a new
room which represents the specific learning experiences with the learning content,
simulations, or experiments.


![](/Assets/Images/Screenshots/lab_overview.png)

An conceptual overview of the different version of Maroon (Maroon Desktop, Maroon Room-Scale VR, Maroon Mobile VR and Maroon
Multi-User) support different forms of engagement is illustrated in the following figure.

<img src="/Assets/Images/Screenshots/architecture.png" width="600"/>

## Simulations and Experiments

### Van de Graaff Generator

A Van de Graff generator is an electrostatic generator and converts mechanical energy
into electrical energy. It is one of the most frequently used device for physics
teaching experiments. A rotating insulating belt will be electrically charged by friction.
The electrical charge is transported by the movement of the belt into the large
metallic hollow ball.

There are currently two experiments in Maroon which deal with a Van de Graff
generator. The first one demonstrates the electric field between the generator and
a grounding sphere. The Van de Graaff generator is charged by holding the trigger
button on the controller. Users can change the distance between the two objects to
see how the frequency of the discharges changes. If the generator stores enough
energy it produces a visible spark. The second experiment
shows a balloon placed between a Van de Graaff generator and a grounding sphere.
Users can observe the behavior of the balloon by charging the generator.

<img src="/Assets/Images/Screenshots/vandegraff2.PNG" width="600"/>

### Falling Coil and Faraday’s Law
In the Falling Coil experiment, a small magnet is positioned some distance above
the table and a conducting non-magnetic ring is then dropped down onto it. If the
coils enters the magnetic field of the magnet, a current is induced. Once there is a
current running through the ring, it has its own magnetic field which then interacts
with the magnetic field of the magnet and applies an upward force to the coil that
pushes it back up. The ring has a specified mass, a resistance and self-inductance and
the magnet has a magnet dipole moment. Users can change these parameters using
control panels to see how the magnetic flux and the induced current are changing
correspondingly. Furthermore, users can activate different visualizations like field
lines, a vector field and the iron filling visualization. This allows the
user to recognize invisible phenomena and thus, get a better understanding of the
underlying magnet field.

The Faraday’s Law experiment is very similar to the Falling Coil experiment. It
shows the interaction between a coil and a magnet, both constrained on the horizontal axis.
When users move the magnet themselves, the changing flux through the coil
leads to a current into the coil which is displayed to the user as a graph. The special
feature in this experiment is that the user can also feel the acting force through haptic
feedback which corresponds to the force. This gives the user an ”extra-dimension”
for an even better experience.

<img src="/Assets/Images/Screenshots/falling_coil.png" width="600"/>

### Capacitor
A capacitor is an electrical component which consists of two electrically conductive
surfaces which are separated from each other by an insulating material, the dielectric.
It stores electric charge and the associated energy in an electric field. The stored
charge per voltage is called electrical capacitance which depends on the plate distance,
the overlapping area and the dielectric. Users can change these parameters to
see how each parameter affects the capacitance. The resulting capacitance value is
shown to the user on the display. By clicking the play button, the capacitor is being
charged up to the given voltage. The charging process is illustrated as graph on the
display and through charges that move from one plate to the other plate. The color
of the plates indicate how positive or negative they are charged. Users can also observe
the behavior of charges in the electric field by placing them into the field. The
underlying electric field can be visualized by using field lines and a 3D vector field
visualization.

<img src="/Assets/Images/Screenshots/capacitor.png" width="600"/>

### Huygens Principle 
The Huygens Principle experiment is one of the first experiment in Maroon that does
not deal with electromagnetism. It shows the physical model of diffraction, which
states that every point of a wavefront can be seen as a starting point of a new wave,
called elementary wave. The new wavefront results from the overlaying elementary
waves. To show diffraction, a slit plate is placed into the basin. Behind this plate,
the user can observe the interference pattern generated by diffraction of the wave
propagation at the slits. Users can change the wave amplitude, the wave length,
the wave frequency and the propagation mode to see how these parameters affect
the interference pattern behind the plate. The user can also replace the used plate
with other plates which have more or fewer slits. This leads to different interference
patterns. For a better wave illustration, the wave color can be freely changed.

<img src="/Assets/Images/Screenshots/huygens_principle.png" width="600"/>

## Supported SDK's
| SDK | Download Link |
|---------------|---------------|
| Unity 2018.3.4f1  | [Unity] |
| SteamVR 1.2.3 | [SteamVR Plugin] |
| VRTK 3.2.1 | [VRTK Plugin] |
| HTC Vive | [HTC Vive] |
| Oculus | [Oculus Integration] |
| Samung Gear VR | [Samsung Gear VR] |

[Unity]: https://unity3d.com
[SteamVR Plugin]: https://github.com/ValveSoftware/steamvr_unity_plugin/releases/download/1.2.3/SteamVR.Plugin.unitypackage
[VRTK Plugin]: https://github.com/thestonefox/VRTK/archive/3.2.1.zip
[HTC Vive]: https://www.vive.com
[Oculus Integration]: https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022
[Google VR SDK for Unity]: https://developers.google.com/vr/unity/download
[Samsung Gear VR]: https://www.samsung.com/us/mobile/virtual-reality/gear-vr
