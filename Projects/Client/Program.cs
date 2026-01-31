using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

var anonymousAuthProvider = new AnonymousAuthenticationProvider();
var adapter = new HttpClientRequestAdapter(anonymousAuthProvider);
// TODO: Load from .env
adapter.BaseUrl = "http://localhost:5000";
var client = new Api.ApiClient(adapter);

var random = new Random();
const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
var randomKey = new string(Enumerable.Repeat(chars, 8)
	.Select(s => s[random.Next(s.Length)]).ToArray());

var email = $"test_{randomKey}@gmail.com";
var password = "12345678";

var signupResult = (await client.Auth.Signup.PostAsync(new Api.Models.AuthRequestDTO { Email = email, Password = password }))!;
var signinResult = (await client.Auth.Signin.PostAsync(new Api.Models.AuthRequestDTO { Email = email, Password = password }))!;

// Swap unauthenticated client with authenticated one
var authProvider = new BaseBearerTokenAuthenticationProvider(new BearerTokenProvider(signinResult.Token!));
adapter = new HttpClientRequestAdapter(authProvider);
// TODO: Load from .env
adapter.BaseUrl = "http://localhost:5000";
client = new Api.ApiClient(adapter);

var message1 = (await client.Messages.PostAsync(new Api.Models.MessageRequestDTO { Text = "test 1" }))!;
var message2 = (await client.Messages.PostAsync(new Api.Models.MessageRequestDTO { Text = "test 2" }))!;
var message3 = (await client.Messages.PostAsync(new Api.Models.MessageRequestDTO { Text = "test 3" }))!;

var messageList = (await client.Messages.GetAsync())!;
Console.WriteLine("Finished!");

class BearerTokenProvider : IAccessTokenProvider
{
	private string Token;

	public BearerTokenProvider(string token)
	{
		this.Token = token;
	}

	public Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = default, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(this.Token);
	}

	public AllowedHostsValidator AllowedHostsValidator => new AllowedHostsValidator();
}
