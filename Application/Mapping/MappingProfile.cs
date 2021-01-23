﻿using Application.Activities.Dtos;
using AutoMapper;
using Domains;
using System;
using System.Collections.Generic;
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
                .ForMember(d=> d.Username, o=> o.MapFrom(s=> s.AppUser.UserName));
        }
    }
}