using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamFace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);

        try
        {
            SteamClient.Init(480);
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
        SteamClient.RunCallbacks();
        if(Input.GetKeyDown(KeyCode.X)) {
            foreach ( InventoryDef def in SteamInventory.Definitions )
            {
                Debug.Log( $"{def.Name}" );
            }
        }
    }

    private void OnApplicationQuit() {
       SteamClient.Shutdown();
    }

    public void LeaveGame() => Application.Quit();
}
