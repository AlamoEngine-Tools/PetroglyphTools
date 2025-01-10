using System;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.Test.Services.Builder;

public class TestFileBuilder(IServiceProvider services) : FileBuilderBase<byte[], TestFileInfo>(services)
{
    private byte[] _builderData = null!;

    public override byte[] BuilderData => _builderData;

    public void SetData(byte[] data) => _builderData = data;

    private string? FileWritten { get; set; }

    protected override void BuildFileCore(FileSystemStream fileStream, TestFileInfo fileInformation, byte[] data)
    {
        Assert.True(fileInformation.IsValid);
        fileStream.Write(data, 0, data.Length);
        FileWritten = fileStream.Name;
    }

    protected override bool ValidateFileInformationCore(TestFileInfo fileInformation, byte[] builderData, out string? failedReason)
    {
        failedReason = string.Empty;
        return fileInformation.IsValid;
    }

    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        if (string.IsNullOrEmpty(FileWritten)) 
            Assert.False(FileSystem.File.Exists(FileWritten));
    }
}