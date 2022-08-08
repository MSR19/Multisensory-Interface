# Multisensory Interface

This is a interface done for the thesys "Application of Multisensory Interfaces in Immersive Analytics"

It uses sound, touch and graohs to transmit information to the user.

This projetc used IATK (https://github.com/MaximeCordeil/IATK) as a starting point to the interface. 

# Dependencies
Unity version:
2019.4.28f1

Packages:

VRTK (https://vrtoolkit.readme.io/)

Maestro (https://assetstore.unity.com/packages/tools/audio/maestro-midi-player-tool-kit-free-107994)

This also used another repo to create the music (https://github.com/MSR19/Sonification)
Afther creation the music is added to the Maestro midi tracks.

# Features

## Graphs

The IATK toolkit was used to create the graphs of the interface.
The graphs of IATK can use the X, Y, Z-axis, and color to represent data.

To move the graphs, VRTK was used. There are two ways of moving the graph. If the user is close enough to the graph, the grip button is enough to grip the graph. The user can also use a combination of the trackpad and the grip button to move the graph from a distance.

![Graphs](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/Graphs.gif)

## Visual Indicators

Visual feedback was implemented in the soundboard buttons, the soundboard sliders, and the graphs. 

![Visual Indicators](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/VisualInteraction.gif)

![Visual Indicators2](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/VisualIndication2.gif)

## More Details

To allow the user to retrieve specific information about a point, Haptic Details were added. The details on-demand function by giving the specific information of the closest point of the right hand, displaying this information on top of the left hand. 
This feature works together with the haptic experience to give a complete picture of each point. 

![More Details](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/MoreDetails.gif)

## Soundboard

The soundboard is the main object for sound manipulation, with it the user can change the speed, initial position and final position of the music. 
The basic music commands play, pause, restart, stop, next track, and previous track are also available.

![Soundboard](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/SoundBoard.gif)

## Portable Soundboard

A more acessible version of the soundboard.

![Portable Soundboard](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/PortableSoundBoard.gif)

## Sound Animation

Sound animation is the connection between the visual and the sound. 
The point being played is represented in yellow, and points already played are represented in white. 
The proposes of the animation is to help the user identify trends and to help synchronize the visual with the sound.

![Sound Animation](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/SoundAnimation.gif)

## Haptic Experience

In the specific experience, the user can interact with the graph choosing the particular point to be analyzed. 
After selecting a specific point is possible to retrieve more information about the point, by using the trigger button.

![Haptic Experience](https://github.com/MSR19/MSR19/Multisensory-Interface/blob/main/Images/HapticExperience.gif)