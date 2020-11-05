using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoInkasServiceAPI.Services
{
    public interface ISpamControlService
    {
        public int SpamDelay { get; }
    }
}
