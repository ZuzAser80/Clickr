using System.Linq;
using Mirror;
using UnityEngine;

public class ClickrNetworkManager : NetworkManager
{
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
            var _p = player.GetComponent<Player>();

            _p.material = m;
            _p.isLeft = numPlayers == 0;

            if(numPlayers != 0) {
                Player _ = FindObjectsByType<Player>(FindObjectsSortMode.None).ToList().Where(x => x.color != _p.color).FirstOrDefault();
                _.SetEnemy(_p);
                _p.SetEnemy(_);
            }

            NetworkServer.AddPlayerForConnection(conn, player);
            _p.color = start;
        }
}
