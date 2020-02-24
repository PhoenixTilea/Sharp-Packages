// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;

namespace SE.Storage.Zip
{
    /// <summary>
    /// Zip File Format Encoding 
    /// </summary>
    public class ZipEncoding
    {
        /// <summary>
        /// A context object providing information about a single
        /// Zip file header entry
        /// </summary>
        public class Entry
        {
            public string Name { get; internal set; }
            public string Comment { get; internal set; }

            public UInt32 Crc32 { get; internal set; }

            public int CompressedSize { get; internal set; }
            public int OriginalSize { get; internal set; }

            public bool Deflated { get; internal set; }

            public bool IsDirectory { get { return Name.EndsWith("/"); } }
            public bool IsFile { get { return !IsDirectory; } }

            public DateTime Timestamp { get; internal set; }

            public int HeaderOffset { get; internal set; }
            public int DataOffset { get; internal set; }

            public Entry()
            { }
        }

        /// <summary>
        /// Reads the header of the given data stream to detect all Zip header entries
        /// </summary>
        /// <param name="data">The input data to try processing header information from</param>
        /// <param name="reader">A binary reader instance for processing encoded data types</param>
        /// <returns>A list of found header entries</returns>
        public virtual List<Entry> Decode(Stream data, BinaryReader reader)
        {
            List<Entry> result = new List<Entry>();

            if (data.Length < 22) return result;
            data.Seek(-22, SeekOrigin.End);

            while (reader.ReadInt32() != ZipUtils.DirectorySignature)
            {
                if (data.Position <= 5)
                    return result;

                data.Seek(-5, SeekOrigin.Current);
            }

            data.Seek(6, SeekOrigin.Current);
            UInt16 entries = reader.ReadUInt16();
            Int32 difSize = reader.ReadInt32();
            UInt32 dirOffset = reader.ReadUInt32();
            data.Seek(dirOffset, SeekOrigin.Begin);

            for (int i = 0; i < entries; i++)
            {
                if (reader.ReadInt32() != ZipUtils.EntrySignature)
                    continue;

                Entry entry = new Entry();

                reader.ReadInt32();
                System.Text.Encoding encoding = (((reader.ReadInt16() & ZipUtils.Utf8Flag) != 0) ? System.Text.Encoding.UTF8 : System.Text.Encoding.Default);

                entry.Deflated = (reader.ReadInt16() == 8);
                entry.Timestamp = ZipUtils.ToDateTime(reader.ReadInt32());
                entry.Crc32 = reader.ReadUInt32();
                entry.CompressedSize = reader.ReadInt32();
                entry.OriginalSize = reader.ReadInt32();

                short fileNameSize = reader.ReadInt16();
                short extraSize = reader.ReadInt16();
                short commentSize = reader.ReadInt16();
                
                reader.ReadInt32();
                reader.ReadInt32();

                entry.HeaderOffset = reader.ReadInt32();
                entry.Name = encoding.GetString(reader.ReadBytes(fileNameSize));
                data.Seek(extraSize, SeekOrigin.Current);

                entry.Comment = encoding.GetString(reader.ReadBytes(commentSize));
                entry.DataOffset = CalculateFileDataOffset(data, reader, entry.HeaderOffset);
                result.Add(entry);
            }

            return result;
        }
        /// <summary>
        /// Decodes a single entry into a stream object
        /// </summary>
        /// <param name="data">The input data to try processing information from</param>
        /// <param name="reader">A binary reader instance for processing encoded data types</param>
        /// <param name="entry">An entry pointing to the data location</param>
        /// <param name="node">The output stream data is written to</param>
        public virtual void Decode(Stream data, BinaryReader reader, Entry entry, Stream node)
        {
            data.Seek(entry.HeaderOffset, SeekOrigin.Begin);
            if (reader.ReadInt32() != ZipUtils.FileSignature)
                throw new InvalidDataException();

            data.Seek(entry.DataOffset, SeekOrigin.Begin);
            Stream inputStream = data;

            if (entry.Deflated)
                inputStream = new System.IO.Compression.DeflateStream(data, System.IO.Compression.CompressionMode.Decompress, true);

            int count = entry.OriginalSize;
            int bufferSize = Math.Min(ZipUtils.BufferSize, entry.OriginalSize);

            byte[] buffer = new byte[bufferSize];
            UInt32 crcValue = ZipUtils.Crc32Identity;

            while (count > 0)
            {
                int read = inputStream.Read(buffer, 0, bufferSize);
                if (read == 0)
                    break;

                crcValue = ZipUtils.SetCrc32(buffer, read, crcValue);

                node.Write(buffer, 0, read);
                count -= read;
            }

            if (ZipUtils.GetCrc32(crcValue) != entry.Crc32)
                throw new InvalidDataException();
        }

        protected int CalculateFileDataOffset(Stream data, BinaryReader reader, int fileHeaderOffset)
        {
            long position = data.Position;
            data.Seek(fileHeaderOffset + 26, SeekOrigin.Begin);
            var fileNameSize = reader.ReadInt16();
            var extraSize = reader.ReadInt16();

            int fileOffset = (int)data.Position + fileNameSize + extraSize;
            data.Seek(position, SeekOrigin.Begin);
            return fileOffset;
        }
    }
}
