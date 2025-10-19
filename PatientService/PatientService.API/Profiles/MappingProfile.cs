using AutoMapper;
using PatientService.API.Models;
using PatientService.Domain.Entities;

namespace PatientService.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Metadata
        CreateMap<LookupEntity, MetadataDto>();

        // Addresses
        CreateMap<AddressDto, PatientAddress>();
        CreateMap<PatientAddress, AddressDto>();

        // Phones
        CreateMap<PhoneDto, PatientPhone>();
        CreateMap<PatientPhone, PhoneDto>();

        // Emergency Contacts
        CreateMap<EmergencyContactDto, EmergencyContact>();
        CreateMap<EmergencyContact, EmergencyContactDto>();

        // Insurance Policies
        CreateMap<InsurancePolicyDto, InsurancePolicy>();
        CreateMap<InsurancePolicy, InsurancePolicyDto>();

        // Social History & Legal Consents
        CreateMap<SocialHistoryDto, SocialHistory>().ReverseMap();
        CreateMap<LegalConsentDto, LegalConsent>().ReverseMap();

        // Allergies
        CreateMap<AllergyDto, Allergy>()
            .ForMember(dest => dest.AllergyId, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "Patient"));

        // Medications
        CreateMap<MedicationDto, Medication>()
            .ForMember(dest => dest.MedicationId, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "Patient"));

        // Surgeries
        CreateMap<SurgeryDto, PatientSurgery>()
            .ForMember(dest => dest.SurgeryId, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Hospitalizations
        CreateMap<HospitalizationDto, PatientHospitalization>()
            .ForMember(dest => dest.HospitalizationId, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Family Medical History
        CreateMap<FamilyHistoryDto, FamilyMedicalHistory>()
            .ForMember(dest => dest.FamilyHistoryId, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Patient Core
        CreateMap<PatientCoreDto, Patient>()
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Id == Guid.Empty ? Guid.NewGuid() : src.Id))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => "Patient"))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));

        CreateMap<Patient, PatientCoreDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PatientId));
    }
}
