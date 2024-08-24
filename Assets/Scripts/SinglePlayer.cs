using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Assets.Scripts.Unit;
using Assets.Scripts.Unit.Units;
using Mirror;
using UnityEngine;

public class SinglePlayer : ISP {

    [SerializeField] private GameObject ui;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float cameraSpeed = 1f;

    public Material material;

    private UIReciever _reciever;
    private float rotationX;

    #region Timer + count
    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; count++; StopAllCoroutines(); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime / 3.5f, 3); },
            3.5f
        ));
    }

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
    }

    public override void OnStartClient() { 
        PutOnCooldown(); 
    } 

    #region Commands

    [Command]
    public void WinCmd() {
        Debug.Log("Player: " + this + " Won");
    }
    [Command]
    public void LoseCmd() {
        Debug.Log("Player: " + this + " Won");
    }
    #endregion

    private void Update() {
        if(!isLocalPlayer) { return; }
        UpdateEnemyTimer();
        _reciever.UpdateUIRpc(timer, enemyTimer, count);
        if(Input.GetKeyDown(KeyCode.Space)) {
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