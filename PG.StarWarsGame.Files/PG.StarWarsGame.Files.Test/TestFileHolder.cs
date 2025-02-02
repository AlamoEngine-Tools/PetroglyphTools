using System;

namespace PG.StarWarsGame.Files.Test;

public class TestFileHolder<TModel, TFileInfo>(TModel model, TFileInfo fileInformation, IServiceProvider serviceProvider)
    : PetroglyphFileHolder<TModel, TFileInfo>(model, fileInformation, serviceProvider) 
    where TModel : notnull
    where TFileInfo : PetroglyphFileInformation;