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
            // количество игроков = 2, начать игру
            players.Add(player);
            return player.ID;
        }

        public void Disconnect(int id)
        {
            var player = players.FirstOrDefault(i => i.ID == id);
            if (player != null)
            {
                players.Remove(player);
                // конец игры
            }
        }

        public void SendPlayerInfo(object info, int id)
        {
            foreach(var item in players)
            {
                var player = players.FirstOrDefault(i => i.ID == id);
                //if (player != null)
                //{
                   
                //}
                item.operationContext.GetCallbackChannel<IServerGameCallback>().PlayerInfoCallback(info);
            }
        }
    }
}
