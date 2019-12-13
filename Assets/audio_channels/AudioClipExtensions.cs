using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class CreateChanelSpecificClips : MonoBehaviour


public static class AudioClipExtensions
{
    public static AudioClip CreateChanelSpecificClips(this AudioClip originalClip, int amountOfChannels, int targetChannel,string newName)
    {
        // Create a new clip with the target amount of channels.
        AudioClip clip = AudioClip.Create(newName, originalClip.samples, amountOfChannels, originalClip.frequency, false);
        // Init audio arrays.
        float[] audioData = new float[originalClip.samples * amountOfChannels];
        float[] originalAudioData = new float[originalClip.samples * originalClip.channels];
        if (!originalClip.GetData(originalAudioData, 0))
        {
            Debug.Log("SOMETHING AINT RIGHT IN MAKING THIS AUDIO, EARLY");
            return null;
        }
        // Fill in the audio from the original clip into the target channel. Samples are interleaved by channel (L0, R0, L1, R1, etc).
        int originalClipIndex = 0;
        for (int i = targetChannel; i < audioData.Length; i += amountOfChannels)
        {
            audioData[i] = originalAudioData[originalClipIndex];
            originalClipIndex += originalClip.channels;
        }
        if (!clip.SetData(audioData, 0))
        {
            Debug.Log("SOMETHING AINT RIGHT IN MAKING THIS AUDIO");
            return null;
        }
        return clip;
    }
}