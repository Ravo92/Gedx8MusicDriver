using System.Runtime.InteropServices;

namespace Gedx8MusicDriver.Models
{
    internal readonly record struct Gedx8ThinType2NamedSignature(uint Dword0, uint Dword1, uint Dword2, uint Dword3)
    {
        internal static Gedx8ThinType2NamedSignature FromPointer(IntPtr signaturePointer)
        {
            if (signaturePointer == IntPtr.Zero)
            {
                return default;
            }

            return new Gedx8ThinType2NamedSignature(
                unchecked((uint)Marshal.ReadInt32(signaturePointer, 0x00)),
                unchecked((uint)Marshal.ReadInt32(signaturePointer, 0x04)),
                unchecked((uint)Marshal.ReadInt32(signaturePointer, 0x08)),
                unchecked((uint)Marshal.ReadInt32(signaturePointer, 0x0C)));
        }

        internal bool IsZero => Dword0 == 0 && Dword1 == 0 && Dword2 == 0 && Dword3 == 0;
    }

    internal readonly record struct Gedx8ThinType2NamedRecordSnapshot(Gedx8ThinType2NamedSignature Signature, byte[] Payload, int Size)
    {
    }

    internal sealed class Gedx8ThinType2NamedRecordStore
    {
        private Gedx8ThinType2NamedRecord? _head;

        internal bool TryFind(Gedx8ThinType2NamedSignature signature, out Gedx8ThinType2NamedRecordSnapshot snapshot)
        {
            snapshot = default;
            if (signature.IsZero)
            {
                return false;
            }

            Gedx8ThinType2NamedRecord? current = _head;
            while (current != null)
            {
                if (current.Signature == signature)
                {
                    byte[] payloadCopy = new byte[current.Payload.Length];
                    Array.Copy(current.Payload, payloadCopy, payloadCopy.Length);
                    snapshot = new Gedx8ThinType2NamedRecordSnapshot(current.Signature, payloadCopy, current.Size);
                    return true;
                }

                current = current.Next;
            }

            return false;
        }

        internal Gedx8ThinType2NamedRecordSnapshot Upsert(Gedx8ThinType2NamedSignature signature, ReadOnlySpan<byte> payload, int size)
        {
            if (signature.IsZero)
            {
                throw new ArgumentException("Signature must not be zero.", nameof(signature));
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            int clampedSize = Math.Min(size, payload.Length);
            Gedx8ThinType2NamedRecord? current = _head;
            while (current != null)
            {
                if (current.Signature == signature)
                {
                    current.SetPayload(payload[..clampedSize], clampedSize);
                    return current.CreateSnapshot();
                }

                current = current.Next;
            }

            Gedx8ThinType2NamedRecord created = new Gedx8ThinType2NamedRecord(signature, payload[..clampedSize], clampedSize)
            {
                Next = _head,
            };

            _head = created;
            return created.CreateSnapshot();
        }

        internal bool TryCopyPayloadToPointer(Gedx8ThinType2NamedSignature signature, IntPtr destination, int requestedSize)
        {
            if (destination == IntPtr.Zero || requestedSize < 0)
            {
                return false;
            }

            if (!TryFind(signature, out Gedx8ThinType2NamedRecordSnapshot snapshot))
            {
                return false;
            }

            int copySize = Math.Min(requestedSize, snapshot.Size);
            if (copySize == 0)
            {
                return true;
            }

            Marshal.Copy(snapshot.Payload, 0, destination, copySize);
            return true;
        }

        internal void Clear()
        {
            _head = null;
        }

        private sealed class Gedx8ThinType2NamedRecord
        {
            internal Gedx8ThinType2NamedRecord(Gedx8ThinType2NamedSignature signature, ReadOnlySpan<byte> payload, int size)
            {
                Signature = signature;
                Payload = payload.ToArray();
                Size = size;
            }

            internal Gedx8ThinType2NamedSignature Signature { get; }

            internal byte[] Payload { get; private set; }

            internal int Size { get; private set; }

            internal Gedx8ThinType2NamedRecord? Next { get; set; }

            internal void SetPayload(ReadOnlySpan<byte> payload, int size)
            {
                Payload = payload.ToArray();
                Size = size;
            }

            internal Gedx8ThinType2NamedRecordSnapshot CreateSnapshot()
            {
                byte[] payloadCopy = new byte[Payload.Length];
                Array.Copy(Payload, payloadCopy, payloadCopy.Length);
                return new Gedx8ThinType2NamedRecordSnapshot(Signature, payloadCopy, Size);
            }
        }
    }
}
