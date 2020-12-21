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

        public BlockingCollection<byte[]> MessageQueue { get; } = new BlockingCollection<byte[]>();

        public CancellationTokenSource BroadcastLoopTokenSource { get; set; } = new CancellationTokenSource();

        public async Task BroadcastLoopAsync()
        {
            var cancellationToken = BroadcastLoopTokenSource.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!cancellationToken.IsCancellationRequested && Socket.State == WebSocketState.Open && MessageQueue.TryTake(out byte[] message))
                    {
                        GameProcess gameProcess = GameProcess.Instance;

                        Console.WriteLine($"Socket {SocketId}: Sending from queue.");
                        string gameState = gameProcess.GetGameManager().GetDebugString();

                        this.workingByteList.Clear();
                        byte[] sandboxBytes = Encoding.UTF8.GetBytes(gameProcess.GetGameManager().GetDebugString());
                        await Socket.SendAsync(sandboxBytes, WebSocketMessageType.Binary, endOfMessage: true, CancellationToken.None);
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
