namespace PG.StarWarsGame.Files.Test;

public record MegTestParam : PetroglyphMegPackableFileInformation
{
    public bool IsDisposed { get; private set; }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }
}