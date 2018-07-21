using MixItUp.Base.Commands;
using MixItUp.Base.Model.Remote;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MixItUp.Base.Services
{
    public class StreamDeckKeyEvent
    {
        public int KeyID { get; set; }
        public bool IsDown { get; set; }

        public StreamDeckKeyEvent(int keyID, bool isDown)
        {
            this.KeyID = keyID;
            this.IsDown = isDown;
        }
    }

    public interface IStreamDeckService
    {
        event EventHandler<bool> ConnectionStateChangeOccurred;
        event EventHandler<StreamDeckKeyEvent> KeyChangeOccurred;
        event EventHandler<RemoteCommand> CommandRun;

        Task<bool> Connect(RemoteBoardModel board);
        Task Disconnect();

        Task SetBrightness(int brightness);

        Task SetKeyColor(int keyID, byte r, byte g, byte b);
        Task SetKeyBitmapBytes(int keyID, byte[] data);
        Task SetKeyBitmapStream(int keyID, Stream stream);
        Task SetKeyImage(int keyID, string imageFilePath);

        Task ClearKey(int keyID);
        Task ClearAllKeys();

        Task<int> GetKeyCount();
        Task<int> GetIconSize();

        Tuple<int, int> GetPositionIndexForKeyID(int keyID);
    }
}
