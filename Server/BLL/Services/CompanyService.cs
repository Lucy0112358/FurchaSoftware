using FurchaBLL.Models;
using FurchaDAL.Models;

namespace FurchaBLL.Services
{
    public class CompanyService
    {
        private readonly furchaContext Db;
        private readonly MqttService _mqttService;
        public CompanyService(furchaContext db, MqttService mqttService)
        {
            Db = db;
            _mqttService = mqttService;
        }

        public async Task RegisterCompany(BllCompany newCompany)
        {
            var company = new Company
            {
                Name = newCompany.Name,
                City = newCompany.City,
                Street = newCompany.Street,
                Country = newCompany.Country,
                Email = newCompany.Email,
                Phone = newCompany.Phone,
                CountryCode = newCompany.CountryCode
            };

            var dbCompany = await Db.Companies.AddAsync(company);
            await Db.SaveChangesAsync();

            if (dbCompany != null)
            {
                await _mqttService.AddAccount(dbCompany.Entity.AccountUid, "123456789"); // to add pass hashing
            }
        }
    }
}
