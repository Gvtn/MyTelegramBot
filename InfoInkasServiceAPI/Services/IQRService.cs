using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoInkasServiceAPI.Services
{
    public interface IQRService
    {
        public byte[] GetQRBytes(string qrText);
    }
}
