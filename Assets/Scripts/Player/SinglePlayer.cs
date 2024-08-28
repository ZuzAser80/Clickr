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
    [SerializeField] private AudioClip click;

    public Material material;

    [SerializeField] private AudioSource source;
    private UIReciever _reciever;
    private float rotationX;

    #region Timer + count
    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; count += localCount + 1; localCount = 0; StopAllCoroutines(); PutOnCooldown(); }, 
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
        base.OnStartAuthority();
        
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

    private void Update() {
        if(!isLocalPlayer) { return; }
        UpdateEnemyTimer();
        CmdUpdateUI();
        _reciever.UpdateUIRpc(timer, enemyTimer, count);
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