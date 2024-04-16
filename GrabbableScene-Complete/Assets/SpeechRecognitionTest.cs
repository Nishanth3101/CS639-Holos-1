using HuggingFace.API;
using OpenAI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechRecognitionTest : MonoBehaviour
{
    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    public ChatGPTManager chatGPTManager;

    private void StartRecording()
    {
        clip = Microphone.Start(null, true, 10, 44100);
        recording = true;
    }

    private void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One)) //Button A
        {
            StartRecording();
            //StartCoroutine(ButtonDelay());

        }
        if (OVRInput.Get(OVRInput.Button.Two)) //Button B
        {
            StopRecording();
        }
        if (recording && Microphone.GetPosition(null) >= clip.samples)
        {
            Debug.Log("Clip Samples: " + clip.samples + " Position: " + Microphone.GetPosition(null));
            StopRecording();
        }
    }

    //IEnumerator ButtonDelay()
    //{
     //   yield return new WaitForSeconds(1);
    //}

    private void StopRecording()
    {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording()
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, (response) =>
        {
            chatGPTManager.AskChatGPT(response);
        }, (error) =>
        {
            Debug.Log("Error");
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write("RIFF".ToCharArray());
                binaryWriter.Write(36 + samples.Length * 2);
                binaryWriter.Write("WAVE".ToCharArray());
                binaryWriter.Write("fmt ".ToCharArray());
                binaryWriter.Write(16);
                binaryWriter.Write((ushort)1);
                binaryWriter.Write((ushort)channels);
                binaryWriter.Write(frequency);
                binaryWriter.Write(frequency * channels * 2);
                binaryWriter.Write((ushort)(channels * 2));
                binaryWriter.Write((ushort)16);
                binaryWriter.Write("data".ToCharArray());
                binaryWriter.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    binaryWriter.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
}
