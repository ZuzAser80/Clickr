using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Assets.Scripts.Unit;
using Assets.Scripts.Unit.Units;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayer : ISP {

    [SerializeField] private GameObject ui;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float cameraSpeed = 1f;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip onWin;
    [SerializeField] private AudioClip onLose;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject quitPanel;
    [SerializeField] private GameObject afdg;
    [SerializeField] private GameObject afgd;

    public Material material;

    [SerializeField] private AudioSource source;
    private UIReciever _reciever;
    private float rotationX;

    #region Timer + count
    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; if(count < 1) { count += localCount + 1; localCount = 0; } StopAllCoroutines(); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime / 3.5f, 3); },
            3.5f
        ));
    }

    public override void CmdClick()
    {
        base.CmdClick();
        if(count < 1) {
            PutOnCooldown();
        }
    }

    public void Click() {
        FindObjectOfType<SteamFace>().PlayClick();
    }

    public void Restart() { NetworkServer.Shutdown(); NetworkClient.Disconnect(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

    public void Quit() { NetworkServer.Shutdown(); NetworkClient.Disconnect(); SceneManager.LoadScene(0); }

    public void FlipGOState(GameObject go) {
        go.SetActive(!go.activeSelf);
    }  

    public void DisableGO(GameObject go) => go.SetActive(false);

    public IEnumerator wait(Action action, Action update, float seconds) {
        for (float i = 0; i < seconds;) {
            update?.Invoke();
            yield return new WaitForEndOfFrame();
            i += Time.deltaTime;
        }
        action?.Invoke();
    }
    #endregion

    public override void OnStartAuthority()
    {
        ui.SetActive(true);
        _reciever = ui.GetComponentInChildren<UIReciever>();   
        cameraHolder.gameObject.SetActive(true);
        base.OnStartAuthority();
        if(FindObjectOfType<SteamFace>().shouldShow) {
            helpPanel.SetActive(true);
        }
        Pause();

    }

    public override void WinCmd()
    {
        source.PlayOneShot(onWin);
        base.WinCmd();
    }

    public override void LoseCmd()
    {
        SteamInventory.TriggerItemDropAsync(0);
        source.PlayOneShot(onLose);
        base.LoseCmd();
    }

    public override void UpdateBases()
    {
        baseUnit.onDeath += delegate {FindObjectOfType<UIBaseHpManager>().Lost();};
        _enemy.GetBase().onDeath += delegate {FindObjectOfType<UIBaseHpManager>().Win();}; 
    }

    public override void OnStartClient() { 
        PutOnCooldown();

    } 

    #region Commands

    public virtual void CmdUpdateUI() {
        if(_enemy != null) {  
            _enemy.enemyTimer = timer; 
            if(_enemy.GetBase() != null) {
                FindObjectOfType<UIBaseHpManager>().UpdateUISingle(baseUnit.GetHealth(), _enemy.GetBase().GetHealth(), baseUnit.GetProperties().MaxHealth);
            }   
        }
    }

    #endregion

    public void Pause() {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    private void Update() {
        if(!isLocalPlayer || Time.timeScale == 0) { return; }
        UpdateEnemyTimer();
        CmdUpdateUI();
        _reciever.UpdateUIRpc(timer, enemyTimer, (count + localCount));
        if(Input.GetKeyDown(KeyCode.Space)) {
            source.PlayOneShot(click);
            CmdClick();
        }
        // Camera rotation
        cameraHolder.position = new Vector3(cameraHolder.position.x + Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, cameraHolder.position.y, cameraHolder.position.z);
        
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -45, 45);
        _camera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        cameraHolder.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); 
    }
}