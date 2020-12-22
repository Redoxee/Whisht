using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebWist
{
    public class ConnectedClient
    {
        public int PlayerIndex = -1;

        System.Collections.Generic.List<byte> workingByteList = new System.Collections.Generic.List<byte>();

        public ConnectedClient(int socketId, WebSocket socket, TaskCompletionSource<object> taskCompletion)
        {
            SocketId = socketId;
            Socket = socket;
            TaskCompletion = taskCompletion;
        }

        public int SocketId { get; private set; }

        public WebSocket Socket { get; private set; }

        public TaskCompletionSource<object> TaskCompletion { get; private set; }

        public BlockingCollection<string> MessageQueue { get; } = new BlockingCollection<string>();

        public CancellationTokenSource BroadcastLoopTokenSource { get; set; } = new CancellationTokenSource();

        public async Task BroadcastLoopAsync()
        {
            var cancellationToken = BroadcastLoopTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!cancellationToken.IsCancellationRequested && Socket.State == WebSocketState.Open && MessageQueue.TryTake(out string message))
                    {
                        GameProcess gameProcess = GameProcess.Instance;

                        Console.WriteLine($"Socket {SocketId}: Sending from queue.");
                        string gameState = gameProcess.GetGameManager().GetDebugString();

                        this.workingByteList.Clear();

                        Console.WriteLine($"Sending : {message}");
                        byte[] sandboxBytes = Encoding.UTF8.GetBytes(message);
                        await Socket.SendAsync(sandboxBytes, WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
                        // await Socket.SendAsync(Encoding.UTF8.GetBytes("Test message"), WebSocketMessageType.Text, endOfMessage : true, CancellationToken.None);
                    }
                }
                catch (OperationCanceledException)
                {
                    // normal upon task/token cancellation, disregard
                }
                catch (Exception ex)
                {
                    Program.ReportException(ex);
                }
            }
        }
    }
}
