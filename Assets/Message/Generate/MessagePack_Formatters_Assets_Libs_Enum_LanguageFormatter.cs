// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168
#pragma warning disable CS1591 // document public APIs

#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Assets.Libs.Enum
{

    public sealed class LanguageFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Assets.Libs.Enum.Language>
    {
        public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Assets.Libs.Enum.Language value, global::MessagePack.MessagePackSerializerOptions options)
        {
            writer.Write((global::System.Int32)value);
        }

        public global::Assets.Libs.Enum.Language Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            return (global::Assets.Libs.Enum.Language)reader.ReadInt32();
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
