using Mirror;
using UnityEngine;

public class ClickrSNetworkManager : NetworkManager {
    public Material color1;
        public Material color2;

        public Transform p1;
        public Transform p2;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // add player at correct spawn position
            Color start = numPlayers == 0 ? color1.color : color2.color;
            Material m = numPlayers == 0 ? color1 : color2;
            Vector3 startP = numPlayers == 0 ? p1.position : p2.position;
            GameObject player = Instantiate(playerPrefab, startP, Quaternion.identity);
            var _p = player.GetComponent<SinglePlayer>();
            _p.color = start;
            _p.material = m;

            _p.SetEnemy(FindObjectOfType<AI>());
            _p.isLeft = true;

            FindObjectOfType<AI>().SetEnemy(_p);
            FindObjectOfType<AI>().GetComponent<NetworkIdentity>().AssignClientAuthority(conn); 

            NetworkServer.AddPlayerForConnection(conn, player);
        }
}