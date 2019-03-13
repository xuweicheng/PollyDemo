# Polly POC
This POC mimic the infrastructure of Teleclaim. These are key projects:
- **MyClaims.Web** is a web app shows list of Claims, creates a claim.
- **FunctionApi** has two Azure functions GetClaims and CreateClaim.
- **CmsIntegrationApi** is a web api has endpoints of getting all claims, get one claim, and post new claim. The get all claims endpoint **fails** every other call. The post new claim endpoint checks is **Bearer** token is **valid_token**.

### Communication flow

**MyClaims.Web** -> **FunctionApi** -> **CmsIntegrationApi**

### Polly policy
#### Retry on get all claims
**FunctionApi**, in **startup.cs**, wraps **HttpClient** with **WaitAndRetryAsync** policy, and injects the **HttpClient** in **ClaimsClient**.
When **CmsIntegrationApi** get all claims endpoint failed, **HttpClient** will try 3 times and wait for a second before a retry.
```csharp
	services.AddHttpClient<IClaimsClient, ClaimsClient>()
		.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new []
		{
			TimeSpan.FromSeconds(1),
			TimeSpan.FromSeconds(1),
			TimeSpan.FromSeconds(1)
		}));
```

In **GetClaimsFunction.cs**, call **IClaimsClient.GetAllAsync()** as if polly does not exist.
The wrapped **HttpClient** will handle the retry.
```csharp
	var claims = await claimsClient.GetAllAsync();
```
#### Retry with token refresh
**Function Api**, in  **startup.cs**, creates a **RetryAsync** policy, which activates when the http response result is "401", the policy will refresh the token to make it valid, and retry.
```csharp
        var authPolicy = Policy.HandleResult<string>(result => result == "401")
            .RetryAsync(
            retryCount: 1,
            onRetry: (response, retryNumber, context) =>
            {
                var tokenService = provider.GetService<ITokenService>();
                tokenService.RefreshToken();
                //telemetryClient.log retried
            });
        var policyRegistry = services.AddPolicyRegistry();
        policyRegistry.Add("auth_policy", authPolicy);
```
In **CreateClaimFunction.cs**, get the policy from policy registry, and execute.
```csharp
        string referenceNum = default(string);

        var authPolicy = policyRegistry.Get<IAsyncPolicy<string>>("auth_policy");

        await authPolicy.ExecuteAsync(
                async () =>
                referenceNum = await claimsClient.PostAsync(myClaim)
            );
```

#### Retry in web
**MyClaims.Web**, in **startup.cs**, wraps **HttpClient** with **WaitAndRetryAsync** policy, and injects into **FunctionApiClient**
``` C#
        services.AddHttpClient<IFunctionApiClient, FunctionApiClient>()
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1)
            }));
```
In **HomeController**, when calling the following lines
```C#
...
var claims = await functionApiClient.GetAllAsync();
...
string referenceNum = await functionApiClient.PostAsync(myClaim);
...
``` 
Within each **FunctionApiClient** method, the **WaitAndRetryAsync** policy will trigger **HttpClient.SendAsync(...)**.
```C#
var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
```