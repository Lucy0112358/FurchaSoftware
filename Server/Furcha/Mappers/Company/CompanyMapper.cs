using FurchaAdminApi.Models.Request;
using FurchaBLL.Models;

namespace FurchaAdminApi.Mappers.Company
{
    public static class CompanyMapper
    {
        public static BllCompany ToBllCompany(this CreateCompanyRequest company)
        {
            return new BllCompany
            {
                Name = company.Name,
                City = company.City,
                Street = company.Street,
                Country = company.Country,
                Email = company.Email,
                Phone = company.Phone,
                CountryCode = company.CountryCode
            };
        }
    }
}
