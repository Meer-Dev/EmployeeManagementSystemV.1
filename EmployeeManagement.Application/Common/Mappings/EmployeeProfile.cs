using AutoMapper;


namespace EmployeeManagement.Application.Common.Mappings;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>();
    }
}
