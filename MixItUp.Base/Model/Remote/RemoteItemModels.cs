using MixItUp.Base.Commands;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace MixItUp.Base.Model.Remote
{
    [DataContract]
    public class RemoteItemModel
    {
        public const int RequiredSize = 72;

        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public RemoteBoardItemSizeEnum Size { get; set; }

        [DataMember]
        public int XPosition { get; set; }
        [DataMember]
        public int YPosition { get; set; }

        [JsonIgnore]
        public RemoteCommand Command { get { return ChannelSession.Settings.RemoteCommands.FirstOrDefault(c => c.ID.Equals(this.ID)); } }

        public RemoteItemModel() { }

        public virtual void SetValuesFromCommand()
        {
            RemoteCommand command = this.Command;
            if (command != null)
            {
                this.Name = command.Name;
            }
        }
    }

    [DataContract]
    public class RemoteButtonItemModel : RemoteItemModel
    {
        [DataMember]
        public string BackgroundColor { get; set; }

        [DataMember]
        public string TextColor { get; set; }

        [DataMember]
        public byte[] ImageData { get; set; }

        public RemoteButtonItemModel() { }

        public RemoteButtonItemModel(RemoteCommand command, int xPosition, int yPosition)
        {
            this.ID = command.ID;
            this.Name = command.Name;
            this.XPosition = xPosition;
            this.YPosition = yPosition;

            this.SetValuesFromCommand();
        }

        public override void SetValuesFromCommand()
        {
            base.SetValuesFromCommand();

            RemoteCommand command = this.Command;
            if (command != null)
            {
                this.BackgroundColor = command.BackgroundColor;
                this.TextColor = command.TextColor;
                this.ImageData = command.ImageData;
            }
        }
    }

    [DataContract]
    public class RemoteFolderItemModel : RemoteButtonItemModel
    {
        [DataMember]
        public RemoteBoardModel Board { get; set; }

        public RemoteFolderItemModel() { }

        public RemoteFolderItemModel(string name, int xPosition, int yPosition)
        {
            this.ID = Guid.NewGuid();
            this.Name = name;
            this.XPosition = xPosition;
            this.YPosition = yPosition;

            this.Board = new RemoteBoardModel(this.Name);
            this.Board.IsSubBoard = true;
        }
    }
}
