using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var _p = player.GetComponent<Player>();
            _p.color = start;

            if(numPlayers != 0) {
                Player _ = FindObjectsByType<Player>(FindObjectsSortMode.None).ToList().Where(x => x.color != _p.color).FirstOrDefault();
                _.SetEnemy(_p);
                _p.SetEnemy(_);
            }

            NetworkServer.AddPlayerForConnection(conn, player);
        }

}
