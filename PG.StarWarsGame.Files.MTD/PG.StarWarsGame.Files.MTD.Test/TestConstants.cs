// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MTD.Test
{
    public static class TestConstants
    {
        public static class MtdImageTableRecordTestConstants
        {
            public const int OBJECT_SIZE_IN_BYTE = 81;
            public const string DEFAULT_TEST_RECORD_NAME = "DEFAULT_TEST_RECORD";
            public const uint DEFAULT_TEST_RECORD_X_POSITION = 5;
            public const uint DEFAULT_TEST_RECORD_Y_POSITION = 5;
            public const uint DEFAULT_TEST_RECORD_X_EXTEND = 1;
            public const uint DEFAULT_TEST_RECORD_Y_EXTEND = 1;
            public const bool DEFAULT_TEST_RECORD_ALPHA = false;

            public static readonly byte[] EXPECTED_MTD_IMAGE_TABLE_RECORD_AS_BYTES =
            {
                68, 69, 70, 65, 85, 76, 84, 95, 84, 69, 83, 84, 95, 82, 69, 67, 79, 82, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 5, 0, 0, 0, 5, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0,
            };
        }

        public static class MtdImageTableTestConstants
        {
            public static readonly byte[] DEFAULT_TO_BYTES =
            {
                68, 69, 70, 65, 85, 76, 84, 95, 84, 69, 83, 84, 95, 82, 69, 67, 79, 82, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 5, 0, 0, 0, 5, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 68, 69, 70, 65, 85, 76, 84, 95, 84, 69, 83, 84,
                95, 82, 69, 67, 79, 82, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 5, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0,
                0, 0,
            };
        }

        public static class MtdFileTestConstants
        {
            public static readonly byte[] METADATA_TO_BYTES =
            {
                2, 0, 0, 0, 68, 69, 70, 65, 85, 76, 84, 95, 84, 69, 83, 84, 95, 82, 69, 67, 79, 82, 68, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 5, 0, 0, 0, 5, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 68, 69, 70, 65, 85, 76, 84, 95, 84,
                69, 83, 84, 95, 82, 69, 67, 79, 82, 68, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 5, 0, 0, 0, 1, 0, 0,
                0, 1, 0, 0, 0, 0,
            };
        }
    }
}
