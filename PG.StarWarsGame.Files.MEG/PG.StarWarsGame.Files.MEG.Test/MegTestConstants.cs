// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test;

internal sealed class MegTestConstants : TestConstants
{
    internal static readonly byte[] ContentMegFileV1 =
    {
        2, 0, 0, 0, 2, 0, 0, 0, 28, 0, 68, 65, 84, 65, 47, 88, 77, 76, 47, 71, 65, 77, 69, 79, 66, 74, 69, 67, 84,
        70, 73, 76, 69, 83, 46, 88, 77, 76, 26, 0, 68, 65, 84, 65, 47, 88, 77, 76, 47, 67, 65, 77, 80, 65, 73, 71,
        78, 70, 73, 76, 69, 83, 46, 88, 77, 76, 146, 206, 127, 126, 0, 0, 0, 0, 158, 20, 0, 0, 106, 0, 0, 0, 0, 0,
        0, 0, 252, 90, 98, 183, 1, 0, 0, 0, 114, 1, 0, 0, 8, 21, 0, 0, 1, 0, 0, 0,
        60, 63, 120, 109, 108, 32, 118, 101, 114, 115, 105, 111, 110, 61, 34, 49, 46, 48, 34, 32, 63, 62, 10, 10,
        60, 71, 97, 109, 101, 95, 79, 98, 106, 101, 99, 116, 95, 70, 105, 108, 101, 115, 62, 10, 10, 9, 60, 70, 105,
        108, 101, 62, 84, 114, 97, 110, 115, 112, 111, 114, 116, 85, 110, 105, 116, 115, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 84, 101, 99, 104, 66, 117, 105, 108, 100, 105,
        110, 103, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 9, 9, 60, 33, 45, 45, 32, 65, 74, 65, 32, 49,
        48, 47, 49, 50, 47, 50, 48, 48, 53, 32, 45, 32, 77, 111, 118, 101, 100, 32, 116, 111, 32, 116, 104, 101, 32,
        116, 111, 112, 32, 115, 111, 32, 116, 104, 97, 116, 32, 116, 101, 99, 104, 32, 117, 112, 103, 114, 97, 100,
        101, 115, 32, 97, 108, 119, 97, 121, 115, 32, 115, 104, 111, 119, 32, 117, 112, 32, 111, 110, 32, 116, 104,
        101, 32, 108, 101, 102, 116, 32, 111, 102, 32, 116, 104, 101, 32, 98, 117, 105, 108, 100, 32, 98, 97, 114,
        46, 32, 45, 45, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 66, 97, 115, 101, 115,
        46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110,
        100, 83, 116, 114, 117, 99, 116, 117, 114, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10,
        9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 84, 117, 114, 114, 101, 116, 115, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 73, 110,
        102, 97, 110, 116, 114, 121, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101,
        62, 71, 114, 111, 117, 110, 100, 86, 101, 104, 105, 99, 108, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105,
        108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 67, 111, 109, 112, 97, 110,
        105, 101, 115, 82, 101, 98, 101, 108, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105,
        108, 101, 62, 71, 114, 111, 117, 110, 100, 67, 111, 109, 112, 97, 110, 105, 101, 115, 69, 109, 112, 105,
        114, 101, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111,
        117, 110, 100, 73, 110, 100, 105, 103, 101, 110, 111, 117, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108,
        101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 67, 111, 109, 112, 97, 110, 105,
        101, 115, 73, 110, 100, 105, 103, 101, 110, 111, 117, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62,
        10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 66, 117, 105, 108, 100, 97, 98, 108, 101,
        115, 82, 101, 98, 101, 108, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101,
        62, 71, 114, 111, 117, 110, 100, 66, 117, 105, 108, 100, 97, 98, 108, 101, 115, 69, 109, 112, 105, 114, 101,
        46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110,
        100, 66, 117, 105, 108, 100, 97, 98, 108, 101, 115, 80, 105, 114, 97, 116, 101, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 66, 117, 105, 108,
        100, 97, 98, 108, 101, 115, 83, 107, 105, 114, 109, 105, 115, 104, 46, 120, 109, 108, 60, 47, 70, 105, 108,
        101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 97, 99, 101, 80, 114, 111, 112, 115, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 97, 99, 101, 85, 110, 105,
        116, 115, 67, 97, 112, 105, 116, 97, 108, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70,
        105, 108, 101, 62, 83, 112, 97, 99, 101, 85, 110, 105, 116, 115, 70, 105, 103, 104, 116, 101, 114, 115, 46,
        120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 97, 99, 101, 85,
        110, 105, 116, 115, 67, 111, 114, 118, 101, 116, 116, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108,
        101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 97, 99, 101, 85, 110, 105, 116, 115, 70, 114, 105, 103,
        97, 116, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83,
        112, 97, 99, 101, 85, 110, 105, 116, 115, 83, 117, 112, 101, 114, 115, 46, 120, 109, 108, 60, 47, 70, 105,
        108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 97, 99, 101, 80, 114, 105, 109, 97, 114, 121, 83,
        107, 121, 100, 111, 109, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105,
        108, 101, 62, 83, 112, 97, 99, 101, 83, 101, 99, 111, 110, 100, 97, 114, 121, 83, 107, 121, 100, 111, 109,
        101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 97,
        99, 101, 66, 117, 105, 108, 100, 97, 98, 108, 101, 115, 83, 107, 105, 114, 109, 105, 115, 104, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 10, 9, 60, 70, 105, 108, 101, 62, 78, 97, 109, 101, 100, 72, 101,
        114, 111, 85, 110, 105, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108,
        101, 62, 71, 101, 110, 101, 114, 105, 99, 72, 101, 114, 111, 85, 110, 105, 116, 115, 46, 120, 109, 108, 60,
        47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 72, 101, 114, 111, 67, 111, 109, 112, 97, 110,
        105, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 76,
        97, 110, 100, 80, 114, 105, 109, 97, 114, 121, 83, 107, 121, 100, 111, 109, 101, 115, 46, 120, 109, 108, 60,
        47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 76, 97, 110, 100, 83, 101, 99, 111, 110, 100,
        97, 114, 121, 83, 107, 121, 100, 111, 109, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10,
        10, 9, 60, 70, 105, 108, 101, 62, 76, 97, 110, 100, 66, 111, 109, 98, 105, 110, 103, 82, 117, 110, 85, 110,
        105, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 83,
        112, 101, 99, 105, 97, 108, 83, 116, 114, 117, 99, 116, 117, 114, 101, 115, 46, 120, 109, 108, 60, 47, 70,
        105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 116, 97, 114, 66, 97, 115, 101, 115, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 83, 113, 117, 97, 100, 114, 111,
        110, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 80, 108,
        97, 110, 101, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101,
        62, 80, 114, 111, 106, 101, 99, 116, 105, 108, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62,
        10, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 101, 99, 105, 97, 108, 69, 102, 102, 101, 99, 116, 115, 46,
        120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 80, 97, 114, 116, 105, 99,
        108, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 77, 97,
        114, 107, 101, 114, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62,
        85, 110, 105, 113, 117, 101, 85, 110, 105, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10,
        9, 60, 70, 105, 108, 101, 62, 67, 111, 110, 116, 97, 105, 110, 101, 114, 115, 46, 120, 109, 108, 60, 47, 70,
        105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 99, 114, 105, 112, 116, 77, 97, 114, 107, 101, 114,
        115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 77, 105, 115,
        99, 79, 98, 106, 101, 99, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105,
        108, 101, 62, 85, 112, 103, 114, 97, 100, 101, 79, 98, 106, 101, 99, 116, 115, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 101, 99, 111, 110, 100, 97, 114, 121, 83, 116,
        114, 117, 99, 116, 117, 114, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70,
        105, 108, 101, 62, 80, 114, 111, 112, 115, 95, 84, 101, 109, 112, 101, 114, 97, 116, 101, 46, 120, 109, 108,
        60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 80, 114, 111, 112, 115, 95, 83, 119, 97,
        109, 112, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 80, 114, 111,
        112, 115, 95, 86, 111, 108, 99, 97, 110, 105, 99, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9,
        60, 70, 105, 108, 101, 62, 80, 114, 111, 112, 115, 95, 70, 111, 114, 101, 115, 116, 46, 120, 109, 108, 60,
        47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 80, 114, 111, 112, 115, 95, 83, 110, 111, 119,
        46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 80, 114, 111, 112, 115,
        95, 68, 101, 115, 101, 114, 116, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108,
        101, 62, 80, 114, 111, 112, 115, 95, 85, 114, 98, 97, 110, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62,
        10, 9, 60, 70, 105, 108, 101, 62, 80, 114, 111, 112, 115, 95, 71, 101, 110, 101, 114, 105, 99, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 80, 114, 111, 112, 115, 95, 83, 116,
        111, 114, 121, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 9, 10, 10, 9, 60, 70, 105, 108, 101, 62,
        67, 73, 78, 95, 83, 112, 97, 99, 101, 85, 110, 105, 116, 115, 67, 97, 112, 105, 116, 97, 108, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 83, 112, 97, 99, 101,
        85, 110, 105, 116, 115, 70, 105, 103, 104, 116, 101, 114, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 83, 112, 97, 99, 101, 85, 110, 105, 116, 115, 67, 111,
        114, 118, 101, 116, 116, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105,
        108, 101, 62, 67, 73, 78, 95, 83, 112, 97, 99, 101, 85, 110, 105, 116, 115, 70, 114, 105, 103, 97, 116, 101,
        115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 71,
        114, 111, 117, 110, 100, 73, 110, 102, 97, 110, 116, 114, 121, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 71, 114, 111, 117, 110, 100, 86, 101, 104, 105, 99,
        108, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 73,
        78, 95, 83, 112, 97, 99, 101, 80, 114, 111, 112, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10,
        9, 60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 84, 114, 97, 110, 115, 112, 111, 114, 116, 85, 110, 105, 116,
        115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 80,
        114, 111, 106, 101, 99, 116, 105, 108, 101, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9,
        60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 71, 114, 111, 117, 110, 100, 80, 114, 111, 112, 115, 46, 120,
        109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 73, 78, 95, 71, 114, 111,
        117, 110, 100, 84, 117, 114, 114, 101, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10,
        9, 60, 70, 105, 108, 101, 62, 77, 79, 86, 95, 67, 105, 110, 101, 109, 97, 116, 105, 99, 115, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 10, 10, 10, 10, 10, 9, 60, 33, 45, 45, 32, 69, 120, 112, 97,
        110, 115, 105, 111, 110, 32, 85, 110, 105, 116, 115, 58, 32, 69, 109, 112, 105, 114, 101, 32, 45, 45, 62,
        10, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95, 69, 109, 112, 105,
        114, 101, 95, 84, 104, 114, 97, 119, 110, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60,
        70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 69, 109, 112, 105, 114, 101,
        95, 84, 73, 69, 95, 68, 101, 102, 101, 110, 100, 101, 114, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62,
        10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 69, 109, 112, 105,
        114, 101, 95, 84, 73, 69, 95, 73, 110, 116, 101, 114, 99, 101, 112, 116, 111, 114, 46, 120, 109, 108, 60,
        47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99,
        101, 95, 69, 109, 112, 105, 114, 101, 95, 84, 73, 69, 95, 80, 104, 97, 110, 116, 111, 109, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112,
        97, 99, 101, 95, 69, 109, 112, 105, 114, 101, 95, 69, 120, 101, 99, 117, 116, 111, 114, 46, 120, 109, 108,
        60, 47, 70, 105, 108, 101, 62, 10, 9, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97,
        110, 100, 95, 69, 109, 112, 105, 114, 101, 95, 68, 97, 114, 107, 84, 114, 111, 111, 112, 101, 114, 115, 46,
        120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95,
        76, 97, 110, 100, 95, 69, 109, 112, 105, 114, 101, 95, 76, 97, 110, 99, 101, 116, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97, 110, 100, 95,
        69, 109, 112, 105, 114, 101, 95, 78, 111, 103, 104, 114, 105, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97, 110, 100, 95, 69, 109, 112, 105,
        114, 101, 95, 74, 117, 103, 103, 101, 114, 110, 97, 117, 116, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 10, 10, 10, 9, 60, 33, 45, 45, 32, 69, 120, 112, 97, 110, 115, 105, 111, 110, 32, 85, 110, 105, 116,
        115, 58, 32, 82, 101, 98, 101, 108, 32, 45, 45, 62, 10, 9, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105,
        116, 115, 95, 72, 101, 114, 111, 95, 82, 101, 98, 101, 108, 95, 89, 111, 100, 97, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95,
        82, 101, 98, 101, 108, 95, 76, 117, 107, 101, 83, 107, 121, 119, 97, 108, 107, 101, 114, 46, 120, 109, 108,
        60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114,
        111, 95, 82, 101, 98, 101, 108, 95, 71, 97, 114, 103, 97, 110, 116, 117, 97, 110, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95,
        82, 101, 98, 101, 108, 95, 82, 111, 103, 117, 101, 95, 83, 113, 117, 97, 100, 114, 111, 110, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76,
        97, 110, 100, 95, 82, 101, 98, 101, 108, 95, 71, 97, 108, 108, 111, 102, 114, 101, 101, 95, 72, 84, 84, 46,
        120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115,
        95, 83, 112, 97, 99, 101, 95, 82, 101, 98, 101, 108, 95, 66, 87, 105, 110, 103, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 9, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99,
        101, 95, 82, 101, 98, 101, 108, 95, 77, 67, 51, 48, 95, 70, 114, 105, 103, 97, 116, 101, 46, 120, 109, 108,
        60, 47, 70, 105, 108, 101, 62, 10, 10, 10, 10, 9, 60, 33, 45, 45, 32, 69, 120, 112, 97, 110, 115, 105, 111,
        110, 32, 85, 110, 105, 116, 115, 58, 32, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 32, 45, 45, 62,
        10, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95, 85, 110, 100, 101,
        114, 119, 111, 114, 108, 100, 95, 73, 71, 56, 56, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9,
        60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95, 85, 110, 100, 101, 114, 119,
        111, 114, 108, 100, 95, 66, 111, 115, 115, 107, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60,
        70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95, 85, 110, 100, 101, 114, 119, 111,
        114, 108, 100, 95, 84, 121, 98, 101, 114, 95, 90, 97, 110, 110, 46, 120, 109, 108, 60, 47, 70, 105, 108,
        101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95, 85, 110, 100,
        101, 114, 119, 111, 114, 108, 100, 95, 83, 105, 108, 114, 105, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 72, 101, 114, 111, 95, 85, 110, 100, 101,
        114, 119, 111, 114, 108, 100, 95, 85, 114, 97, 105, 95, 70, 101, 110, 46, 120, 109, 108, 60, 47, 70, 105,
        108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97, 110, 100, 95, 85,
        110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 77, 101, 114, 99, 101, 110, 97, 114, 121, 73, 110, 102, 46,
        120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95,
        76, 97, 110, 100, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 68, 105, 115, 114, 117, 112, 116,
        111, 114, 73, 110, 102, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62,
        85, 110, 105, 116, 115, 95, 76, 97, 110, 100, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 68,
        101, 115, 116, 114, 111, 121, 101, 114, 68, 114, 111, 105, 100, 115, 46, 120, 109, 108, 60, 47, 70, 105,
        108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97, 110, 100, 95, 85, 110,
        100, 101, 114, 119, 111, 114, 108, 100, 95, 86, 111, 114, 110, 115, 107, 114, 46, 120, 109, 108, 60, 47, 70,
        105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97, 110, 100, 95, 85,
        110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 70, 57, 84, 90, 95, 84, 114, 97, 110, 115, 112, 111, 114,
        116, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116,
        115, 95, 76, 97, 110, 100, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 77, 65, 76, 46, 120,
        109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76,
        97, 110, 100, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 80, 117, 108, 115, 101, 84, 97, 110,
        107, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116,
        115, 95, 76, 97, 110, 100, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 67, 97, 110, 100, 101,
        114, 111, 117, 115, 95, 84, 97, 110, 107, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70,
        105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97, 110, 100, 95, 85, 110, 100, 101, 114, 119, 111, 114,
        108, 100, 95, 83, 99, 97, 118, 101, 110, 103, 101, 114, 68, 114, 111, 105, 100, 115, 46, 120, 109, 108, 60,
        47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 76, 97, 110, 100,
        95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 78, 105, 103, 104, 116, 83, 105, 115, 116, 101,
        114, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116,
        115, 95, 76, 97, 110, 100, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 69, 119, 111, 107, 95,
        66, 111, 109, 98, 101, 114, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108,
        101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108,
        100, 95, 83, 116, 97, 114, 86, 105, 112, 101, 114, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9,
        60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100, 101, 114,
        119, 111, 114, 108, 100, 95, 83, 107, 105, 112, 114, 97, 121, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100,
        101, 114, 119, 111, 114, 108, 100, 95, 67, 114, 117, 115, 97, 100, 101, 114, 95, 71, 117, 110, 115, 104,
        105, 112, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105,
        116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 73, 110, 116,
        101, 114, 99, 101, 112, 116, 111, 114, 52, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70,
        105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100, 101, 114, 119, 111,
        114, 108, 100, 95, 86, 101, 110, 103, 101, 97, 110, 99, 101, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 9, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100,
        101, 114, 119, 111, 114, 108, 100, 95, 75, 97, 100, 97, 108, 98, 101, 95, 66, 97, 116, 116, 108, 101, 115,
        104, 105, 112, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110,
        105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 75, 114,
        97, 121, 116, 95, 68, 101, 115, 116, 114, 111, 121, 101, 114, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 85, 110, 100,
        101, 114, 119, 111, 114, 108, 100, 95, 66, 117, 122, 122, 95, 68, 114, 111, 105, 100, 115, 46, 120, 109,
        108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 101, 99, 105, 97, 108,
        83, 116, 114, 117, 99, 116, 117, 114, 101, 115, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 46,
        120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 83, 116, 97, 114, 66, 97,
        115, 101, 115, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 46, 120, 109, 108, 60, 47, 70, 105, 108,
        101, 62, 10, 10, 10, 10, 9, 60, 33, 45, 45, 32, 69, 120, 112, 97, 110, 115, 105, 111, 110, 32, 85, 110, 105,
        116, 115, 58, 32, 78, 101, 117, 116, 114, 97, 108, 32, 45, 45, 62, 10, 10, 9, 60, 70, 105, 108, 101, 62, 85,
        110, 105, 116, 115, 95, 83, 112, 97, 99, 101, 95, 78, 101, 117, 116, 114, 97, 108, 95, 69, 99, 108, 105,
        112, 115, 101, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 72, 117,
        116, 116, 95, 70, 97, 99, 116, 105, 111, 110, 95, 85, 110, 105, 116, 115, 46, 120, 109, 108, 60, 47, 70,
        105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 66, 117, 105, 108, 100,
        97, 98, 108, 101, 115, 72, 117, 116, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 10, 10,
        9, 60, 33, 45, 45, 32, 69, 120, 112, 97, 110, 115, 105, 111, 110, 32, 71, 101, 110, 101, 114, 97, 108, 32,
        70, 105, 108, 101, 115, 32, 45, 45, 62, 10, 9, 10, 9, 60, 70, 105, 108, 101, 62, 83, 112, 97, 99, 101, 80,
        114, 111, 112, 115, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 46, 120, 109, 108, 60, 47, 70, 105,
        108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 84,
        114, 97, 110, 115, 112, 111, 114, 116, 85, 110, 105, 116, 115, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101,
        62, 10, 9, 60, 70, 105, 108, 101, 62, 77, 105, 110, 111, 114, 95, 72, 101, 114, 111, 101, 115, 95, 69, 120,
        112, 97, 110, 115, 105, 111, 110, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108,
        101, 62, 80, 114, 111, 112, 115, 95, 70, 101, 108, 117, 99, 105, 97, 46, 120, 109, 108, 60, 47, 70, 105,
        108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 66, 117, 105, 108, 100, 97, 98,
        108, 101, 115, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 46, 120, 109, 108, 60, 47, 70, 105, 108,
        101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 71, 114, 111, 117, 110, 100, 83, 116, 114, 117, 99, 116, 117,
        114, 101, 115, 95, 85, 110, 100, 101, 114, 119, 111, 114, 108, 100, 46, 120, 109, 108, 60, 47, 70, 105, 108,
        101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 77, 111, 98, 105, 108, 101, 95, 68, 101, 102, 101, 110, 115, 101,
        95, 85, 110, 105, 116, 115, 46, 88, 77, 76, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62,
        77, 117, 108, 116, 105, 112, 108, 97, 121, 101, 114, 95, 83, 116, 114, 117, 99, 116, 117, 114, 101, 95, 77,
        97, 114, 107, 101, 114, 115, 46, 88, 77, 76, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101,
        62, 85, 112, 103, 114, 97, 100, 101, 79, 98, 106, 101, 99, 116, 115, 95, 85, 110, 100, 101, 114, 119, 111,
        114, 108, 100, 46, 88, 77, 76, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 67, 111,
        114, 114, 117, 112, 116, 105, 111, 110, 95, 83, 112, 101, 99, 105, 97, 108, 95, 85, 110, 105, 116, 115, 46,
        88, 77, 76, 60, 47, 70, 105, 108, 101, 62, 10, 10, 10, 10, 60, 47, 71, 97, 109, 101, 95, 79, 98, 106, 101,
        99, 116, 95, 70, 105, 108, 101, 115, 62,
        60, 63, 120, 109, 108, 32, 118, 101, 114, 115, 105, 111, 110, 61, 39, 49, 46, 48, 39, 32, 101, 110, 99, 111,
        100, 105, 110, 103, 61, 39, 117, 116, 102, 45, 56, 39, 63, 62, 10, 60, 67, 97, 109, 112, 97, 105, 103, 110,
        95, 70, 105, 108, 101, 115, 62, 10, 9, 60, 70, 105, 108, 101, 62, 99, 111, 114, 101, 47, 103, 99, 47, 99,
        97, 109, 112, 97, 105, 103, 110, 47, 99, 97, 109, 112, 97, 105, 103, 110, 115, 95, 117, 110, 100, 101, 114,
        119, 111, 114, 108, 100, 95, 103, 99, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105,
        108, 101, 62, 99, 111, 114, 101, 47, 103, 99, 47, 99, 97, 109, 112, 97, 105, 103, 110, 47, 99, 97, 109, 112,
        97, 105, 103, 110, 115, 95, 117, 110, 100, 101, 114, 119, 111, 114, 108, 100, 95, 115, 116, 111, 114, 121,
        46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 99, 111, 114, 101, 47,
        103, 99, 47, 99, 97, 109, 112, 97, 105, 103, 110, 47, 99, 97, 109, 112, 97, 105, 103, 110, 115, 95, 109,
        117, 108, 116, 105, 112, 108, 97, 121, 101, 114, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62, 10, 9,
        60, 70, 105, 108, 101, 62, 99, 111, 114, 101, 47, 103, 99, 47, 99, 97, 109, 112, 97, 105, 103, 110, 47, 99,
        97, 109, 112, 97, 105, 103, 110, 115, 95, 116, 117, 116, 111, 114, 105, 97, 108, 46, 120, 109, 108, 60, 47,
        70, 105, 108, 101, 62, 10, 9, 60, 70, 105, 108, 101, 62, 99, 111, 114, 101, 47, 103, 99, 47, 99, 97, 109,
        112, 97, 105, 103, 110, 47, 99, 97, 109, 112, 97, 105, 103, 110, 115, 95, 117, 110, 100, 101, 114, 119, 111,
        114, 108, 100, 95, 116, 117, 116, 111, 114, 105, 97, 108, 46, 120, 109, 108, 60, 47, 70, 105, 108, 101, 62,
        10, 60, 47, 67, 97, 109, 112, 97, 105, 103, 110, 95, 70, 105, 108, 101, 115, 62,
    };

    internal static readonly byte[] CONTENT_MEG_FILE_HEADER_V1 =
    {
        2, 0, 0, 0, 2, 0, 0, 0, 28, 0, 68, 65, 84, 65, 47, 88, 77, 76, 47, 71, 65, 77, 69, 79, 66, 74, 69, 67, 84,
        70, 73, 76, 69, 83, 46, 88, 77, 76, 26, 0, 68, 65, 84, 65, 47, 88, 77, 76, 47, 67, 65, 77, 80, 65, 73, 71,
        78, 70, 73, 76, 69, 83, 46, 88, 77, 76, 146, 206, 127, 126, 0, 0, 0, 0, 158, 20, 0, 0, 106, 0, 0, 0, 0, 0,
        0, 0, 252, 90, 98, 183, 1, 0, 0, 0, 114, 1, 0, 0, 8, 21, 0, 0, 1, 0, 0, 0
    };
}