using System;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Files;

namespace PG.StarWarsGame.Files.ALO.Files;

public sealed record AloFileInformation : PetroglyphMegPackableFileInformation
{
    /// <summary>
    /// Gets the model's content version.
    /// </summary>
    public AloContentInfo ContentInfo { get; }

    public bool IsModel => ContentInfo.Type == AloType.Model;

    public bool IsParticle => !IsModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="AloFileInformation"/> class.
    /// </summary>
    /// <param name="path">The file path of the alo file.</param>
    /// <param name="isInMeg">Information whether this file info is created from a meg data entry.</param>
    /// <param name="contentInfo">The content information version of the ALO file.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    [SetsRequiredMembers]
    public AloFileInformation(string path, bool isInMeg, AloContentInfo contentInfo) : base(path, isInMeg)
    {
        ContentInfo = contentInfo;
    }
}