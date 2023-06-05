using Application.ViewModels;
using AutoMapper;
using Domain.LookupModels;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<AppUser, AddUserViewModel>().ReverseMap();
            CreateMap<CareHome, CareHomeViewModel>().ReverseMap();
            CreateMap<Qualification, QualificationViewModel>().ReverseMap();
            CreateMap<Staff, StaffViewModel>().ReverseMap();
            //CreateMap<StaffQualification, StaffQualificationViewModel>().ReverseMap();
        }
    }
}
