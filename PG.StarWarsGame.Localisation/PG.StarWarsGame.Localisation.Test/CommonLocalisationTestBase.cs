// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT;
using PG.StarWarsGame.Localisation.IO.Csv;
using PG.StarWarsGame.Localisation.IO.Dat;
using PG.StarWarsGame.Localisation.IO.Properties;
using PG.StarWarsGame.Localisation.IO.Xml;
using PG.StarWarsGame.Localisation.Languages;
using PG.StarWarsGame.Localisation.Services;
using PG.Testing;

namespace PG.StarWarsGame.Localisation.Test;

public abstract class CommonLocalisationTestBase : PGTestBase
{
    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportDAT();

        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new EnglishAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new GermanAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new FrenchAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new SpanishAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new ItalianAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new ChineseAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new JapaneseAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new KoreanAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new PolishAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new RussianAlamoLanguageDefinition());
        serviceCollection.AddSingleton<IAlamoLanguageDefinition>(new ThaiAlamoLanguageDefinition());

        serviceCollection.AddSingleton<ILanguageService>(sp =>
            new LanguageService(sp.GetServices<IAlamoLanguageDefinition>()));

        serviceCollection.AddTransient<IDatTranslationImporter, DatTranslationImporter>();
        serviceCollection.AddTransient<IDatTranslationExporter, DatTranslationExporter>();
        serviceCollection.AddTransient<IXmlTranslationImporter, XmlTranslationImporter>();
        serviceCollection.AddTransient<IXmlTranslationExporter, XmlTranslationExporter>();
        serviceCollection.AddTransient<ICsvTranslationImporter, CsvTranslationImporter>();
        serviceCollection.AddTransient<ICsvTranslationExporter, CsvTranslationExporter>();
        serviceCollection.AddTransient<IPropertiesTranslationImporter, PropertiesTranslationImporter>();
        serviceCollection.AddTransient<IPropertiesTranslationExporter, PropertiesTranslationExporter>();
    }
}
