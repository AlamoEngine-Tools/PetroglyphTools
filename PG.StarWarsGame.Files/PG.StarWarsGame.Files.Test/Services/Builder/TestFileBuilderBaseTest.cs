namespace PG.StarWarsGame.Files.Test.Services.Builder;

public class TestFileBuilderBaseTest : FileBuilderTestBase<TestFileBuilder, byte[], TestFileInfo>
{
    protected override TestFileBuilder CreateBuilder()
    {
        return new TestFileBuilder(ServiceProvider);
    }

    protected override TestFileInfo CreateFileInfo(bool valid, string path)
    {
        return TestFileInfo.Create(path, valid);
    }

    protected override void AddDataToBuilder(byte[] data, TestFileBuilder builder)
    {
        builder.SetData(data);
    }

    protected override (byte[] Data, byte[] Bytes) CreateValidData()
    {
        var bytes = new byte[] { 1, 2, 3, 4, 5, 6 };
        return (bytes, bytes);
    }

    protected override byte[] CreateInvalidData()
    {
        return null!;
    }
}