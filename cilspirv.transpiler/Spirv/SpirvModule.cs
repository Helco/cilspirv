using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace cilspirv.Spirv
{
    public sealed record SpirvModule
    {
        public const uint BoundNotSet = uint.MaxValue;
        public const uint DefaultMagic = 0x07230203;
        public const uint SwappedMagic = 0x03022307;

        public uint Magic { get; init; } = DefaultMagic;
        public Version SpirvVersion { get; init; } = new Version(1, 4);
        public ushort GeneratorToolID { get; init; } = 42; // TODO: Register actual tool ID at Khronos
        public Version GeneratorVersion { get; init; } = typeof(SpirvModule).Assembly.GetName().Version ?? new Version(0, 1);
        public uint Bound { get; init; } = BoundNotSet;
        public IReadOnlyList<Instruction> Instructions { get; init; } = Array.Empty<Instruction>();

        public int SizeInWords => 5 + Instructions.Sum(i => i.WordCount);
        public int SizeInBytes => SizeInWords * sizeof(uint);

        public SpirvModule() { }

        public SpirvModule(Stream stream, bool leaveOpen = false)
        {
            using var reader = new BinaryReader(stream, System.Text.Encoding.Default, leaveOpen);
            Magic = reader.ReadUInt32();

            var readWord = Magic switch
            {
                DefaultMagic => (Func<uint>)reader.ReadUInt32,
                SwappedMagic => () => Swap32(reader.ReadUInt32()),
                _ => throw new InvalidDataException($"Invalid magic number {Magic.ToString("X8")}")
            };
            SpirvVersion = DecodeVersion(readWord() >> 8);
            uint generatorMagic = readWord();
            GeneratorToolID = (ushort)(generatorMagic >> 16);
            GeneratorVersion = DecodeVersion(generatorMagic);
            Bound = readWord();
            if (readWord() != 0)
                throw new NotSupportedException("Unsupported reserved number");

            var uintBuffer = new uint[16];
            var byteBuffer = MemoryMarshal.AsBytes(uintBuffer.AsSpan());
            var instructions = new List<Instruction>();
            while (true)
            {
                var read = stream.Read(byteBuffer.Slice(0, sizeof(uint)));
                if (read == 0)
                    break;
                else if (read != sizeof(uint))
                    throw new EndOfStreamException("Stream ended during instruction");
                if (Magic == SwappedMagic)
                    uintBuffer[0] = Swap32(uintBuffer[0]);

                int wordCount = (int)(uintBuffer[0] >> 16);
                if (uintBuffer.Length < wordCount)
                {
                    Array.Resize(ref uintBuffer, wordCount);
                    byteBuffer = MemoryMarshal.AsBytes(uintBuffer.AsSpan());
                }
                var expectedRead = sizeof(uint) * (wordCount - 1);
                read = stream.Read(byteBuffer.Slice(sizeof(uint), expectedRead));
                if (read != expectedRead)
                    throw new EndOfStreamException("Stream ended during operands");

                if (Magic == SwappedMagic)
                    SwapAll(uintBuffer.AsSpan(1));
                instructions.Add(Instruction.Read(uintBuffer));
            }
            Instructions = instructions;
        }

        public void Write(Stream stream, bool leaveOpen = false, Func<ID, uint>? mapID = null)
        {
            using var writer = new BinaryWriter(stream, System.Text.Encoding.Default, leaveOpen);
            writer.Write(Magic);

            var writeWord = Magic switch
            {
                DefaultMagic => (Action<uint>)writer.Write,
                SwappedMagic => word => writer.Write(Swap32(word)),
                _ => throw new InvalidDataException($"Invalid magic number {Magic.ToString("X8")}")
            };
            writeWord(EncodeVersion(SpirvVersion) << 8);
            writeWord((((uint)GeneratorToolID) << 16) | EncodeVersion(GeneratorVersion));
            writeWord(Bound == BoundNotSet ? CalculateBound() : Bound);
            writeWord(0);

            var uintBuffer = new uint[16];
            var byteBuffer = MemoryMarshal.AsBytes(uintBuffer.AsSpan());
            foreach (var instruction in Instructions)
            {
                if (uintBuffer.Length < instruction.WordCount)
                {
                    Array.Resize(ref uintBuffer, instruction.WordCount);
                    byteBuffer = MemoryMarshal.AsBytes(uintBuffer.AsSpan());
                }
                instruction.Write(uintBuffer.AsSpan(), mapID ?? (x => x.Value));
                writer.Write(byteBuffer.Slice(0, instruction.WordCount * sizeof(uint)));
            }
        }

        public uint CalculateBound() => Instructions.SelectMany(i => i.AllIDs).Max().Value;

        // from https://stackoverflow.com/questions/19560436/bitwise-endian-swap-for-various-types
        private static uint Swap32(uint x)
        {
            // swap adjacent 16-bit blocks
            x = (x >> 16) | (x << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        private static void SwapAll(Span<uint> words)
        {
            // Let's leave the unnecessary but certainly fun performance optimization for later
            foreach (ref var word in words)
                word = Swap32(word);
        }

        private static uint EncodeVersion(Version v) => (v.Major < 0 || v.Major > 255 || v.Minor < 0 || v.Minor > 255)
            ? throw new ArgumentOutOfRangeException(nameof(v))
            : (ushort)((v.Major << 8) | (v.Minor));

        private static Version DecodeVersion(uint v) => new Version((int)(v >> 8) & 0xff, (int)(v & 0xff));
    }
}
