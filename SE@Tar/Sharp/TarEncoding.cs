// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SE.Storage.Tar
{
    /// <summary>
    /// Tar File Format Encoding 
    /// </summary>
    public class TarEncoding
    {
        public const int TarChunkSize = 512;
        public const string UstarBOM = "ustar";

        protected readonly DateTime InitialUnixTime = new DateTime(1970, 1, 1, 0, 0, 0);

        /// <summary>
        /// A context object providing information about a single
        /// Tar file header entry
        /// </summary>
        public class Entry
        {
            public string Name { get; internal set; }

            public int FileMode { get; internal set; }

            public int User { get; internal set; }
            public string UserName { get; internal set; }

            public int Group { get; internal set; }
            public string GroupName { get; internal set; }

            public long Size { get; internal set; }
            public DateTime Timestamp { get; internal set; }
            public TarEntryType Type { get; internal set; }

            public long Chunk { get; internal set; }

            public Entry()
            { }
        }

        /// <summary>
        /// Reads the header of the given data stream to detect the next file entry
        /// </summary>
        /// <param name="data">The input data to try processing header information from</param>
        /// <param name="buffer">A binary buffer used to read from the stream</param>
        /// <param name="contentBytes">The amount of bytes left for the file content</param>
        /// <param name="entry">An entry pointing to the data location</param>
        /// <returns>True if an entry was read from the stream, false otherwise</returns>
        public bool Decode(Stream data, byte[] buffer, ref long contentBytes, out Entry entry)
        {
            entry = new Entry();
            long checksum = 0;

            if (buffer.Length < 155)
            {
                throw new ArgumentOutOfRangeException("buffer");
            }
            else if (contentBytes > 0)
            {
                if (!data.CanSeek)
                {
                    if (!CalculateFileDataOffset(data, buffer, contentBytes))
                        return false;
                }
                else data.Seek(contentBytes, SeekOrigin.Current);
            }
            long headerChecksum = 0;
            do
            {
                contentBytes = TarChunkSize;

                //Field offset | Field size | Field 

                #region 0 	100 	File name 
                if (Read(data, buffer, 100, ref contentBytes, ref checksum))
                {
                    entry.Name = GetString(buffer, 100);
                }
                else return false;
                #endregion

                #region 100 	8 	File mode 
                if (Read(data, buffer, 8, ref contentBytes, ref checksum))
                {
                    entry.FileMode = GetInt32(buffer, 8);
                }
                else return false;
                #endregion

                #region 108 	8 	Owner's numeric user ID 
                if (Read(data, buffer, 8, ref contentBytes, ref checksum))
                {
                    entry.User = GetInt32(buffer, 8);
                }
                else return false;
                #endregion

                #region 116 	8 	Group's numeric user ID 
                if (Read(data, buffer, 8, ref contentBytes, ref checksum))
                {
                    entry.Group = GetInt32(buffer, 8);
                }
                else return false;
                #endregion

                #region 124 	12 	File size in bytes (octal base) 
                if (Read(data, buffer, 12, ref contentBytes, ref checksum))
                {
                    entry.Size = GetInt64(buffer, 12);
                }
                else return false;
                #endregion

                #region 136 	12 	Last modification time in numeric Unix time format (octal)  
                if (Read(data, buffer, 12, ref contentBytes, ref checksum))
                {
                    long unixTimeStamp = GetInt64(buffer, 12);
                    entry.Timestamp = InitialUnixTime.AddSeconds(unixTimeStamp).ToLocalTime();
                }
                else return false;
                #endregion

                #region 148 	8 	Checksum for header record
                if (Read(data, buffer, 8, ref contentBytes, ref headerChecksum))
                {
                    headerChecksum = GetInt32(buffer, 8);
                    /**
                     The checksum is calculated by taking the sum of the unsigned byte values of the header
                     record with the eight checksum bytes taken to be ascii spaces (decimal value 32)
                    */
                    checksum += (8 * 32);
                }
                else return false;
                #endregion

                #region 156 	1 	Link indicator (file type) 
                if (Read(data, buffer, 1, ref contentBytes, ref checksum))
                {
                    switch ((char)buffer[0])
                    {
                        case '\0':
                            {
                                entry.Type = TarEntryType.File;
                            }
                            break;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                            {
                                entry.Type = (TarEntryType)(buffer[0] - '0');
                            }
                            break;
                    }
                }
                else return false;
                #endregion

                #region 157 	100 	Name of linked file 
                if (!Read(data, buffer, 100, ref contentBytes, ref checksum))
                {
                    return false;
                }
                #endregion

                #region 257 	6 	UStar indicator "ustar" then NUL 
                bool isUstarHeader = false;
                if (Read(data, buffer, 6, ref contentBytes, ref checksum))
                {
                    isUstarHeader = (GetString(buffer, 6) == UstarBOM);
                }
                else return false;
                #endregion

                if (isUstarHeader)
                {
                    #region 263 	2 	UStar version "00" 
                    if (!Read(data, buffer, 2, ref contentBytes, ref checksum))
                    {
                        return false;
                    }
                    #endregion

                    #region 265 	32 	Owner user name 
                    if (Read(data, buffer, 32, ref contentBytes, ref checksum))
                    {
                        entry.UserName = GetString(buffer, 32);
                    }
                    else return false;
                    #endregion

                    #region 297 	32 	Owner group name 
                    if (Read(data, buffer, 32, ref contentBytes, ref checksum))
                    {
                        entry.GroupName = GetString(buffer, 32);
                    }
                    else return false;
                    #endregion

                    #region 329 	8 	Device major number 
                    if (!Read(data, buffer, 8, ref contentBytes, ref checksum))
                    {
                        return false;
                    }
                    #endregion

                    #region 337 	8 	Device minor number 
                    if (!Read(data, buffer, 8, ref contentBytes, ref checksum))
                    {
                        return false;
                    }
                    #endregion

                    #region 345 	155 	Filename prefix 
                    if (Read(data, buffer, 155, ref contentBytes, ref checksum))
                    {
                        entry.Name = string.Concat(GetString(buffer, 155), entry.Name);
                    }
                    else return false;
                    #endregion
                }

                #region ? 	? 	(remaining) 
                if (!CalculateFileDataOffset(data, buffer, contentBytes))
                {
                    throw new TarException();
                }
                #endregion

                contentBytes = entry.Size;
            }
            while (headerChecksum == 0 && entry.Size == 0);
            if (checksum == headerChecksum)
            {
                long offset = (contentBytes % TarChunkSize);
                if (offset > 0)
                {
                    offset = (TarChunkSize - offset);
                }
                contentBytes += offset;
                return true;
            }
            else throw new TarException();
        }

        protected bool Read(Stream data, byte[] buffer, int size, ref long remainingBytesInFile, ref long checksum)
        {
            if (data.Read(buffer, 0, size) == size)
            {
                for (int i = 0; i < size; i++)
                {
                    checksum += buffer[i];
                }
                remainingBytesInFile -= size;
                return true;
            }
            else return false;
        }
        protected string GetString(byte[] buffer, int size)
        {
            return Encoding.ASCII.GetString(buffer, 0, size).Trim('\0').Trim();
        }
        protected int GetInt32(byte[] buffer, int size)
        {
            string str = GetString(buffer, size);
            if (!string.IsNullOrWhiteSpace(str))
            {
                return Convert.ToInt32(str, 8);
            }
            else return default(int);
        }
        protected long GetInt64(byte[] buffer, int size)
        {
            string str = GetString(buffer, size);
            if (!string.IsNullOrWhiteSpace(str))
            {
                return Convert.ToInt32(str, 8);
            }
            else return default(int);
        }

        protected bool CalculateFileDataOffset(Stream data, byte[] buffer, long remainingBytesInFile)
        {
            while (remainingBytesInFile > 0)
            {
                int count = data.Read(buffer, 0, (int)Math.Min(remainingBytesInFile, buffer.Length));
                if (count == 0)
                {
                    return false;
                }
                else remainingBytesInFile -= count;
            }
            return true;
        }
    }
}