using AutoMapper;
using SupplierGuard.Application.Suppliers.DTOs;
using SupplierGuard.Domain.Entities;
using SupplierGuard.Domain.Enums;

namespace SupplierGuard.Application.Common.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between domain entities and DTOs.
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ===== SUPPLIER MAPPINGS =====

            // Supplier -> SupplierDto
            CreateMap<Supplier, SupplierDto>()
                .ForMember(dest => dest.LegalName, opt => opt.MapFrom(src => src.LegalName.Value))
                .ForMember(dest => dest.CommercialName, opt => opt.MapFrom(src => src.CommercialName.Value))
                .ForMember(dest => dest.TaxId, opt => opt.MapFrom(src => src.TaxId.Value))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber.Value))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.Website, opt => opt.MapFrom(src => src.Website != null ? src.Website.Value : null))
                .ForMember(dest => dest.PhysicalAddress, opt => opt.MapFrom(src => src.PhysicalAddress.Value))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.GetDisplayName()))
                .ForMember(dest => dest.CountryCode, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.AnnualRevenue, opt => opt.MapFrom(src => src.AnnualRevenue.Amount))
                .ForMember(dest => dest.AnnualRevenueFormatted, opt => opt.MapFrom(src => src.AnnualRevenue.Formatted()))
                .ForMember(dest => dest.IsHighRevenue, opt => opt.MapFrom(src => src.AnnualRevenue.IsHighRevenue()));

            // Supplier -> SupplierListDto (for listings)
            CreateMap<Supplier, SupplierListDto>()
                .ForMember(dest => dest.LegalName, opt => opt.MapFrom(src => src.LegalName.Value))
                .ForMember(dest => dest.CommercialName, opt => opt.MapFrom(src => src.CommercialName.Value))
                .ForMember(dest => dest.TaxId, opt => opt.MapFrom(src => src.TaxId.Value))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.GetDisplayName()))
                .ForMember(dest => dest.AnnualRevenue, opt => opt.MapFrom(src => src.AnnualRevenue.Amount))
                .ForMember(dest => dest.AnnualRevenueFormatted, opt => opt.MapFrom(src => src.AnnualRevenue.Formatted()));
        }
    }
}
