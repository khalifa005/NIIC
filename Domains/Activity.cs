using System;
using System.Collections.Generic;
using System.Text;

namespace Domains
{
    public class Activity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }
    }
}
