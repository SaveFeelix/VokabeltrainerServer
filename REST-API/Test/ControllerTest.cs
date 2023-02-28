using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using REST_API.Data.Api.Dto.User;
using REST_API.Data.Api.Dto.Vocable;
using REST_API.Data.Api.Dto.VocableCollection;
using REST_API.Test.Attributes;
using REST_API.Test.Service;
using Xunit;
using Xunit.Abstractions;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

namespace REST_API.Test;

[TestCaseOrderer("REST_API.Test.Order.PriorityOrder", "REST-API")]
public class ControllerTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly ITestOutputHelper _logger;
    private readonly WebApplicationFactory<Program> _factory;

    public ControllerTest(ITestOutputHelper logger)
    {
        _logger = logger;
        _factory = new WebApplicationFactory<Program>();
    }


    #region Auth

    [Theory(DisplayName = "Auth - Register"), TestPriority(1)]
    [InlineData("test@test.de", "test", "test")]
    public async Task RegisterTest(string email, string userName, string password)
    {
        var response = await Request("Auth/Register", HttpMethod.Post, null,
            new UserRegisterDto(userName, email, password));
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    [Theory(DisplayName = "Auth - Login"), TestPriority(2)]
    [InlineData("test", "test")]
    public async Task LoginTest(string userName, string password)
    {
        var response =
            await Request("Auth/Login", HttpMethod.Post, null, new UserLoginDto(userName, password));
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
        TmpService.Token = response.Value.Replace("\"", "");
    }

    #endregion

    #region Collection

    [Theory(DisplayName = "Collection - Create"), TestPriority(3)]
    [InlineData("TestCollection")]
    [InlineData("TestCollection2")]
    [InlineData("TestCollection3")]
    public async Task CollectionCreateTest(string collectionName)
    {
        var response =
            await Request("VocableCollection/Create", HttpMethod.Post, TmpService.Token,
                new VocableCollectionCreateDto(collectionName));
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    [Theory(DisplayName = "Collection - Update"), TestPriority(4)]
    [InlineData(1, "TestCollection 1.0")]
    public async Task CollectionUpdateTest(int id, string collectionName)
    {
        var response =
            await Request("VocableCollection/Update", HttpMethod.Put, TmpService.Token,
                new VocableCollectionUpdateDto(id, collectionName));
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    [Theory(DisplayName = "Collection - Delete"), TestPriority(5)]
    [InlineData(1)]
    public async Task CollectionDeleteTest(int id)
    {
        var response =
            await Request<object>($"VocableCollection/Delete?id={id}", HttpMethod.Delete, TmpService.Token);
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    [Fact(DisplayName = "Collection - All"), TestPriority(6)]
    public async Task CollectionAllTest()
    {
        var response =
            await Request<object>("VocableCollection/All", HttpMethod.Get, TmpService.Token);
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    #endregion

    #region Cards

    [Theory(DisplayName = "Card - Create"), TestPriority(7)]
    [InlineData(2, "Card 1", "card1;card1;card1")]
    [InlineData(2, "Card 2", "card2;card2;card2")]
    [InlineData(2, "Card 3", "card3;card3;card3")]
    public async Task CardCreateTest(int collectionId, string display, string possibleAnswers)
    {
        var response =
            await Request("Vocable/Create", HttpMethod.Post, TmpService.Token,
                new VocableCreateDto(collectionId, display, possibleAnswers));
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    [Theory(DisplayName = "Card - Update"), TestPriority(8)]
    [InlineData(1, "Card - 1.1", "card1.1;card1.1;card1.1")]
    public async Task CardUpdateTest(int cardId, string display, string possibleAnswers)
    {
        var response =
            await Request("Vocable/Update", HttpMethod.Put, TmpService.Token,
                new VocableUpdateDto(cardId, display, possibleAnswers));
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    [Theory(DisplayName = "Card - Delete"), TestPriority(9)]
    [InlineData(2)]
    public async Task CardDeleteTest(int cardId)
    {
        var response =
            await Request<object>($"Vocable/Delete?id={cardId}", HttpMethod.Delete, TmpService.Token);
        Assert.Equal(HttpStatusCode.OK, response.Key.StatusCode);
        WriteInformation(response);
    }

    #endregion

    public async Task<KeyValuePair<HttpResponseMessage, string>> Request<TData>([StringSyntax("uri")] string uri,
        HttpMethod method, string? token, TData? data = default)
    {
        using var client = _factory.CreateClient();

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var responseMessage = method switch
        {
            HttpMethod.Get => await client.GetAsync(uri),
            HttpMethod.Delete => await client.DeleteAsync(uri),
            HttpMethod.Post => await client.PostAsJsonAsync(uri, data),
            HttpMethod.Put => await client.PutAsJsonAsync(uri, data),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };

        return new KeyValuePair<HttpResponseMessage, string>(responseMessage,
            await responseMessage.Content.ReadAsStringAsync());
    }

    private void WriteInformation(KeyValuePair<HttpResponseMessage, string> response)
    {
        _logger.WriteLine("Information:");
        _logger.WriteLine(" - StatusCode: " + response.Key.StatusCode);
        _logger.WriteLine(" - StatusCode (int): " + (int)response.Key.StatusCode);
        _logger.WriteLine(" - URI: " + response.Key.RequestMessage?.RequestUri);
        _logger.WriteLine(" - Method: " + response.Key.RequestMessage?.Method);
        if (string.IsNullOrEmpty(response.Value))
            return;
        _logger.WriteLine("Data: ");
        _logger.WriteLine(response.Value);
    }
}