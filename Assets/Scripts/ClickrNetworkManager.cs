using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ClickrNetworkManager : NetworkManager
{
        public Color color1;
        public Color color2;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // add player at correct spawn position
            Color start = numPlayers == 0 ? color1 : color2;
            GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            player.GetComponent<Player>().color = start;
            NetworkServer.AddPlayerForConnection(conn, player);
        }

}
