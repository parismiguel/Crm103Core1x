using System.Linq;
using Abp.Configuration;
using Abp.Localization;
using Abp.Net.Mail;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly AbpProjectNameDbContext _context;

        public DefaultSettingsCreator(AbpProjectNameDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@vcaperu.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "VCA");

            //Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "es-PE");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (_context.Settings.Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(tenantId, null, name, value));
            _context.SaveChanges();
        }
    }
}