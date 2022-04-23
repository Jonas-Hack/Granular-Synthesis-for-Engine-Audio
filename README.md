This is a small script, which uses granular synthesis to dynimically alter engine sounds in Unity.

Inspired by <a href="https://youtu.be/pvrrCNxrMvg">this video</a> by Francesco Cucchiara.


## Concept

This script uses <a href="https://cmtext.indiana.edu/synthesis/chapter4_granular.php">granular synthesis</a> to dynamically mimic engine sounds.

More specifically it makes use of granulation of sampled sound. The script doesnt speed up or pitch shift the input sound, but traverses it based on engine RPM.

It seperates an input sound into smaller sections called grains and adjusts them to avoid clipping.
During runtime it chooses a grain, which resembles the wanted RPM of the virtual engine.
To make the granulation less noticable, grains are crossfaded (through so called <a href="http://www.granularsynthesis.com/hthesis/envelope.html">envelopes</a>) and some variation (in the form of slightly randomized RPM) is added.


## Usage

To get good results parameters need to be tweaked to fit your engine sound clip.
A high quality sound with a long duration also helps.
Works better with engines, which dont have slow and clearly audible pistons.
 
For debugging I used <a href="https://freesound.org/s/425384/">this</a>  <a href="https://freesound.org/people/Soundholder/sounds/425846/">and this sound</a> by "Soundholder", cut to only the engine reving.

## Limitations

This system has no information on when and for how long pistons are audible. 
Therefore it may sometimes cut them off. 
It may also lead to slightly irregular rythms.
This can make the granulation easy to hear and ruin the illusion of a continous engine sound.
Some way of adjusting the grain sizes and positions to encompass a single firing of the piston each could solve this problem.
At this point, that is outside the scope of this little project. When using engines whithout clearly audible pistons, the results sound quite good, although slightly sci fi.