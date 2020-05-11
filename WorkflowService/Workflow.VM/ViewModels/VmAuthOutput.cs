using System.IdentityModel.Tokens.Jwt;

namespace Workflow.VM.ViewModels
{
    public class VmAuthOutput
    {
        public VmUser User { get; set; }

        public JwtSecurityToken Token { get; set; }

        public VmAuthOutput()
        { }

        public VmAuthOutput(VmUser user, JwtSecurityToken token)
        {
            User = user;
            Token = token;
        }
    }
}