using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

var authProvider = new AnonymousAuthenticationProvider();
var adapter = new HttpClientRequestAdapter(authProvider);
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
Console.WriteLine(signupResult.Token);

var signinResult = (await client.Auth.Signin.PostAsync(new Api.Models.AuthRequestDTO { Email = email, Password = password }))!;
Console.WriteLine(signinResult.Token);
