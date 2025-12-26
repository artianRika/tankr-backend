using AutoMapper;
using TankR.Data.Dtos.FuelTypes;
using TankR.Data.Dtos.StationAddresses;
using TankR.Data.Dtos.StationEmployees;
using TankR.Data.Dtos.StationFuelPrices;
using TankR.Data.Dtos.StationPhotos;
using TankR.Data.Dtos.Stations;
using TankR.Data.Dtos.Transactions;
using TankR.Data.Dtos.UserAddresses;
using TankR.Data.Models;

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

        //UserAddress
        CreateMap<UserAddress, UserAddressDto>();
        CreateMap<CreateUserAddressDto, UserAddress>();
        CreateMap<UpdateUserAddressDto, UserAddress>();
        
        //Station
        CreateMap<Station, StationDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
        CreateMap<CreateStationDto, Station>();
        CreateMap<UpdateStationDto, Station>();
        
        //StationAddress
        CreateMap<StationAddress, StationAddressDto>();
        CreateMap<CreateStationAddressDto, StationAddress>();
        CreateMap<UpdateStationAddressDto, StationAddress>();
        
        
        //FuelType
        CreateMap<FuelType, FuelTypeDto>();
        CreateMap<CreateFuelTypeDto, FuelType>();
        CreateMap<UpdateFuelTypeDto, FuelType>();
        
        
        //StationFuelPrices
        CreateMap<StationFuelPrice, StationFuelPriceDto>();
        
        
        //StationEmployees
        CreateMap<StationEmployee, StationEmployeeDto>();
        CreateMap<AssignEmployeeDto, StationEmployee>();
        
        
        //StationPhotos
        CreateMap<StationPhoto, StationPhotoDto>();
        CreateMap<CreateStationPhotoDto, StationPhoto>();
        
        
        //Transactions
        CreateMap<Transaction, TransactionDto>();
        CreateMap<CreateTransactionDto, Transaction>();
    }
}