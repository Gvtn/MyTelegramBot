using InfoInkasService.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InfoInkasServiceAPI.Services
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(IEmailData data);
    }
}