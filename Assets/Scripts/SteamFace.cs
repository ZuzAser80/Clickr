using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamFace : MonoBehaviour
{
    [SerializeField] private AudioClip onMenuButtonClick;

    public static SteamFace instance;

    public bool shouldShow;

    private AudioSource _source;

    public void PlayClick() {
        _source.PlayOneShot(onMenuButtonClick);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null) { Destroy(this); return; }
        DontDestroyOnLoad(this);
        instance = this;

        _source = GetComponent<AudioSource>();

        try
        {
            SteamClient.Init(3202880, true);
            SteamInventory.LoadItemDefinitions();
            SteamInventory.GetAllItemsAsync();
            Debug.Log(SteamClient.Name);
        }
        catch ( System.Exception e )
        {
            // Something went wrong! Steam is closed?
            // Application.Quit();
            Debug.LogError(e);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // if(instance == null) { return; }
        SteamClient.RunCallbacks();
        // if(Input.GetKeyDown(KeyCode.X)) {
        //     Debug.Log(SteamInventory.Items);
        //     // Debug.Log();
        //     // SteamInventory.TriggerItemDropAsync(1)
        //     // foreach ( InventoryDef def in  SteamInventory.Definitions)
        //     // {
        //     //     Debug.Log( $"{def.Name}" );
        //     // }
        // }
    }

    private void OnApplicationQuit() {
       SteamClient.Shutdown();
    }

}
