using System;
using System.Text;
using PG.Commons.Utilities;
using Xunit;

namespace PG.Commons.Test.Utilities;

// Adapted from
// https://github.com/dotnet/runtime/
// &
// https://github.com/linkdotnet/StringBuilder

public class ValueStringBuilderTests
{
    [Fact]
    public void Ctor_Default_CanAppend()
    {
        var vsb = default(ValueStringBuilder);
        Assert.Equal(0, vsb.Length);

        vsb.Append('a');
        Assert.Equal(1, vsb.Length);
        Assert.Equal("a", vsb.ToString());
    }

    [Fact]
    public void Ctor_Span_CanAppend()
    {
        var vsb = new ValueStringBuilder(new char[1]);
        Assert.Equal(0, vsb.Length);

        vsb.Append('a');
        Assert.Equal(1, vsb.Length);
        Assert.Equal("a", vsb.ToString());
    }

    [Fact]
    public void Ctor_InitialCapacity_CanAppend()
    {
        var vsb = new ValueStringBuilder(1);
        Assert.Equal(0, vsb.Length);

        vsb.Append('a');
        Assert.Equal(1, vsb.Length);
        Assert.Equal("a", vsb.ToString());
    }

    [Fact]
    public void Append_Char_MatchesStringBuilder()
    {
        var sb = new StringBuilder();
        var vsb = new ValueStringBuilder();
        for (var i = 1; i <= 100; i++)
        {
            sb.Append((char)i);
            vsb.Append((char)i);
        }

        Assert.Equal(sb.Length, vsb.Length);
        Assert.Equal(sb.ToString(), vsb.ToString());
    }

    [Fact]
    public void Append_String_MatchesStringBuilder()
    {
        var sb = new StringBuilder();
        var vsb = new ValueStringBuilder();
        for (var i = 1; i <= 100; i++)
        {
            var s = i.ToString();
            sb.Append(s);
            sb.Append(s);
            vsb.Append(s);
            vsb.Append(s.AsSpan());
        }

        Assert.Equal(sb.Length, vsb.Length);
        Assert.Equal(sb.ToString(), vsb.ToString());
    }

    [Theory]
    [InlineData(0, 4 * 1024 * 1024)]
    [InlineData(1025, 4 * 1024 * 1024)]
    [InlineData(3 * 1024 * 1024, 6 * 1024 * 1024)]
    public void Append_String_Large_MatchesStringBuilder(int initialLength, int stringLength)
    {
        var sb = new StringBuilder(initialLength);
        var vsb = new ValueStringBuilder(new char[initialLength]);

        var s = new string('a', stringLength);
        sb.Append(s);
        vsb.Append(s);

        Assert.Equal(sb.Length, vsb.Length);
        Assert.Equal(sb.ToString(), vsb.ToString());
    }

    [Fact]
    public void AppendSpan_DataAppendedCorrectly()
    {
        var sb = new StringBuilder();
        var vsb = new ValueStringBuilder();

        for (var i = 1; i <= 1000; i++)
        {
            var s = i.ToString();

            sb.Append(s);

            var span = vsb.AppendSpan(s.Length);
            Assert.Equal(sb.Length, vsb.Length);

            s.AsSpan().CopyTo(span);
        }

        Assert.Equal(sb.Length, vsb.Length);
        Assert.Equal(sb.ToString(), vsb.ToString());
    }

    [Fact]
    public void AsSpan_ReturnsCorrectValue_DoesntClearBuilder()
    {
        var sb = new StringBuilder();
        var vsb = new ValueStringBuilder();

        for (var i = 1; i <= 100; i++)
        {
            var s = i.ToString();
            sb.Append(s);
            vsb.Append(s);
        }

        var resultString = vsb.AsSpan().ToString();
        Assert.Equal(sb.ToString(), resultString);

        Assert.NotEqual(0, sb.Length);
        Assert.Equal(sb.Length, vsb.Length);
        Assert.Equal(sb.ToString(), vsb.ToString());
    }

    [Fact]
    public void ToString_ClearsBuilder_ThenReusable()
    {
        const string Text1 = "test";
        var vsb = new ValueStringBuilder();

        vsb.Append(Text1);
        Assert.Equal(Text1.Length, vsb.Length);

        var s = vsb.ToString();
        Assert.Equal(Text1, s);

        Assert.Equal(0, vsb.Length);
        Assert.Equal(string.Empty, vsb.ToString());
        Assert.True(vsb.TryCopyTo(Span<char>.Empty, out _));

        const string Text2 = "another test";
        vsb.Append(Text2);
        Assert.Equal(Text2.Length, vsb.Length);
        Assert.Equal(Text2, vsb.ToString());
    }

    [Fact]
    public void TryCopyTo_FailsWhenDestinationIsTooSmall_SucceedsWhenItsLargeEnough()
    {
        var vsb = new ValueStringBuilder();

        const string Text = "expected text";
        vsb.Append(Text);
        Assert.Equal(Text.Length, vsb.Length);

        Span<char> dst = new char[Text.Length - 1];
        Assert.False(vsb.TryCopyTo(dst, out var charsWritten));
        Assert.Equal(0, charsWritten);
        Assert.Equal(0, vsb.Length);
    }

    [Fact]
    public void TryCopyTo_ClearsBuilder_ThenReusable()
    {
        const string Text1 = "test";
        var vsb = new ValueStringBuilder();

        vsb.Append(Text1);
        Assert.Equal(Text1.Length, vsb.Length);

        Span<char> dst = new char[Text1.Length];
        Assert.True(vsb.TryCopyTo(dst, out var charsWritten));
        Assert.Equal(Text1.Length, charsWritten);
        Assert.Equal(Text1, dst.ToString());

        Assert.Equal(0, vsb.Length);
        Assert.Equal(string.Empty, vsb.ToString());
        Assert.True(vsb.TryCopyTo(Span<char>.Empty, out _));

        const string Text2 = "another test";
        vsb.Append(Text2);
        Assert.Equal(Text2.Length, vsb.Length);
        Assert.Equal(Text2, vsb.ToString());
    }

    [Fact]
    public void Dispose_ClearsBuilder_ThenReusable()
    {
        const string Text1 = "test";
        var vsb = new ValueStringBuilder();

        vsb.Append(Text1);
        Assert.Equal(Text1.Length, vsb.Length);

        vsb.Dispose();

        Assert.Equal(0, vsb.Length);
        Assert.Equal(string.Empty, vsb.ToString());
        Assert.True(vsb.TryCopyTo(Span<char>.Empty, out _));

        const string Text2 = "another test";
        vsb.Append(Text2);
        Assert.Equal(Text2.Length, vsb.Length);
        Assert.Equal(Text2, vsb.ToString());
    }

    [Fact]
    public void ShouldRemoveRange()
    {
        var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello World");

        stringBuilder.Remove(0, 6);

        Assert.Equal(5, stringBuilder.Length);
        Assert.Equal("World", stringBuilder.AsSpan().ToString());

        stringBuilder.Remove(0, 5);
        Assert.Equal("", stringBuilder.ToString());
    }

    [Theory]
    [InlineData(-1, 2)]
    [InlineData(1, -2)]
    [InlineData(90, 1)]
    public void ShouldThrowExceptionWhenOutOfRangeIndex(int startIndex, int length)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            using var stringBuilder = new ValueStringBuilder();
            stringBuilder.Append("Hello");
            stringBuilder.Remove(startIndex, length);
        });
    }

    [Fact]
    public void Remove_ShouldNotEntriesWhenLengthIsEqualToZero()
    {
        using var stringBuilder = new ValueStringBuilder();
        stringBuilder.Append("Hello");
        stringBuilder.Remove(0, 0);
        Assert.Equal("Hello", stringBuilder.ToString());
    }

    [Fact]
    public void ShouldInsertString()
    {
        var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Append("Hello World");
        valueStringBuilder.Insert(6, "dear ");
        Assert.Equal("Hello dear World", valueStringBuilder.ToString());
    }

    [Fact]
    public void ShouldInsertWhenEmpty()
    {
        var valueStringBuilder = new ValueStringBuilder();
        valueStringBuilder.Insert(0, "Hello");
        Assert.Equal("Hello", valueStringBuilder.ToString());
    }

    [Fact]
    public void Insert_ThrowsWhenIndexIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            using var builder = new ValueStringBuilder();
            builder.Insert(-1, "Hello");
        });
    }

    [Fact]
    public void Insert_ThrowsWhenIndexIsBehindBufferLength()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            using var builder = new ValueStringBuilder();
            builder.Insert(1, "Hello");
        });
    }


    [Fact]
    public void Indexer()
    {
        const string Text1 = "foobar";
        var vsb = new ValueStringBuilder();

        vsb.Append(Text1);

        Assert.Equal('b', vsb[3]);
        vsb[3] = 'c';
        Assert.Equal('c', vsb[3]);
        vsb.Dispose();
    }

    [Fact]
    public void EnsureCapacity_IfRequestedCapacityWins()
    {
        // Note: constants used here may be dependent on minimal buffer size
        // the ArrayPool is able to return.
        var builder = new ValueStringBuilder(stackalloc char[32]);

        builder.EnsureCapacity(65);

        Assert.Equal(128, builder.Capacity);
    }

    [Fact]
    public void EnsureCapacity_IfBufferTimesTwoWins()
    {
        var builder = new ValueStringBuilder(stackalloc char[32]);

        builder.EnsureCapacity(33);

        Assert.Equal(64, builder.Capacity);
        builder.Dispose();
    }

    [Fact]
    public void EnsureCapacity_NoAllocIfNotNeeded()
    {
        // Note: constants used here may be dependent on minimal buffer size
        // the ArrayPool is able to return.
        var builder = new ValueStringBuilder(stackalloc char[64]);

        builder.EnsureCapacity(16);

        Assert.Equal(64, builder.Capacity);
        builder.Dispose();
    }

    [Fact]
    public void AsSpan()
    {
        var builder = new ValueStringBuilder(stackalloc char[10]);
        builder.Append("0123456789");
        Assert.Equal("0123456789", builder.AsSpan().ToString());
        Assert.Equal("0123456789", builder.AsSpan(true).ToString());
        Assert.Equal("0123", builder.AsSpan(0, 4).ToString());
        Assert.Equal("789", builder.AsSpan(7).ToString());

        builder.Dispose();
    }
}