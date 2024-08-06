using System;
using PG.Commons.Files;
using PG.StarWarsGame.Files.MTD.Data;

namespace PG.StarWarsGame.Files.MTD.Files;

/// <inheritdoc cref="IMtdFile"/>
public sealed class MtdFile(IMegaTextureDirectory model, MtdFileInformation fileInfo, IServiceProvider serviceProvider)
    : PetroglyphFileHolder<IMegaTextureDirectory, MtdFileInformation>(model, fileInfo, serviceProvider), IMtdFile;