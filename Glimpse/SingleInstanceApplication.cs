using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glimpse
{
    class SingleInstanceApplication
    {
        private const string GlimpsePipeName = "Glimpse_F90238A6-4DE1-4D5B-B5A5-3BCC43EDBDCE";

        public static event EventHandler<string[]> CommandReceived;
        private bool stopServer = false;

        public void RunAsMaster()
        {
            if (IsMasterInstanceRunning())
                throw new InvalidOperationException("Theres already a master instance running.");

            Task.Run(() =>
            {
                StartPipeServer();
            });
        }

        public void StopServer()
        {
            this.stopServer = true;

            try
            {
                using (var nps = new NamedPipeClientStream(".", GlimpsePipeName, PipeDirection.Out))
                {
                    // connect client to un-block WaitForConnection
                    nps.Connect(100);
                }
            }
            catch (Exception)
            {
                // we can ignore any exception; we just wanted to unblock the server thread
            }
        }

        private void StartPipeServer()
        {
            try
            {
                using (var pipeServer = new NamedPipeServerStream(GlimpsePipeName, PipeDirection.In))
                {
                    while (!stopServer)
                    {
                        try
                        {
                            pipeServer.WaitForConnection();

                            CommandStream cmdStream = new CommandStream(pipeServer);
                            string[] args = cmdStream.ReceiveArgs();

                            if (CommandReceived != null && args != null)
                            {
                                CommandReceived(this, args);
                            }
                        }
                        finally
                        {
                            pipeServer.Disconnect();
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Master failed to connect with exception: {0}", ex);
            }
        }

        public void RunAsSlave(string[] args)
        {
            if (args == null)
                return;
            if (args.Length == 0)
                return;

            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", GlimpsePipeName, PipeDirection.Out))
                {
                    pipeClient.Connect();

                    CommandStream cmdStream = new CommandStream(pipeClient);
                    cmdStream.SendArgs(args);
                    cmdStream.Flush();
                }
            }
            catch (TimeoutException)
            {
                System.Diagnostics.Debug.WriteLine("Connect to namedpipe server timed out.");
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to connect to namedpipe with exception: {0}", ex);
            }
        }

        public bool IsMasterInstanceRunning()
        {
            return File.Exists(@"\\.\pipe\" + GlimpsePipeName);
        }

        class CommandStream
        {
            private Stream baseStream;
            private Encoding encoding;

            public CommandStream(Stream stream)
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                this.baseStream = stream;
                this.encoding = Encoding.UTF8;
            }

            public void Flush()
            {
                baseStream.Flush();
            }

            public void SendArgs(string[] args)
            {
                if (args == null)
                    throw new ArgumentNullException("args");

                string messageText = string.Join(Environment.NewLine, args);

                byte[] message = this.encoding.GetBytes(messageText);
                byte[] messageSize = BitConverter.GetBytes(message.Length);

                baseStream.Write(messageSize, 0, messageSize.Length);
                baseStream.Write(message, 0, message.Length);
            }

            public string[] ReceiveArgs()
            {
                byte[] sizeBuffer = new byte[4];
                baseStream.Read(sizeBuffer, 0, 4);
                int messageSize = BitConverter.ToInt32(sizeBuffer, 0);

                byte[] message = new byte[messageSize];
                baseStream.Read(message, 0, message.Length);

                string messageText = this.encoding.GetString(message);
                if (!string.IsNullOrEmpty(messageText))
                {
                    string[] args = messageText.Split(Environment.NewLine);
                    return args;
                }

                return null;
            }
        }
    }
}
