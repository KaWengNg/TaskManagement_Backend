
# Part B: Security & Architecture Questions

## Security Questions

### Question 1:Multi-Tenancy & Data Isolation
How would you prevent users from accessing tasks that belong to other organizations in a
multi-tenant application? Describe your approach including middleware, claims-based
authorization, and data filtering strategies.

### Answer :
Implement db_per_tenant, share db schema with a tenant_id column on every multi-tenant table.
Detect the tenant in middleware before every request reaches to the API by adding tenant_id in the request header.
Encoded tenant_id using JWT token will then be decoded and read the claim's expiration in the middleware.
Implement global filtering by tenant_id in data access layer, to prevent forgot "where tenant_id = " statement.

### Question 2: JWT Storage
Would you store JWTs in localStorage, sessionStorage, or httpOnly cookies? Explain the
security implications of each approach and defend your choice. Consider XSS and CSRF
attacks in your answer.

### Answer :
I would store JWT in httpOnly cookies because they cannot be accessed by JavaScript, reducing XSS risk.
I would enable SameSite=strict to prevent cross-site request forgery.

### Question 3: Sensitive Data in Logs
How would you prevent logging sensitive user data (passwords, tokens, personally
identifiable information) when using Serilog or ILogger? Provide specific techniques or code
examples.

### Answer :
Serilog :
using Serilog
Log.Logger = new LoggerConfiguration()
    .Filter.ByExcluding("Contains(@Message, 'password') or Contains(@Message, 'token')")
    .WriteTo.File("logs/log.txt")
    .CreateLogger();

Ilogger: only log the information that are not sensitive
using Microsoft.Extensions.Logging;
_logger.LogInformation("User login attempt for userId {UserId}", userId);

### Question 4: Validation Limitations
Data annotations like [Required] can prevent null values, but what security risks can they
NOT prevent? Provide a specific example and explain how you would mitigate it.

### Answer :
Data annotation does not protect against XSS (cross-site scripting). 
To mitigate this, I will use Html sanitizer in middleware to detect javascript or html scripting for incoming input.

### Question 5: Error Response Security
How do you ensure error responses never reveal sensitive information (stack traces, database
connection strings, internal file paths) in production while remaining useful for debugging?
What specific techniques would you use?

### Answer :
In production, I will not expose raw exceptions or stack traces by configuring global exception handler (UseExceptionHandler("/error")) to log the full details internally while returning a generic error response to the client. Detailed stack traces are only shown in development mode via UseDeveloperExceptionPage().

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
}
else
{
    app.UseExceptionHandler("/error"); 
}

app.Map("/error", (HttpContext context) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogError("Unhandled exception at {Path}", context.Request.Path);
    return Results.Problem("An unexpected error occurred. Please try again later.");
});

### Question 6: API Key Management
How would you securely store and manage API keys for third-party integrations in ASP.NET
Core? Consider both development and production environments, and explain your approach
to configuration management.

### Answer :
In development I will use appsettings.json. For production I will use environment variables and Azure Key Vault to manage secrets as well as API keys securely. But I never set this up before.

### Question 7: SQL Injection Prevention
How do security concerns influence your choice between raw SQL queries and Entity
Framework Core? What specific EF Core practices help prevent SQL injection attacks?

### Answer :
Due to security concerns I will use EF core since it automatically parameterise query to prevent SQL injection. If there is a situation where raw sql is unavoidable, I will use FromSqlInterpolated() or ExecuteSqlRaw() with parameters instead of concatenating the input.

## Architecture Questions
### Question 8: Dependency Injection Lifetimes
Explain the difference between Singleton, Scoped, and Transient service lifetimes in
ASP.NET Core. When would you use each for:
• A DbContext
• A logger service
• A cache service

### Answer :
Transient - new instance is created every time it’s requested.
Scoped - one instance per request.
Singleton - one instance for entire application lifetime.

DBContext - I will use Scoped to have own instance for each request. If using Singleton it will have concurrency exception, multiple threads will cause data leaks. If using Transient, it will be inefficient since each repository gets a new context per call.

LoggerService - I will use Singleton since the logger service is thread safe which means the object can be accessed by multiple thread concurrently without causing data corruption.

Cache Service - I will use Singleton to ensure consistent access. If using scope or transient, it defeats the caching purpose.

### Question 9: Asynchronous Programming
Why is it important to use async/await for database operations in ASP.NET Core? What
problems can occur if you mix synchronous and asynchronous code incorrectly?

### Answer :
Async/ await allow database operation to run without blocking the request thread. Using await frees the request thread to handle other request while waiting the DB to respond. In Synchronous, every request blocked thread cause one less thread can be used to process incoming request. 

Mixing sync and async can causes deadlock where synchorization waiting asynchronies task to complete while asynchronous task is waiting the context to free the thread. Also, mixing it can cause inconsistent transaction scope or partial cancellation behaviour if the request is aborted in the middle of the operation.

### Question 10: Frontend State Management
Describe how you managed state for the tasks list in your React application. Why did you
choose that approach over alternatives? What would make you reconsider your choice?

### Answer :
I managed the tasks list using React built-in useState and useEffect hooks. I choose this approach because it’s simple, efficient for a small single page app. When it comes to more complex for example, multiple pages sharing the same task data / states, I will consider to use Redux.

