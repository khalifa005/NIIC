using Application.Comments.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.Activities.Dtos
{
    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public DateTime Date { get; set; }

        [JsonProperty("Attendees")]
        //to tell automapper how to map it and in the
       //response property name will change
        public List<AttendeeDto> UserActivities { get; set; }
        public List<CommentDto> Comments { get; set; }

    }
}
