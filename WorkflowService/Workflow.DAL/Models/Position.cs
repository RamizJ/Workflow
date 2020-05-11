using System;
using System.Collections.Generic;
using System.Text;

namespace Workflow.DAL.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRemoved { get; set; }
    }
}
