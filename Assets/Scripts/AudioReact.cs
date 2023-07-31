using System.Collections;
using UnityEngine;

public class AudioReact : MonoBehaviour
{
    float loudness;
    Vector3 scale, startScale;
    public AudioSource audioSource;
    public float updateStep = 0.1f, multiplier;
    public int sampleDataLength = 1024;
    public Transform Object;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    // Use this for initialization
    void Awake()
    {
        startScale = transform.localScale;
        if (!audioSource)
        {
            Debug.LogError(GetType() + ".Awake: there was no audioSource set.");
        }
        clipSampleData = new float[sampleDataLength];

    }

    // Update is called once per frame
    void Update()
    {

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
        }
        loudness = clipLoudness * multiplier;

        scale = new Vector3(loudness, loudness, loudness);
        transform.localScale = new Vector3(transform.localScale.x, loudness + startScale.y, transform.localScale.z);
        
    }

}