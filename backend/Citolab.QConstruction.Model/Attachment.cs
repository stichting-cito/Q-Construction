using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Citolab.Repository.Helpers;

namespace Citolab.QConstruction.Model
{
    public class Attachment : Citolab.Repository.Model
    {
        /// <summary>
        ///     Bytes of the attachment.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        ///     Bytes of the thumbnail of the attachment,
        ///     in case it's an image or video.
        /// </summary>
        public byte[] ThumbnailBytes { get; set; }

        /// <summary>
        ///     Hash of the contents.
        /// </summary>
        [EnsureIndex]
        public byte[] Hash { get; set; }

        /// <summary>
        ///     MIME-type of the contents.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        ///     File name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     List of item ids this attachment is used in.
        /// </summary>
        public List<Guid> UsedInItemIds { get; set; }

        public byte[] ComputeHash()
        {
            HashAlgorithm hasher = MD5.Create();
            Hash = hasher.ComputeHash(Bytes);
            return Hash;
        }
    }
}