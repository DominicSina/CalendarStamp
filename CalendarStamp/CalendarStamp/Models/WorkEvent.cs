using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarStamp.Models
{
    using Plugin.Calendars.Abstractions;

    public class WorkEvent : CalendarEvent
    {
        public WorkEvent() : base()
        {
            this.AllDay = false;
            this.Description = "Work event. Currently not finished";
            this.Name = "Work";
            this.Start = DateTime.Now;
            this.End = DateTime.Now;
        }

        public void end()
        {
            this.End = DateTime.Now;
            this.Description = "Finished work event.";
        }
    }
}
