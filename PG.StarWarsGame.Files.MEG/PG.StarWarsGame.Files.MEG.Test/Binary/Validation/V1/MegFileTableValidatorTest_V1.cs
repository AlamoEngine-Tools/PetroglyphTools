using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation.V1;

public class MegFileTableValidatorTest_V1 : MegFileTableValidatorTestBase
{
    private protected override IMegFileTable CreateFileTable(IList<TestFileDescriptorInfo> files)
    {
        return new MegFileTable(files
            .Select(e =>
                new MegFileTableRecord(e.Crc32, e.Index, e.FileSize, e.FileOffset, e.FileNameIndex))
            .ToList());
    }
}