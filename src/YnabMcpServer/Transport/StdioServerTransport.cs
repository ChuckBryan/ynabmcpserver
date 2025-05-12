using ModelContextProtocol.Transport;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YnabMcpServer.Transport;

public class StdioServerTransport : IServerTransport
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly SemaphoreSlim _writeSemaphore = new SemaphoreSlim(1, 1);

    public StdioServerTransport() : this(Console.In, Console.Out)
    {
    }

    public StdioServerTransport(TextReader input, TextWriter output)
    {
        _input = input;
        _output = output;
    }

    public async Task<string?> ReadMessageAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _input.ReadLineAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    public async Task WriteMessageAsync(string message, CancellationToken cancellationToken)
    {
        try
        {
            await _writeSemaphore.WaitAsync(cancellationToken);
            await _output.WriteLineAsync(new StringBuilder(message));
            await _output.FlushAsync();
        }
        finally
        {
            _writeSemaphore.Release();
        }
    }

    public void Dispose()
    {
        _writeSemaphore.Dispose();
    }
}
