using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IFile
    {
        byte[] Content { get; set; }
        string ContentType { get; set; }

        string FileName { get; set; }
    }
}
