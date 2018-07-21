using MixItUp.Base.Commands;
using MixItUp.Base.Model.Remote;
using MixItUp.Base.Services;
using MixItUp.Base.Util;
using StreamDeckSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MixItUp.Desktop.Services
{
    public class StreamDeckService : IStreamDeckService
    {
        public event EventHandler<bool> ConnectionStateChangeOccurred = delegate { };
        public event EventHandler<StreamDeckKeyEvent> KeyChangeOccurred = delegate { };
        public event EventHandler<RemoteCommand> CommandRun = delegate { };

        private string deviceName;

        private RemoteBoardModel board;

        private IStreamDeck deck;

        public StreamDeckService() { }

        public StreamDeckService(string deviceName)
        {
            this.deviceName = deviceName;
        }

        public async Task<bool> Connect(RemoteBoardModel board)
        {
            this.board = board;

            bool result = false;
            try
            {
                this.deck = StreamDeck.OpenDevice();
                await this.ConnectionWrapper((deck) =>
                {
                    deck.ConnectionStateChanged -= Deck_ConnectionStateChanged;
                    deck.KeyStateChanged -= Deck_KeyStateChanged;

                    //deck.ShowLogo();

                    deck.ConnectionStateChanged += Deck_ConnectionStateChanged;
                    deck.KeyStateChanged += Deck_KeyStateChanged;

                    result = true;

                    return Task.FromResult(0);
                });
            }
            catch (Exception ex) { Logger.Log(ex); }
            return result;
        }

        public async Task Disconnect()
        {
            await this.ConnectionWrapper((deck) =>
            {
                deck.ClearKeys();
                deck.ConnectionStateChanged -= Deck_ConnectionStateChanged;
                deck.KeyStateChanged -= Deck_KeyStateChanged;
                return Task.FromResult(0);
            });

            this.deck.Dispose();
            this.deck = null;
        }

        public async Task SetBrightness(int brightness)
        {
            await this.ConnectionWrapper((deck) =>
            {
                deck.SetBrightness((byte)MathHelper.Clamp(brightness, 0, 100));
                return Task.FromResult(0);
            });
        }

        public async Task SetKeyColor(int keyID, byte r, byte g, byte b)
        {
            await this.ConnectionWrapper((deck) =>
            {
                KeyBitmap bitmap = KeyBitmap.FromRGBColor(r, g, b);
                deck.SetKeyBitmap(keyID, bitmap);
                return Task.FromResult(0);
            });
        }

        public async Task SetKeyBitmapBytes(int keyID, byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                await this.SetKeyBitmapStream(keyID, stream);
            }
        }

        public async Task SetKeyBitmapStream(int keyID, Stream stream)
        {
            await this.ConnectionWrapper((deck) =>
            {
                KeyBitmap bitmap = KeyBitmap.FromStream(stream);
                deck.SetKeyBitmap(keyID, bitmap);
                return Task.FromResult(0);
            });
        }

        public async Task SetKeyImage(int keyID, string imageFilePath)
        {
            await this.ConnectionWrapper((deck) =>
            {
                KeyBitmap bitmap = KeyBitmap.FromFile(imageFilePath);
                deck.SetKeyBitmap(keyID, bitmap);
                return Task.FromResult(0);
            });
        }

        public async Task ClearKey(int keyID)
        {
            await this.ConnectionWrapper((deck) =>
            {
                deck.ClearKey(keyID);
                return Task.FromResult(0);
            });
        }

        public async Task ClearAllKeys()
        {
            await this.ConnectionWrapper((deck) =>
            {
                deck.ClearKeys();
                return Task.FromResult(0);
            });
        }

        public async Task<int> GetKeyCount()
        {
            int result = 0;
            await this.ConnectionWrapper((deck) =>
            {
                result = deck.KeyCount;
                return Task.FromResult(0);
            });
            return result;
        }

        public async Task<int> GetIconSize()
        {
            int result = 0;
            await this.ConnectionWrapper((deck) =>
            {
                result = deck.IconSize;
                return Task.FromResult(0);
            });
            return result;
        }

        public Tuple<int, int> GetPositionIndexForKeyID(int keyID)
        {
            return new Tuple<int, int>(4 - keyID % 5, keyID / 5);
        }

        private async Task ConnectionWrapper(Func<IStreamDeck, Task> action)
        {
            try
            {
                if (this.deck != null && deck.IsConnected)
                {
                    await action(deck);
                }
            }
            catch (Exception ex) { Logger.Log(ex); }
        }

        private void Deck_ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            this.ConnectionStateChangeOccurred(this, e.NewConnectionState);
        }

        private async void Deck_KeyStateChanged(object sender, KeyEventArgs e)
        {
            this.KeyChangeOccurred(this, new StreamDeckKeyEvent(e.Key, e.IsDown));

            if (e.IsDown)
            {
                Tuple<int, int> keyPosition = this.GetPositionIndexForKeyID(e.Key);
                foreach (RemoteItemModel item in this.board.Items)
                {
                    if (item.Command != null && item.XPosition == keyPosition.Item1 && item.YPosition == keyPosition.Item2)
                    {
                        await item.Command.Perform();

                        this.CommandRun(this, item.Command);
                    }
                }
            }
        }
    }
}
