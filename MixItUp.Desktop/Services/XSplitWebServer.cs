using Mixer.Base.Model.Client;
using MixItUp.Base.Services;
using MixItUp.Base.Util;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace MixItUp.XSplit
{
    public class XSplitPacket : WebSocketPacket
    {
        public JObject data;

        public XSplitPacket(string type, JObject data)
        {
            this.type = type;
            this.data = data;
        }
    }

    public class XSplitWebServer : WebSocketServerBase, IXSplitService
    {
        private XSplitSourceDimensions lastSourceDimensions = null;

        public XSplitWebServer(string address) : base(address) { }

        public async Task<XSplitSourceDimensions> GetSourceDimensions(XSplitSource source)
        {
            this.lastSourceDimensions = null;
            await this.Send(new XSplitPacket("getSourceDimensions", JObject.FromObject(source)));
            for (int i = 0; i < 10 && this.lastSourceDimensions == null; i++)
            {
                await Task.Delay(250);
            }
            return this.lastSourceDimensions;
        }

        public async Task SetCurrentScene(XSplitScene scene) { await this.Send(new XSplitPacket("sceneTransition", JObject.FromObject(scene))); }

        public async Task SetSourceVisibility(XSplitSource source) { await this.Send(new XSplitPacket("sourceUpdate", JObject.FromObject(source))); }

        public async Task SetWebBrowserSource(XSplitWebBrowserSource source) { await this.Send(new XSplitPacket("sourceUpdate", JObject.FromObject(source))); }

        public async Task SetSourceDimensions(XSplitSourceDimensions dimensions) { await this.Send(new XSplitPacket("setSourceDimensions", JObject.FromObject(dimensions))); }

        protected override async Task PacketReceived(string packet)
        {
            if (packet != null)
            {
                JObject packetObj = JObject.Parse(packet);
                if (packetObj["type"].ToString().Equals("getSourceDimensions") && packetObj["data"] != null)
                {
                    lastSourceDimensions = packetObj["data"].ToObject<XSplitSourceDimensions>();
                }
            }
            await base.PacketReceived(packet);
        }
    }
}
