using Mirror;
using UnityEngine;

public class AutoHost : MonoBehaviour {
    
    [SerializeField] private NetworkManager manager;
    
    private void Start() {
        manager.StopServer();
        manager.StopHost();
        manager.networkAddress = "localhost";
        manager.StartHost();
    }
}