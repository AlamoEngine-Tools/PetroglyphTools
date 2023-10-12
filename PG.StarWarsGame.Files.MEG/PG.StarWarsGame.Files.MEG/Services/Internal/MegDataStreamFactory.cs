using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services;

internal sealed class MegDataStreamFactory : IMegDataStreamFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MegDataStreamFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public Stream CreateDataStream(string path, uint offset, uint size)
    {
        // Cause MIKE.NL's tool uses the offset megFile[megSize + 1] for empty Files we would cause an ArgumentOutOfRangeException
        // when trying to access this index on a real file. Therefore we return the Null stream.
        if (size == 0)
            return Stream.Null;

        var fs = _serviceProvider.GetRequiredService<IFileSystem>();
        return new MegFileDataStream(fs.FileStream.New(path, FileMode.Open, FileAccess.Read, FileShare.Read), offset, size);
    }
}