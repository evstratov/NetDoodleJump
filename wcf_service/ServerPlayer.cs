using System.ServiceModel;


namespace wcf_service
{
    class ServerPlayer
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public OperationContext operationContext { get; set; }
    }
}
