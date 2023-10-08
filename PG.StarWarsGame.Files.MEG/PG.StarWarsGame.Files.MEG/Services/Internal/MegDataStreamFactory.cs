using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace PG.StarWarsGame.Files.MEG.Services;

internal sealed class MegDataStreamFactory : IMegDataStreamFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MegDataStreamFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Stream CreateDataStream(string path, uint offset, uint size)
    {
        var fs = _serviceProvider.GetRequiredService<IFileSystem>();
        return new MegFileDataStream(fs.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read), offset, size);
    }
}