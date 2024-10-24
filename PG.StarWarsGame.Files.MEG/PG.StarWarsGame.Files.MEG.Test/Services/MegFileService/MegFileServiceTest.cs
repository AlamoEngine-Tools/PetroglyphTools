using System;
using System.IO.Abstractions;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Services;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public partial class MegFileServiceTest
{
    private readonly MegFileService _megFileService = null!;
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IMegBinaryServiceFactory> _binaryServiceFactory = new();
    private readonly Mock<IMegFileBinaryReader> _megBinaryReader = new();
    private readonly Mock<IMegBinaryConverter> _megBinaryConverter = new();
    private readonly Mock<IMegBinaryValidator> _binaryValidator = new();
    private readonly Mock<IMegVersionIdentifier> _versionIdentifier = new();
    private readonly Mock<IConstructingMegArchiveBuilder> _constructingArchiveBuilder = new();
    private readonly Mock<IMegDataStreamFactory> _streamFactory = new();

    public MegFileServiceTest()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegBinaryServiceFactory))).Returns(_binaryServiceFactory.Object);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegBinaryValidator))).Returns(_binaryValidator.Object);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegDataStreamFactory))).Returns(_streamFactory.Object);
        _megFileService = new MegFileService(_serviceProvider.Object);
    }
}