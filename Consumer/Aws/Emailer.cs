using System.Text.Json;
using System.Text.Json.Serialization;

using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Consumer.Aws;




public struct UserCreatedEmailOptions
{
    

    [JsonPropertyName("NAME")]
    public string Name { get; set; }
    [JsonPropertyName("EMAIL")]
    public string UserName { get; set; }
    [JsonPropertyName("PASSWORD")]
    public string Password { get; set; }
}


public struct QrReadEmailOptions{

    [JsonPropertyName("NAME")]
    public string Name { get; set; }
    [JsonPropertyName("DATE")]
    public string Date { get; set; }

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



    }

    public async Task SendQrReadEmailAsync(
        string identity,
        string to
        ,QrReadEmailOptions options)
    {
        var request = new Amazon.SimpleEmail.Model.SendTemplatedEmailRequest();
        request.Source = identity;
        request.Destination = new Destination
        {
            ToAddresses = new List<string>(){to}
        };
        request.Template = "QrRead";
        request.TemplateData = JsonSerializer.Serialize(options);
        var result = await _sesClient.SendTemplatedEmailAsync(request);
    }




    public async Task AddIdentityAsync(string email)
    {
        var request = new Amazon.SimpleEmail.Model.VerifyEmailIdentityRequest();
        request.EmailAddress = email;

        var result = await _sesClient.VerifyEmailIdentityAsync(request);

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

    }
}