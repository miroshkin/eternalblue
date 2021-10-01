using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EternalBlue.Models;
using RecruitmentService;
using Candidate = EternalBlue.Data.Candidate;

namespace EternalBlue.Mapping
{
    public class IFSMappingProfile : Profile
    {
        public IFSMappingProfile()
        {
            CreateMap<Candidate, ProcessedCandidate>()
                .ForMember(c => c.Email, cfg => cfg.MapFrom(c => c.Email))
                .ForMember(c => c.Id, cfg => cfg.MapFrom(c => c.CandidateId))
                .ForMember(c => c.FullName, cfg => cfg.MapFrom(c => c.FullName))
                .ForMember(c => c.ProfilePicture, cfg => cfg.MapFrom(c => c.ProfilePicture))
                .ForMember(c =>c.ProcessedCandidateSkills, cfg => cfg.MapFrom(c => c.Experience))
                .ReverseMap();

            CreateMap<Data.Skill, ProcessedCandidateSkill>()
                .ForMember(c => c.Skill, cfg => cfg.MapFrom(c => c))
                .ReverseMap();

            CreateMap<Models.Skill, Data.Skill>()
                .ForMember(c => c.YearsOfExperience, cfg => cfg.MapFrom(c => c.YearsOfExperience))
                .ForMember(c => c.TechnologyId, cfg => cfg.MapFrom(c => c.TechnologyId))
                .ForMember(c => c.TechnologyName, cfg => cfg.MapFrom(c => c.TechnologyName))
                .ReverseMap();

            CreateMap<RecruitmentService.Candidate, Candidate>()
                .ForMember(c => c.CandidateId, cfg => cfg.MapFrom(c => c.CandidateId))
                .ForMember(c => c.Email, cfg => cfg.MapFrom(c => c.Email));

            CreateMap<RecruitmentService.Skill, Data.Skill>()
                .ForMember(c => c.YearsOfExperience, cfg => cfg.MapFrom(c => c.YearsOfExperience))
                .ForMember(c => c.TechnologyId, cfg => cfg.MapFrom(c => c.TechnologyId))
                .ReverseMap();
        }
    }
}
