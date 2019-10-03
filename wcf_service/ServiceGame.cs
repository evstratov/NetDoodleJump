﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ServiceGame : IServiceGame
    {
        List<ServerPlayer> players = new List<ServerPlayer>();
        Random rnd = new Random();
        int requestNum = 0;
        int nextID = 1;
        int xCoord = 0;
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
            if (players.Count == 2)
            {
                return true;
            }
            return false;
        }

        public int GetXcoordinate(int id, int x, int formWidth, int edgeWidth)
        {
            try {
                requestNum++;
                if (requestNum < 2)
                {
                    xCoord = GetNewCoordinate(x, formWidth, edgeWidth);
                    return xCoord;

                } else
                {
                    requestNum = 0;
                    return xCoord;
                }              
            }
            catch(Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
        public int GetNewCoordinate(int x, int formWidth, int edgeWidth)
        {
            try
            {
                int newX = 0;
                if (x - edgeWidth > 100 && (formWidth - edgeWidth) - (x + edgeWidth) > 100)
                {
                    if (rnd.Next(0, 1) == 0)
                        newX = rnd.Next(0, x - edgeWidth);
                    else
                        newX = rnd.Next(x + edgeWidth, formWidth - edgeWidth);

                }
                else
                {
                    if (x - edgeWidth > 100)
                        newX = rnd.Next(0, x - edgeWidth);
                    else if ((formWidth - edgeWidth) - (x + edgeWidth) > 100)
                        newX = rnd.Next(x + edgeWidth, formWidth - edgeWidth);
                }
                return newX;
            } catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
    }
}
