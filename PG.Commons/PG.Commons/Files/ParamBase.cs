// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Reflection;
using System.Text;

namespace PG.Commons.Files;

/// <summary>
///     Base parameter implementation.
/// </summary>
public abstract record ParamBase : IParam
{
    ///<inheritdoc />
    public override string ToString()
    {
        var b = new StringBuilder();
        b.AppendLine($"{GetType()}::[");
        foreach (FieldInfo f in GetType().GetFields().Where(f => f.IsPublic))
        {
            b.AppendLine($"\t{f.Name}={f.GetValue(this)},");
        }

        b.AppendLine("];");
        return b.ToString();
    }
}