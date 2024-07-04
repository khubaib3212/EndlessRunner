using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGeneration : MonoBehaviour
{
    [SerializeField] GameObject[] tracks;
    [SerializeField] Transform trackTransform;

    private float trackPosZ = 0, trackPosX = 0;
    private int randomTrack=0;
    private void OnEnable()
    {
        PlayerMovement.OnDiedEvent += PlayerDied;
    }
    private void OnDisable()
    {
        PlayerMovement.OnDiedEvent -= PlayerDied;
    }
    private void Start()
    {
        //InvokeRepeating(nameof(GenerateTrack), 0, 3);
        for (int i = 0; i < 2; i++)
        {
            GenerateTrack();
        }
    }
    public void GenerateTrack()
    {
        Instantiate(tracks[randomTrack], new Vector3(trackPosX, 0, trackPosZ), Quaternion.identity, trackTransform);
        trackPosZ += 80;
        if (randomTrack == 1)
        {
            trackPosX += 59;
        }
        if (randomTrack == 4)
        {
            trackPosX += 72.5f;
        }
        randomTrack = Random.Range(0, tracks.Length);
    }
    private void PlayerDied()
    {
        Debug.Log("died++");
        //CancelInvoke(nameof(GenerateTrack));
    }
}
