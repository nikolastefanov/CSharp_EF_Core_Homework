using AutoMapper;
using CarDealer.DataTransferObject;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<Supplier, ExportLocalSuppliersDto>();
        }
    }
}
