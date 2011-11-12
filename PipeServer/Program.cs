using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

class PipeServer
{
    static int numThreads = 1;

    private static Mutex mut = new Mutex(true);

    static void Main()
    {
        while (true)
        {
            Thread newThread = new Thread(new ThreadStart(ServerThread));
            mut.ReleaseMutex();
            newThread.Start();
            Thread.Sleep(100);
            mut.WaitOne();
        }
        //Console.WriteLine("Press enter to exit.");
        //Console.ReadLine();
    } // Main()

    private static void ServerThread()
    {
        mut.WaitOne();
        using (NamedPipeServerStream pipeServer =
            new NamedPipeServerStream(@"migration-dds", PipeDirection.InOut, numThreads))
        {
            Console.WriteLine("NamedPipeServerStream thread created.");

            // Wait for a client to connect
            pipeServer.WaitForConnection();
            mut.ReleaseMutex();

            Console.WriteLine("Client connected.");
            try
            {
                // Read the request from the client. Once the client has
                // written to the pipe its security token will be available.
                using (StreamReader sr = new StreamReader(pipeServer))
                using (StreamWriter sw = new StreamWriter(pipeServer))
                {
                    sw.AutoFlush = true;

                    // Verify our identity to the connected client using a
                    // string that the client anticipates.
                    //sw.WriteLine("I am the true server!");

                    // Obtain the filename from the connected client.
                    string ServerName = sr.ReadLine();
                    string DatabaseName = sr.ReadLine();
                    string EventType = sr.ReadLine();
                    string ObjectName = sr.ReadLine();
                    string SchemaName = sr.ReadLine();

                    sw.WriteLine(@"Как бы сгенерированный скрипт:)))");
                    
                    pipeServer.Disconnect();
                }
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (IOException e)
            {
                Console.WriteLine("ERROR: {0}", e.Message);
            }
        }
    } // ServerThread()
} // PipeServer