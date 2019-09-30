using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceGame" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IServerGameCallback))]
    public interface IServiceGame
    {
        [OperationContract]
        int Connect(string name);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendPlayerInfo(object info, int id);
    }
    public interface IServerGameCallback
    {
        [OperationContract(IsOneWay = true)]
        void PlayerInfoCallback(object info);
    }
}
