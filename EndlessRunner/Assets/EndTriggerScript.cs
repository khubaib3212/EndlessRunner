using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTriggerScript : MonoBehaviour
{
    [SerializeField] GameObject track;
    private bool playerDied = false;

    private void OnEnable()
    {
        PlayerMovement.OnDiedEvent += PlayerDied;
    }
    private void OnDisable()
    {
        PlayerMovement.OnDiedEvent -= PlayerDied;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("checkingJumpChange");
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("name = " + track.name);
            MainScript.instance.trackGeneration.GenerateTrack();
            Invoke(nameof(DestroyTrack), 2);
        }
    }
    private void DestroyTrack()
    {
        if (!playerDied)
            Destroy(track);

    }
    private void PlayerDied()
    {
        playerDied = true;
        Debug.Log("died++");
    }
}
