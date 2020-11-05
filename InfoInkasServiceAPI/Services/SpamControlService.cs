using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfoInkasServiceAPI.Services
{
    public class SpamControlService : ISpamControlService
    {
        public int SpamDelay => _config.GetValue<int>("SpamDelay");

        private IConfiguration _config;
        public SpamControlService(IConfiguration config)
        {
            _config = config;
        }
    }
}