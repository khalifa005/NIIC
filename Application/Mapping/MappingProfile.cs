using Application.Activities.Dtos;
using Application.Comments.Dto;
using AutoMapper;
using Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {

        //automapper is convention based  id->id etc..
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDto>();
            CreateMap<UserActivity, AttendeeDto>()
                .ForMember(d=> d.DisplayName, o=> o.MapFrom(s=> s.AppUser.DisplayName))
                .ForMember(d=> d.Username, o=> o.MapFrom(s=> s.AppUser.UserName))
                .ForMember(d=> d.Image, o=> o.MapFrom(s=> s.AppUser.Photos.FirstOrDefault(x => x.IsMain == true).Url))
                .ForMember(d=> d.Following , o=> o.MapFrom<FollowingResolver>()); 
            //to resolver if the current loged user is following the owner of this activity we did seperate it with resolver so we can inject data context ..etc

            CreateMap<Comment, CommentDto>()
               .ForMember(d => d.Replies, o => o.MapFrom(s => s.Replies))
               .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName))
               .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
               .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain == true).Url));
        }
    }
}
