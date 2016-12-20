using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFS210.IO;

namespace WindowsFormsApplication1
{
    class FakeStream : Stream
    {
        private Stream stream;

        private Checksum checksum;

        public FakeStream(Stream stream,Checksum checksum)
        {
            this.stream = stream;
            this.checksum = checksum;
        }
        public override bool CanRead
        {
            get
            {
                return stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return stream.Position;
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer,offset,count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset,origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            checksum.Update(buffer, offset, count);
            stream.Write(buffer, offset, count);
        }
    }
}
