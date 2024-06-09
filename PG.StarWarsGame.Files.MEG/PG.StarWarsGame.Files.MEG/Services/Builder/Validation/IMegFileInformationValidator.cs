// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// A validator that check an instance of a <see cref="MegBuilderFileInformationValidationData"/>
/// </summary>
public interface IMegFileInformationValidator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="infoValidationData"></param>
    /// <returns></returns>
    MegFileInfoValidationResult Validate(MegBuilderFileInformationValidationData infoValidationData);
}

/// <summary>
/// 
/// </summary>
public struct MegFileInfoValidationResult
{
    internal static readonly MegFileInfoValidationResult Failed = default;

    internal static readonly MegFileInfoValidationResult Valid = new(true);

    /// <summary>
    /// 
    /// </summary>
    public string? FailReason { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsValid { get; }

    private MegFileInfoValidationResult(string? failReason)
    {
        IsValid = false;
        FailReason = failReason;
    }

    internal MegFileInfoValidationResult(bool isValid)
    {
        IsValid = isValid;
    }

    internal static MegFileInfoValidationResult FromFailed(string? message)
    {
        return new MegFileInfoValidationResult(message);
    }
}