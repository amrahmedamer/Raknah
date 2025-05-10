using Microsoft.AspNetCore.Identity.UI.Services;

namespace Raknah.Extensions;

public static class EmailExtension
{
    public static async Task SendEmailAsync(this IEmailSender emailSender, string email, string subject, string templateName, Dictionary<string, string> placeholders)
    {

        var template = File.ReadAllText($"Templates/{templateName}.html");

        foreach (var item in placeholders)
            template = template.Replace(item.Key, item.Value);

        await emailSender.SendEmailAsync(
            email,
            subject,
            template
        );

    }

}
