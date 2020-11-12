using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DataModels
{
    public class PDFFile : IFile
    {
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}