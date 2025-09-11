using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVCarbonMarketplace.Model.Utils
{
    public static class EmailUtil
    {
        public static string GetTemplate(string fileName, Dictionary<string, string> placeholders)
        {
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var templatePath = Path.Combine(wwwRootPath, "html", fileName);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Không tìm thấy template email: {fileName}");

            var templateContent = File.ReadAllText(templatePath);

            if (placeholders != null)
            {
                foreach (var item in placeholders)
                {
                    templateContent = templateContent.Replace("{" + item.Key + "}", item.Value ?? string.Empty);
                }
            }

            return templateContent;
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var emailAttr = new EmailAddressAttribute();
            return emailAttr.IsValid(email);
        }
    }
}
