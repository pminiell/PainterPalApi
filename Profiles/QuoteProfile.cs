using AutoMapper;
using PainterPalApi.DTOs;
using PainterPalApi.Models;

namespace PainterPalApi.Profiles
{
    public class QuoteProfile : Profile
    {
        public QuoteProfile()
        {
            CreateMap<Quote, QuoteDTO>();
            CreateMap<QuoteDTO, Quote>();
            CreateMap<Colour, ColourDTO>();
            CreateMap<ColourDTO, Colour>();
            CreateMap<Customer, CustomerDTO>();
            CreateMap<CustomerDTO, Customer>();
            CreateMap<Job, JobDTO>();
            CreateMap<JobDTO, Job>();
            CreateMap<JobTask, JobTaskDTO>();
            CreateMap<JobTaskDTO, JobTask>();
        }
    }
}
