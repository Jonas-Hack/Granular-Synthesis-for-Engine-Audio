This is a small script, which uses granular synthesis to dynimically alter engine sounds in Unity.

Inspired by <a href="https://youtu.be/pvrrCNxrMvg">this video</a> by Francesco Cucchiara.


## Concept

This script uses <a href="https://cmtext.indiana.edu/synthesis/chapter4_granular.php">granular synthesis</a> to dynamically mimic engine sounds.

More specifically it makes use of granulation of sampled sound. The script doesnt speed up or pitch shift the input sound, but traverses it based on engine RPM.

It seperates an input sound into smaller sections called grains and adjusts them to avoid clipping.
During runtime it chooses a grain, which resembles the wanted RPM of the virtual engine.
To make the granulation less noticable, grains are crossfaded (through so called <a href="http://www.granularsynthesis.com/hthesis/envelope.html">envelopes</a>) and some variation (in the form of slightly randomized RPM) is added.


## Usage

Provide a recording of an engine accelerating from idle to maximum RPM, whitout any additional sounds, as an input audio clip.
Use long and high quality recordings.
Works better with engines, which dont have slow and clearly audible pistons.

To get good results parameters need to be tweaked to fit your engine sound clip. Specifically the grain time (in milli seconds) can have a big influence on output quality. A longer grain time is true to the input sound but may lead to more noticable cuts. Short grain times can lead to noise and create underlying frequencies.

The envelope time (duration of clip cross fade) should be kept to around a third of the grain time.
 
For debugging I used <a href="https://freesound.org/s/425384/">this</a> and <a href="https://freesound.org/people/Soundholder/sounds/425846/">this</a> sound by "Soundholder", cut to only the engine revving.

## Limitations

This system has no information on when and for how long pistons are audible. 
Therefore it may sometimes cut them off. 
It may also lead to slightly irregular rhythms.
This can make the granulation easy to hear and ruin the illusion of a continous engine sound.
Some way of adjusting the grain sizes and positions to encompass a single firing of the piston each could solve this problem.
At this point, that is outside the scope of this little project. When using engines whithout clearly audible pistons, the results sound quite good, although slightly sci fi.