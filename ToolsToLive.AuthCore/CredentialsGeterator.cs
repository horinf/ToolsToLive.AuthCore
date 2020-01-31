using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using ToolsToLive.AuthCore.Interfaces;

namespace ToolsToLive.AuthCore
{
    public class CredentialsGeterator : ICredentialsGeterator
    {
        public SigningCredentials GetSigningCredentials()
        {
            throw new NotImplementedException();
        }
    }
}
