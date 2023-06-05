using AutoMapper;
using Genesis.App.Contract.Authentication.ApiModels;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Authentication;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.Dtos.Account;

namespace Genesis.App.Implementation.Utils
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            DisableConstructorMapping();
            CreateAccountMap();
            CreateBllEntitiesMap();
        }

        private void CreateAccountMap()
        {
            CreateMap<AccountDto, AuthenticateResponse>()
                .ForMember(resp => resp.FirstName, opt => 
                    opt.MapFrom(origin => origin.GetRootPerson().FirstName))
                .ForMember(resp => resp.LastName, opt =>
                    opt.MapFrom(origin => origin.GetRootPerson().FirstName))
                .ForMember(resp => resp.Email, opt =>
                    opt.MapFrom(origin => origin.Login))
                .ForMember(resp => resp.Roles, opt =>
                    opt.MapFrom(origin => origin.Roles.Select(r => r.RoleName)));

            CreateMap<RefreshTokenDto, RefreshToken>().ReverseMap();
            CreateMap<AccountDto, Account>()
                .ForMember(resp => resp.Roles, opt =>
                    opt.MapFrom(origin => origin.Roles.Select(r => r.RoleName).ToList()))
                .ForMember(resp => resp.RefreshTokens, opt =>
                    opt.MapFrom(origin => origin.RefreshTokens.ToList()));
            CreateMap<Account,AccountDto>()
                .ForMember(resp => resp.CountryCode, opt =>
                    opt.MapFrom(origin => origin.Country.Code))
                .ForMember(resp => resp.CityId, opt =>
                    opt.MapFrom(origin => origin.City.Id));
        }

        private void CreateBllEntitiesMap()
        {
            CreateMap<PersonDto, Person>().ReverseMap();
            CreateMap<BiographyDto, Biography>().ReverseMap();
            CreateMap<AddressDto, Address>().ReverseMap();
            CreateMap<DocumentDto, Document>().ReverseMap();
            CreateMap<GenealogicalTreeDto, GenealogicalTree>().ReverseMap();
            CreateMap<HistoricalNotationDto, HistoricalNotation>().ReverseMap();
            CreateMap<PictureDto, Picture>().ReverseMap();
            CreateMap<PersonRelationDto, PersonRelation>().ReverseMap();
            CreateMap<AccountConnectionDto, AccountConnection>().ReverseMap();
        }
    }
}
