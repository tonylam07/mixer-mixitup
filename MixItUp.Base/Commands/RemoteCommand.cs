using System.Runtime.Serialization;
using System.Threading;

namespace MixItUp.Base.Commands
{
    [DataContract]
    public class RemoteCommand : CommandBase
    {
        private static SemaphoreSlim remoteCommandPerformSemaphore = new SemaphoreSlim(1);

        [DataMember]
        public string BackgroundColor { get; set; }

        [DataMember]
        public string TextColor { get; set; }

        [DataMember]
        public string ImageName { get; set; }

        public RemoteCommand() { }

        public RemoteCommand(string name)
            : base(name, CommandTypeEnum.Remote, name)
        {
            this.BackgroundColor = "#FFFFFF";
            this.TextColor = "#000000";
        }

        protected override SemaphoreSlim AsyncSemaphore { get { return RemoteCommand.remoteCommandPerformSemaphore; } }
    }
}
