using MixItUp.Base.Commands;
using MixItUp.Base.Model.Remote;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MixItUp.Base.Services
{
    public interface IMixItUpRemoteService
    {
        bool Connected { get; }

        Guid SessionID { get; }

        DateTimeOffset ServerLastSeen { get; }

        string AccessCode { get; }
        DateTimeOffset AccessCodeExpiration { get; }

        Guid ClientID { get; }
        string ClientName { get; }
        DateTimeOffset ClientAuthExpiration { get; }

        event EventHandler<HeartbeatRemoteMessage> OnHeartbeat;
        event EventHandler<AuthRequestRemoteMessage> OnAuthRequest;
        event EventHandler<AccessCodeNewRemoteMessage> OnNewAccessCode;

        event EventHandler<RemoteBoardModel> OnBoardRequest;
        event EventHandler<RemoteCommand> OnActionRequest;

        event EventHandler<string> OnSentOccurred;
        event EventHandler<string> OnReceivedOccurred;
        event EventHandler<WebSocketCloseStatus> OnDisconnectOccurred;

        Task<bool> Connect();

        Task<bool> Connect(string endpoint);

        Task Disconnect(WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure);

        Task SendAuthClientGrant(AuthRequestRemoteMessage authRequest);

        Task SendAuthClientDeny();

        Task SendBoardDetail(RemoteBoardModel board);

        Task SendActionAck(Guid componentID);
    }
}
