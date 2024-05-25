using System.IO;
using PG.StarWarsGame.Files.ALO.Files;

namespace PG.StarWarsGame.Files.ALO.Binary.Identifier;

interface IAloContentInfoIdentifier
{
    AloContentInfo GetContentInfo(Stream stream);
}