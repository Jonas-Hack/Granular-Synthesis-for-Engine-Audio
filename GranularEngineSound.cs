/*
 *  Written by Jonas H.
 *
 *  Uses granular synthesis to dynamically adapt an input engine sound to a given throttle.
 *
 *  To get good results parameters need to be tweaked to fit your engine sound clip.
 *  A high quality sound with a long duration also helps.
 *  Works better with engines, which dont have slow and clearly audible pistons.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class GranularEngineSound : MonoBehaviour
{
    AudioSource source;
    AudioSource source2;
    bool useAudioSource2 = false;

    [Tooltip("Sample of an engine going from low rpm to high")]
    [SerializeField] AudioClip clip = null;

    [Tooltip("Percent of possible RPM")]
    [Range(0.0f, 1.0f)] [SerializeField] float rev = 0;

    [Tooltip("Randomness to break up repetition when not accelerating")]
    [SerializeField] float variance = 0.05f;//Should be kept very low

    [Tooltip("How long is a single grain in milli seconds")]
    [SerializeField] float grainTime = 300.0f;//Will lead to wonky behaviour if < deltaTime

    [Tooltip("How long is a crossfade")]
    [SerializeField] float envelopeTime = 50.0f;//Will lead to wonky behaviour if > grainTime / 2

    [Tooltip("How small does a sample need to be to count as a good cur point to avoid clipping")]
    [SerializeField] float clipMargin = 0.1f;//Should also be kept low

    //Data buffers of each grain
    float[][] grains;

    //Schedule of upcoming grains
    LinkedList<int> grainSchedule = new LinkedList<int>();
    int currentGrain = 0;

    //Time used to determine when to play next sound
    double lastPlayTime = AudioSettings.dspTime;
    double lastPlayDuration = 0;

    void Awake()
    {
        //source = gameObject.GetComponent<AudioSource>();
        source = gameObject.AddComponent<AudioSource>();
        source2 = gameObject.AddComponent<AudioSource>();

        if (clip == null)
            Debug.LogWarning("The input audio clip of " + gameObject.name + " is null. "
                + "This will likely result in an error. Please assign a clip in the inspector");
        
        if(envelopeTime > grainTime)
        {
            envelopeTime = grainTime / 2.0f;
            Debug.LogWarning("The envelope size was higer than half the grainTime and has been adjusted");
        }

        generateGrains();
    }

    private void Update()
    {
        //Just cycles through throttle for debug
        //rev = 1.0f - Mathf.Abs(Mathf.Sin(Time.time / 5.0f));

        scheduleGrains();

        playScheduled();
    }

    /// <summary>Use this to set engine RPM as percent of total</summary>
    public void setRev(float rev)
    {
        this.rev = Mathf.Clamp(rev, 0.0f, 1.0f);
    }

    /// <summary>Cuts the given clip into grains of ca. grainTime and attempts to avoid clipping</summary>
    void generateGrains()
    {
        //Load Audio Clip
        int samples = clip.samples;
        float[] data = new float[samples];
        clip.GetData(data, 0);

        //Find nice seperation points between grains to avoid clicking
        //int grainSize = (int)(grainTime * clip.samples / clip.length / 1000.0f);
        int grainSize = (int)((double)grainTime / 1000.0 * clip.frequency);
        LinkedList<int> grainIndices = new LinkedList<int>();

        int envelopeSize = (int)((double)envelopeTime / 1000.0 * clip.frequency);

        source.clip = AudioClip.Create("grain", (int)(grainSize*2.0f), 1, clip.frequency, false);
        source2.clip = AudioClip.Create("grain2", (int)(grainSize*2.0f), 1, clip.frequency, false); 

        grainIndices.AddLast(0);
        for (int index = grainSize; index < samples; index += grainSize)
        {
            for(int r = 0; r < grainSize / 2; r++)
            {
                //Which sample am I looking at (besides the pre determined break point)
                int right = index - r;

                //How loud is the sample?
                if (Mathf.Abs(data[r]) < clipMargin)
                { 
                    index = right;
                    break;
                }
            }

            //Try to avoid clicking by doing a mini fade out
            data[index] = 0;
            if(index+1 < samples) data[index + 1] *= 0.5f;
            if(index > 0) data[index - 1] *= 0.5f;

            //Keep track of cut time
            grainIndices.AddLast(index);
        }


        //Copy corresponding clips into nice seperated arrays
        grains = new float[grainIndices.Count][];
        LinkedListNode<int> node = grainIndices.First;
        for (int i = 0; i < grainIndices.Count; i++)
        {
            LinkedListNode<int> next = node.Next;

            //Which interval to copy for this grain
            int index = node.Value;
            int nextIndex;

            if (next == null)//Is it the last grain
            {
                nextIndex = samples;//Last grain takes all remaining data

                if (nextIndex - index <= (grainSize+envelopeSize) / 2)
                    index = node.Previous.Value;//Avoid an incredibly short last grain
            }
            else
                nextIndex = next.Value;
            

            //The actual copying
            grains[i] = new float[nextIndex - index];

            for(int sampleNumber = 0, runningIndex = index; runningIndex < nextIndex; runningIndex++, sampleNumber++)
            {
                //Crossfade
                float fadeIn = Mathf.Clamp((float)sampleNumber / (float)envelopeSize, 0.0f, 1.0f);
                float fadeOut = Mathf.Clamp((float)(sampleNumber - grains[i].Length) / -(float)envelopeSize, 0.0f, 1.0f);
                float envMul = fadeIn * fadeOut;
                if (float.IsNaN(envMul) || float.IsInfinity(envMul)) envMul = 1.0f;
                if (envelopeTime == 0) envMul = 1.0f;

                grains[i][sampleNumber] = data[runningIndex] * envMul;
            }

            node = next;
        }
    }

    /// <summary>Schedules some grains to be played in the near future</summary>
    void scheduleGrains()
    {
        //Do I not have enough grains to make it to atleast the next frame?
        while (grainSchedule.Count < System.Math.Max(1, (2 * Time.deltaTime / (grainTime / 1000.0))))
        {
            //Selected wanted grain
            //(with some randomness so it doesnt get hung up on a single grain)
            int grainIndex = (int)(grains.Length * (rev + (Random.value*2.0f-1.0f) * variance));
            grainIndex = Mathf.Clamp(grainIndex, 0, grains.Length - 1);

            //Schedule to play soon
            grainSchedule.AddLast(grainIndex);
        }
    }

    /// <summary>Tells the audio system to play the next grain as soon as the previous finishes</summary>
    void playScheduled()
    {
        //Am I close to finish playing the current grain?
        if (AudioSettings.dspTime >= lastPlayTime + lastPlayDuration * 0.75 - (double)envelopeTime/1000.0)
        {
            //Find next in line
            int first = grainSchedule.First.Value;
            grainSchedule.RemoveFirst();

            //schedule to play next
            double duration = (double)grains[currentGrain].Length / clip.frequency;
            double playTime = lastPlayTime + lastPlayDuration - (double)envelopeTime / 1000.0 - 0.1;

            //Need to use this weird scheduled method as the audio thread if faster than Update
            AudioSource useSource = (useAudioSource2) ? source2 : source;
            useSource.clip.SetData(grains[first], 0);
            useSource.PlayScheduled(playTime);

            //keep track, so you can schedule the next one
            lastPlayDuration = duration;
            lastPlayTime = playTime;
            currentGrain = first;
            useAudioSource2 = !useAudioSource2;//Flip sources, so loading doesnt interfere with playing the other
        }
    }
}