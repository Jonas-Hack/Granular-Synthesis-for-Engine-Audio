This is a small script, which uses granular synthesis to dynimically alter engine sounds in Unity.

Inspired by <a href="https://youtu.be/pvrrCNxrMvg">this video</a> by Francesco Cucchiara.


## Concept

This script takes an unsual approach to granular synthesis by assuming high frequency in the later parts of the sound, instead of manipulating the grains themselves.
It chooses grains accoriding to desired engine RPM linearly from the source sound. To break up the repetition when not accelerating, there is also some randomness.

## Usage

To get good results parameters need to be tweaked to fit your engine sound clip.
A high quality sound with a long duration also helps.
Works better with engines, which dont have slow and clearly audible pistons.
 
For debugging I used <a href="https://freesound.org/s/425384/">this sound</a> by "Soundholder", cut to only the engine reving.

I suggest you play around with the concept yourself, as the results could be improved when the approach is adjusted to your purposes.
As this was written in a single evening, there are still plenty of options for improvement.