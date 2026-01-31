using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

var authProvider = new AnonymousAuthenticationProvider();
var adapter = new HttpClientRequestAdapter(authProvider);
var client = new Api.ApiClient(adapter);

var email = "test@gmail.com";
var password = "12345678";
var result = await client.Auth.Signin.PostAsync(new Api.Models.AuthRequestDTO { Email = email, Password = password });
Console.WriteLine(result.Token);
