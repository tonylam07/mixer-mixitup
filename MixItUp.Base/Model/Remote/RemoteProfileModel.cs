using System;
using System.Runtime.Serialization;

namespace MixItUp.Base.Model.Remote
{
    [DataContract]
    public class RemoteProfileModel
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public Guid VersionID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public RemoteBoardModel MainBoard { get; set; }

        public RemoteProfileModel()
        {
            this.VersionID = Guid.NewGuid();
        }

        public RemoteProfileModel(string name)
            : this()
        {
            this.ID = Guid.NewGuid();
            this.Name = name;
            this.MainBoard = new RemoteBoardModel("Default");
        }

        public RemoteBoardModel ToSimpleModel()
        {
            return new RemoteBoardModel() { ID = this.ID, Name = this.Name };
        }
    }
}
