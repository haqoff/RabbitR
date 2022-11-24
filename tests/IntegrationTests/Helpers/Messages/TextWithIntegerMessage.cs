using RabbitR.Messages;

namespace IntegrationTests.Helpers.Messages;

internal class TextWithIntegerMessage : IEventBusMessage
{
    public TextWithIntegerMessage(string text, int index)
    {
        Text = text;
        Index = index;
    }

    public string Text { get; init; }
    public int Index { get; init; }

    protected bool Equals(TextWithIntegerMessage other)
    {
        return Text == other.Text && Index == other.Index;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((TextWithIntegerMessage)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Text, Index);
    }
}