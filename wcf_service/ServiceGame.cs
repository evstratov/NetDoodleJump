using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ServiceGame" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceGame : IServiceGame
    {
        List<ServerPlayer> players = new List<ServerPlayer>();
        int nextID = 1;
        public int Connect(string name)
        {
            ServerPlayer player = new ServerPlayer()
            {
                ID = nextID,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextID++;
            players.Add(player);
            return player.ID;
        }

        public void Disconnect(int id)
        {
            //var player = players.FirstOrDefault(i => i.ID == id);
            if (players.Count > 0)
            {
                foreach (var item in players)
                {
                    //if (item.ID != id)
                        item.operationContext.GetCallbackChannel<IServerGameCallback>().GameOverCallback();
                }
                players.Clear();
                //players.Remove(player);
                // конец игры
            }
        }

        public void SendPlayerInfo(object[] info, int id)
        {
            foreach(var item in players)
            {
                //var player = players.FirstOrDefault(i => i.ID == id);
                if (item.ID != id)
                {
                    item.operationContext.GetCallbackChannel<IServerGameCallback>().PlayerInfoCallback(info);
                }
            }
        }

        public bool StartGame()
        {
            if(players.Count == 2)
            {
                return true;
            }
            return false;
        }
    }
}
