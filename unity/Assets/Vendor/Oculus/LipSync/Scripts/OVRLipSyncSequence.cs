/************************************************************************************
Filename    :   OVRLipSyncSequence.cs
Content     :   LipSync frames container
Created     :   May 17th, 2018
Copyright   :   Copyright Facebook Technologies, LLC and its affiliates.
                All rights reserved.

Licensed under the Oculus Audio SDK License Version 3.3 (the "License");
you may not use the Oculus Audio SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/audio-3.3/

Unless required by applicable law or agreed to in writing, the Oculus Audio SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
************************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Threading;

// Sequence - holds ordered entries for playback
[System.Serializable]
public class OVRLipSyncSequence : ScriptableObject
{
    public List<OVRLipSync.Frame> entries = new List<OVRLipSync.Frame>();
    public float length;    // in seconds

    public OVRLipSync.Frame GetFrameAtTime(float time)
    {
        OVRLipSync.Frame frame = null;
        if (time < length && entries.Count > 0)
        {
            // todo: we could blend frame output here if we wanted.
            float percentComplete = time / length;
            frame = entries[(int)(entries.Count * percentComplete)];
        }
        return frame;
    }

#if UNITY_EDITOR

    private static readonly int sSampleSize = 1024;

    public static OVRLipSyncSequence CreateSequenceFromAudioClip(AudioClip clip)
    {
        OVRLipSyncSequence sequence = null;

        if (clip.channels > 2)
        {
            Debug.LogError(clip.name +
                ": Cannot process phonemes from an audio clip with " +
                "more than 2 channels");
            return null;
        }

        if (clip.loadType != AudioClipLoadType.DecompressOnLoad)
        {
            Debug.LogError(clip.name +
                ": Cannot process phonemes from an audio clip unless " +
                "its load type is set to DecompressOnLoad.");
            return null;
        }

        if (OVRLipSync.Initialize(clip.frequency, sSampleSize) != OVRLipSync.Result.Success)
        {
            Debug.LogError("Could not create Lip Sync engine.");
            return null;
        }

        if (clip.loadState != AudioDataLoadState.Loaded)
        {
            Debug.LogError("Clip is not loaded!");
            return null;
        }

        uint context = 0;
        OVRLipSync.Result result =
            OVRLipSync.CreateContext(ref context, OVRLipSync.ContextProviders.Enhanced);

        if (result != OVRLipSync.Result.Success)
        {
            Debug.LogError("Could not create Phoneme context. (" + result + ")");
            OVRLipSync.Shutdown();
            return null;
        }

        List<OVRLipSync.Frame> frames = new List<OVRLipSync.Frame>();
        float[] samples = new float[sSampleSize * clip.channels];
        int totalSamples = clip.samples;
        for (int x = 0; x < totalSamples; x += sSampleSize)
        {
            // GetData loops at the end of the read.  Prevent that when it happens.
            if (x + samples.Length > totalSamples)
            {
                samples = new float[(totalSamples - x) * clip.channels];
            }
            clip.GetData(samples, x);
            OVRLipSync.Frame frame = new OVRLipSync.Frame();
            if (clip.channels == 2)
            {
                // interleaved = stereo data, alternating floats
                OVRLipSync.ProcessFrame(context, samples, frame);
            }
            else
            {
                // mono
                OVRLipSync.ProcessFrame(context, samples, frame, OVRLipSync.AudioDataType.F32_Mono);
            }

            frames.Add(frame);
        }

        Debug.Log(clip.name + " produced " + frames.Count +
            " viseme frames, playback rate is " + (frames.Count / clip.length) +
            " fps");
        OVRLipSync.DestroyContext(context);
        OVRLipSync.Shutdown();

        sequence = ScriptableObject.CreateInstance<OVRLipSyncSequence>();
        sequence.entries = frames;
        sequence.length = clip.length;

        return sequence;
    }
#endif
};
