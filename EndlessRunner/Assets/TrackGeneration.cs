using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGeneration : MonoBehaviour
{
    [SerializeField] GameObject[] tracks;
    [SerializeField] Transform trackTransform;
    private float trackPosZ = 0, trackPosX = 0;
    private void Start()
    {
        InvokeRepeating(nameof(GenerateTrack), 2, 3);
    }
    private void GenerateTrack()
    {
        int r = Random.Range(0, tracks.Length);
        Instantiate(tracks[r], new Vector3(trackPosX, 0, trackPosZ), Quaternion.identity, trackTransform);
        trackPosZ += 80;
        if (r == 1)
        {
            trackPosX += 59;
        }
        if (r == 4)
        {
            trackPosX += 72.5f;
        }
    }
}
