namespace Workflow.VM.ViewModels
{
    public class VmAuthOutput
    {
        public VmUser User { get; set; }

        public string Token { get; set; }

        public VmAuthOutput()
        { }

        public VmAuthOutput(VmUser user, string token)
        {
            User = user;
            Token = token;
        }
    }
}