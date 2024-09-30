using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using Assets.Scripts.Unit;
using Assets.Scripts.Unit.Units;
using Mirror;
using Steamworks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SinglePlayer : ISP {

    [SerializeField] private GameObject ui;
    [SerializeField] private Transform _camera;
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float cameraSpeed = 2.5f;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip onWin;
    [SerializeField] private AudioClip onLose;
    [SerializeField] private AudioClip onStartGame;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private float leftX;
    [SerializeField] private float rightX;
    [SerializeField] private float maxForward;
    [SerializeField] private Button button;
    [SerializeField] private GameObject plusOne;
	[SerializeField] private float ZoomSpeedMouse = .5f;

	private bool zoomActive;
    private bool panActive;
	private Vector3 lastPanPosition;
    private float nig = 0;

    public Material material;

    [SerializeField] private AudioSource source;
    private UIReciever _reciever;
    private float rotationX;

    #region Timer + count
    private void PutOnCooldown() {
        StartCoroutine(wait(
            delegate { timer = 0; if(count < 1) { count += localCount + 1; localCount = 0; } StopAllCoroutines(); PutOnCooldown(); }, 
            delegate { timer += MathF.Round(Time.deltaTime / 4f, 3); },
            4f
        ));
        
    }

    [Command]
    public override void CmdClick()
    {
        if(nig < 0.3f || c == 0) { return; }
        nig = 0;
        c -= 1;
        if (proj_speed <= 0.5f) {
            proj_speed = 1;
        } else {
            proj_speed -= 0.05f;
        }
        if(count < 1) {
            count = 1;
            PutOnCooldown();
        }
        base.CmdClick();
        var v = Instantiate(plusOne, button.transform);
        v.transform.position = new Vector3(v.transform.position.x + UnityEngine.Random.Range(-50, 50), 0, 0);
        if(UnityEngine.Random.Range(0f, 1f) > 0) {
            FindObjectOfType<AI>().CmdClick();
        }
    }

    public IEnumerator tryShoot(float seconds) {
        yield return new WaitForSeconds(seconds);
        CmdClick();
        StartCoroutine(tryShoot(seconds));
    }

    public void Click() {
        SteamFace.instance.PlayClick();
    }

    public void SwitchShouldShow(bool flag) {
        SteamFace.instance.shouldShow = flag;
    }

    public void Restart() { Debug.Log("Restart"); Pause(); NetworkServer.Shutdown(); NetworkClient.Disconnect(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

    public void Quit() { Pause(); NetworkServer.Shutdown(); NetworkClient.Disconnect(); SceneManager.LoadScene(0); }

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
        Pause();
        if(SteamFace.instance.shouldShow) {
            helpPanel.SetActive(true);
        }
        source.PlayOneShot(onStartGame);
    StartCoroutine(tryShoot(4f));
    }

    public override void WinCmd()
    {
        SteamInventory.TriggerItemDropAsync(0);
        source.PlayOneShot(onWin);
        base.WinCmd();
    }

    public override void LoseCmd()
    {
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
                FindObjectOfType<UIBaseHpManager>().UpdateUISingle(baseUnit.GetHealth(), _enemy.GetBase().GetHealth(), baseUnit.GetProperties().MaxHealth, curReq, maxReq);
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
        nig += Time.deltaTime;
        CmdUpdateUI();
        _reciever.UpdateUIRpc(timer, enemyTimer, c);
        if(Input.GetKeyDown(KeyCode.Space)) {
            source.PlayOneShot(click);
            CmdClick();
        }
        // Camera rotation
        if(Input.GetAxis("Horizontal") > 0 && cameraHolder.position.x < rightX) {
            cameraHolder.position = new Vector3(cameraHolder.position.x + Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, cameraHolder.position.y, cameraHolder.position.z);
        } else if(Input.GetAxis("Horizontal") < 0 && cameraHolder.position.x > leftX) {
            cameraHolder.position = new Vector3(cameraHolder.position.x + Input.GetAxis("Horizontal") * cameraSpeed * Time.deltaTime, cameraHolder.position.y, cameraHolder.position.z);
        }

        if(Input.GetAxis("Vertical") > 0 && cameraHolder.position.z < maxForward) {
            cameraHolder.position = new Vector3(cameraHolder.position.x, cameraHolder.position.y, cameraHolder.position.z + Input.GetAxis("Vertical") * cameraSpeed * Time.deltaTime);
        } else if(Input.GetAxis("Vertical") < 0 && cameraHolder.position.z > -35) {
            cameraHolder.position = new Vector3(cameraHolder.position.x, cameraHolder.position.y, cameraHolder.position.z + Input.GetAxis("Vertical") * cameraSpeed * Time.deltaTime);
        }
        
        if (Input.GetMouseButtonDown(0)) {
			panActive = true;
			lastPanPosition = Input.mousePosition;
		} else if (Input.GetMouseButtonUp(0)) {
			panActive = false;
		} else if (Input.GetMouseButton(0)) {
			PanCamera(Input.mousePosition);
		}

        float scroll = Input.GetAxis("Mouse ScrollWheel");
		zoomActive = true;
		ZoomCamera(scroll, ZoomSpeedMouse);
		zoomActive = false;

    }

	void ZoomCamera(float offset, float speed) {
		if (!zoomActive || offset == 0) {
			return;
		}

        ZoomSpeedMouse = PlayerPrefs.GetFloat("ZoomSpeedMouse") + 0.5f;

		cameraHolder.GetComponentInChildren<Camera>().fieldOfView = Mathf.Clamp(cameraHolder.GetComponentInChildren<Camera>().fieldOfView - (offset * speed), 10f, 85f);
	}

    void PanCamera(Vector3 newPanPosition) {
		if (!panActive) {
			return;
		}

		Vector3 offset = Camera.main.ScreenToViewportPoint(lastPanPosition - newPanPosition);
		Vector3 move = new Vector3(offset.x * panSpeed, 0, 0);
        if(offset == Vector3.zero) { return; }
        if(offset.x > 0 && cameraHolder.position.x < rightX) {
    		cameraHolder.Translate(move);  
        } else if(offset.x < 0 && cameraHolder.position.x > leftX) {
	    	cameraHolder.Translate(move);
        }

        if(offset.y > 0 && cameraHolder.position.z < maxForward) {
            cameraHolder.Translate(new Vector3(0, 0, offset.y * panSpeed));
        } else if(offset.y < 0 && cameraHolder.position.z > -35) {
            cameraHolder.Translate(new Vector3(0, 0, offset.y * panSpeed));
        }

		lastPanPosition = newPanPosition;
	}
}