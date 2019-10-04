using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_service
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IServerGameCallback))]
    public interface IServiceGame
    {
        [OperationContract]
        int Connect(string name);
        [OperationContract]
        bool StartGame();

        [OperationContract(IsOneWay = true)]
        void Disconnect(int id);
        [OperationContract]
        int GetXcoordinate(int id, int x, int formWidth, int edgeWidth);

        [OperationContract(IsOneWay = true)]
        void SendPlayerInfo(object[] info, int id);
    }
    public interface IServerGameCallback
    {
        [OperationContract(IsOneWay = true)]
        void PlayerInfoCallback(object[] info);
        [OperationContract(IsOneWay = true)]
        void GameOverCallback();
        //[OperationContract(IsOneWay = true)]
        //void EdgeXCoordCallback(int x);
    }
}
