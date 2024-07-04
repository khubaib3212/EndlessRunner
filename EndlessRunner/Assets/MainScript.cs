using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    public static MainScript instance;
    public TrackGeneration trackGeneration;
    [SerializeField] PlayerMovement playerScript;
    public bool gameStarted = false;
    public Transform player1, player2;
    private Transform playerSelected;
    public GameObject btnRetry;
    private int currentVisiblePlayer = 2;

    private void Awake()
    {
        playerSelected = player1;
        instance = this;
    }
    private void Update()
    {
        if(!gameStarted && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Camera.main.transform.SetParent(playerSelected);
            gameStarted = true;
            playerSelected.GetChild(0).GetComponent<Animator>().SetTrigger("Run");
            Camera.main.transform.RotateAround(playerScript.transform.position, transform.up, 180);
            Camera.main.transform.localPosition = new Vector3(0, 5, -10);
            Camera.main.transform.localEulerAngles=new Vector3(8,0,0);
        }
    }

    public void OnBtnInvin()
    {
        playerScript.freeRun = !playerScript.freeRun;
    }
    public void PlayerDied()
    {
        btnRetry.SetActive(true);
    }
    private void OnEnable()
    {
        PlayerMovement.OnDiedEvent += PlayerDied;
    }
    private void OnDisable()
    {
        PlayerMovement.OnDiedEvent -= PlayerDied;
    }
    public void OnBtnNextPlayer()
    {
        if(currentVisiblePlayer== 2) { return; }
        currentVisiblePlayer++;
        player2.DOMoveX(0, 0.5f);
        player1.DOMoveX(5, 0.5f);
        playerSelected = player2;
    }
    public void OnBtnPreviousPlayer()
    {
        if (currentVisiblePlayer == 1) { return; }
        currentVisiblePlayer--;
        player1.DOMoveX(0, 0.5f);
        player2.DOMoveX(-5, 0.5f);
        playerSelected = player1;
    }
    public void OnBtnRetry()
    {
        SceneManager.LoadScene(0);
    }
}
