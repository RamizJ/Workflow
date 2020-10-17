namespace Workflow.DAL.Models
{
    public class Metadata
    {
        public int Id { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public int? GoalId { get; set; }
        public Goal Goal { get; set; }
    }
}