using System;
using System.IO;

namespace WinCopies.IO
{
    public static class StreamExtensions
    {
        public static void GetAvailableLength(this System.IO.Stream stream, ref int count) => UtilHelpers.GetAvailableLength(stream.Length, stream.Position, ref count);

        public static void ReadToBufferEnd(this System.IO.Stream stream, byte[] buffer, int offset, int count)
        {
            int length = 0;

            do

                length += stream.Read(buffer, offset + length, count - length);

            while (length < count);
        }

        public static byte[] ReadToBufferEnd(this System.IO.Stream stream, in int count)
        {
            var buffer = new byte[count];

            stream.ReadToBufferEnd(buffer, 0, count);

            return buffer;
        }

        public static System.IO.Stream Prepend(this System.IO.Stream stream, in long length, in int bufferLength = 4096)
        {
            System.IO.Stream _stream = new ReversedStream(new Substream(stream, 0u, (ulong)stream.Length));

            stream.SetLength(length);

            _ = stream.Seek(0, SeekOrigin.End);

            byte[] buffer = new byte[bufferLength];

            int count;

            while ((count = stream.Read(buffer, 0, bufferLength)) > 0)

                _stream.Write(buffer, 0, count);

            return new Substream(stream, 0u, (ulong)length);
        }
    }

    public class ReversedStream : System.IO.Stream
    {
        protected System.IO.Stream Stream { get; }

        public override bool CanRead => Stream.CanRead;

        public override bool CanSeek => Stream.CanSeek;

        public override bool CanWrite => Stream.CanWrite;

        public override long Length => Stream.Length;

        public override long Position { get => Length - Stream.Position; set => Stream.Position = Length - value; }

        public ReversedStream(in System.IO.Stream stream) => Stream = stream ?? throw new ArgumentNullException(nameof(stream));

        public override void Flush() => Stream.Flush();

        protected void DoWork(in byte[] buffer, in int offset, int count, in Action<byte[], int, int> action)
        {
            void updatePosition() => Position = Stream.Position - count;

            updatePosition();

            action(buffer, offset, count);

            updatePosition();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.GetAvailableLength(ref count);

            DoWork(buffer, offset, count, Stream.ReadToBufferEnd);

            return count;
        }

        public override long Seek(long offset, SeekOrigin origin) => Length - Stream.Seek(Length - offset, origin);
        public override void SetLength(long value) => Stream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => DoWork(buffer, offset, count, Stream.Write);
    }

    public class Substream : System.IO.Stream
    {
        protected System.IO.Stream Stream { get; }

        protected long Start { get; }

        public override bool CanRead => Stream.CanRead;

        public override bool CanSeek => Stream.CanSeek;

        public override bool CanWrite => Stream.CanWrite;

        public override long Length { get; }

        public override long Position { get => Stream.Position - Start; set => Stream.Position = value + Start; }

        public Substream(in System.IO.Stream stream, in ulong start, in ulong length)
        {
            ulong _length = (ulong)(stream ?? throw new ArgumentNullException(nameof(stream))).Length;

            Stream = start <= _length ? length.Between(start, _length - start) ? stream : throw new ArgumentOutOfRangeException(nameof(length)) : throw new ArgumentOutOfRangeException(nameof(start));

            Start = (long)start;

            Length = (long)length;
        }

        public Substream(in System.IO.Stream stream, in ulong start) : this(stream, start, (ulong)stream.Length - start) { /* Left empty. */ }

        protected bool Check(in long offset) => offset <= Length - Position;
        private void DoWork(in long offset, in string paramName, in Action action)
        {
            if (Check(offset))

                action();

            else throw new ArgumentOutOfRangeException(paramName);
        }

        public override void Flush() => Stream.Flush();

        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = 0;

            DoWork(count, nameof(count), () => result = Stream.Read(buffer, offset, count));

            return result;
        }

        public override long Seek(long offset, SeekOrigin origin) => (origin == SeekOrigin.Current ? Check(offset) : offset.Between(0, Length, true, false)) ? Stream.Seek(offset + Start, origin) : throw new ArgumentOutOfRangeException(nameof(offset));
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => DoWork(count, nameof(count), () => Stream.Write(buffer, offset, count));
    }
}
