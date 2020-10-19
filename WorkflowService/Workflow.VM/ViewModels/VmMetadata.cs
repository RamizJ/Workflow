namespace Workflow.VM.ViewModels
{
    public class VmMetadata
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public VmMetadata()
        { }

        public VmMetadata(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}