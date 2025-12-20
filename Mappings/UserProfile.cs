using TankR.Data.Dtos.Address;
using TankR.Data.Models;
using AutoMapper;

namespace TankR.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        //User
        CreateMap<User, UserDto>()    
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();

        //Address
        CreateMap<Address, AddressDto>();
        CreateMap<CreateAddressDto, Address>();
        CreateMap<UpdateAddressDto, Address>();
    }
}