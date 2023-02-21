using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Consumer.Utils;




public struct UserCreatedEmailOptions
{
    

    [JsonPropertyName("NAME")]
    public string Name { get; set; }
    [JsonPropertyName("EMAIL")]
    public string UserName { get; set; }
    [JsonPropertyName("PASSWORD")]
    public string Password { get; set; }
}


public struct CreateTemplateRequestBody {
    
    public string HTML { get; set; }
    public string Text { get; set; }
    public string Subject { get; set; }
    public string TemplateName { get; set; }
}




public class Emailer
{
    private readonly IAmazonSimpleEmailService _sesClient;

    public Emailer(IAmazonSimpleEmailService sesClient)
    {
        _sesClient = sesClient;
    }


    //List Identities
    public async Task<List<string>> ListIdentities()
    {
        var result = await _sesClient.ListIdentitiesAsync();

        return result.Identities.ToList();
    }

    public async Task SendUserCreatedEmailAsync(UserCreatedEmailOptions options)
    {
        var request = new Amazon.SimpleEmail.Model.SendTemplatedEmailRequest();
        request.Source = "jpujol@adhumansoft.com";
        request.Destination = new Destination
        {
            ToAddresses = new List<string>(){options.UserName}
        };
        request.Template = "UserCreated";
        request.TemplateData = JsonSerializer.Serialize(options);

        var result = await _sesClient.SendTemplatedEmailAsync(request);

        Console.WriteLine("MessageId: {0}", result.MessageId);


    }




    public async Task AddIdentityAsync(string email)
    {
        var request = new Amazon.SimpleEmail.Model.VerifyEmailIdentityRequest();
        request.EmailAddress = email;

        var result = await _sesClient.VerifyEmailIdentityAsync(request);

        Console.WriteLine("Identity added: {0}", result.HttpStatusCode);
    }
    public async Task CreateTemplateAsync(
        CreateTemplateRequestBody request
    )
    {
        var _request = new CreateTemplateRequest();
        _request.Template = new Template
        {
            TemplateName = request.TemplateName,
            HtmlPart = request.HTML,
            TextPart = request.Text,
            SubjectPart = request.Subject
        };

        var result = await _sesClient.CreateTemplateAsync(_request);

        Console.WriteLine("Template created: {0}", result.HttpStatusCode);
    }


    public async Task UpdateTemplateAsync(
        CreateTemplateRequestBody request
    )
    {
        var _request = new UpdateTemplateRequest();
        _request.Template = new Template
        {
            TemplateName = request.TemplateName,
            HtmlPart = request.HTML,
            TextPart = request.Text,
            SubjectPart = request.Subject
        };

        var result = await _sesClient.UpdateTemplateAsync(_request);

        Console.WriteLine("Template updated: {0}", result.HttpStatusCode);
    }
}