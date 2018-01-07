using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MixItUp.Base.Services
{
    [DataContract]
    public class XSplitScene
    {
        [DataMember]
        public string sceneName;
    }

    [DataContract]
    public class XSplitSource
    {
        [DataMember]
        public string sourceName;
        [DataMember]
        public bool sourceVisible;
    }

    [DataContract]
    public class XSplitWebBrowserSource : XSplitSource
    {
        [DataMember]
        public string webBrowserUrl;
    }

    [DataContract]
    public class XSplitSourceDimensions
    {
        [DataMember]
        public string Name;
        [DataMember]
        public float Top;
        [DataMember]
        public float Bottom;
        [DataMember]
        public float Left;
        [DataMember]
        public float Right;
        [DataMember]
        public float RotateX;
        [DataMember]
        public float RotateY;
        [DataMember]
        public float RotateZ;
    }

    public interface IXSplitService
    {
        event EventHandler Disconnected;

        Task<bool> Initialize();

        Task<bool> TestConnection();

        Task<XSplitSourceDimensions> GetSourceDimensions(XSplitSource source);

        Task SetCurrentScene(XSplitScene scene);

        Task SetSourceVisibility(XSplitSource source);

        Task SetWebBrowserSource(XSplitWebBrowserSource source);

        Task SetSourceDimensions(XSplitSourceDimensions dimensions);

        Task Disconnect();
    }
}
