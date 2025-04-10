using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorReceivedTypingMsg : NetMessage
{
    public override MsgGroups MsgGroup => MsgGroups.Core;

    public Guid To;
    public string Author = string.Empty;

    public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
    {
        To = buffer.ReadGuid();
        Author = buffer.ReadString();
    }

    public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
    {
        buffer.Write(To);
        buffer.Write(Author);
    }
}
