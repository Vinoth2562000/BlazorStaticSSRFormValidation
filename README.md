# Blazor Static SSR Form Validation Test Application

## 📋 Overview

This is a **production-ready test application** for GitHub Issue #69531: **[Validation] Browser feedback and server validation in a static SSR form**

A comprehensive validation sample demonstrating Blazor Static SSR forms with:
- ✅ Client-side validation feedback (DataAnnotations) without interactive components
- ✅ Server-side validation with custom rules
- ✅ Browser validation on blur + revalidation on input
- ✅ HTTP form submission with proper error/success handling
- ✅ Reserved name validation (server-only custom attribute)
- ✅ Bootstrap 5 responsive UI with proper styling

---

## 🔧 Technical Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| **.NET SDK** | 11.0.100-rc.1.26425.128 | Framework (latest RC) |
| **Blazor** | Static SSR | Server-side rendering (no interactivity) |
| **ASP.NET Core** | 11.0 RC1 | Web API endpoints |
| **Bootstrap** | 5.3 | Responsive UI framework |
| **C#** | 12+ | Language version |

**Configuration**: `rollForward: latestPatch` (follows latest .NET 11 patch)

---

## 📁 Project Structure

```
BlazorStaticSSRFormValidation/
│
├── global.json                            # SDK version config (11.0.100-rc.1.26425.128)
├── NuGet.config                           # NuGet package sources
├── README.md                              # This file
│
└── StaticSSRFormValidation/               # Main Blazor application
    │
    ├── Components/
    │   ├── Pages/
    │   │   ├── Home.razor                 # Home page with project overview
    │   │   ├── Registration.razor         # Main form for testing (STATIC SSR)
    │   │   │                              # - EditForm with FormName binding
    │   │   │                              # - DataAnnotationsValidator
    │   │   │                              # - ValidationMessage components
    │   │   │                              # - Query param handling for success message
    │   │   ├── Error.razor                # Error page handler
    │   │   ├── Weather.razor              # Sample page
    │   │   └── NotFound.razor             # 404 handler
    │   │
    │   ├── Layout/
    │   │   ├── MainLayout.razor           # Main page layout
    │   │   ├── MainLayout.razor.css       # Layout styles
    │   │   ├── NavMenu.razor              # Navigation menu
    │   │   ├── NavMenu.razor.css
    │   │   └── NavMenu.razor.js
    │   │
    │   ├── App.razor                      # App root component
    │   ├── Routes.razor                   # Route definitions
    │   └── _Imports.razor                 # Global imports
    │
    ├── Models/
    │   └── RegistrationModel.cs           # Form data model with validation attributes
    │                                      # - [Required] attributes
    │                                      # - [EmailAddress] validation
    │                                      # - [Range(18, 120)] age validation
    │                                      # - [ReservedName] custom server-only attribute
    │
    ├── Endpoints/
    │   └── RegistrationEndpoint.cs        # POST /api/register handler
    │                                      # - Extracts form data (Model.* prefixed)
    │                                      # - Server-side validation
    │                                      # - Returns 200 (redirect) or 400 (errors)
    │
    ├── Properties/
    │   └── launchSettings.json            # Launch configuration (https://localhost:7155)
    │
    ├── wwwroot/
    │   ├── app.css                        # Application styles
    │   └── lib/bootstrap/                 # Bootstrap 5 CSS framework
    │
    ├── Program.cs                         # Application startup
    │                                      # - Service registration
    │                                      # - Middleware pipeline
    │                                      # - Endpoint mapping
    │
    ├── appsettings.json                   # Production settings
    ├── appsettings.Development.json       # Development settings
    │
    ├── StaticSSRFormValidation.csproj     # Project file
    │
    └── bin/
        └── Release/net11.0/publish/       # Published app output
```

---

## 🎯 Key Files Overview

| File | Purpose | Technology |
|------|---------|-----------|
| `Registration.razor` | Main form page | Blazor Static SSR, EditForm, DataAnnotations |
| `RegistrationModel.cs` | Form data + validation rules | C# DataAnnotations attributes |
| `RegistrationEndpoint.cs` | API handler | ASP.NET Core minimal API |
| `Program.cs` | App configuration | Dependency injection, routing |
| `global.json` | SDK version pinning | .NET 11 RC1 |

---

## ✨ Key Features

### 1. Static SSR Form Architecture
| Feature | Implementation |
|---------|-----------------|
| **Form Binding** | `<EditForm>` with `FormName="registrationForm"` |
| **Validation** | `<DataAnnotationsValidator />` component |
| **Field Feedback** | `<ValidationMessage For="@(() => Model.Field)" />` |
| **Error Overview** | `<ValidationSummary />` card display |
| **Submission** | HTTP form POST (method="post" action="/api/register") |
| **Styling** | Bootstrap 5 form-control, is-invalid classes |
| **No Native HTML** | No `required`, `type="email"`, or `pattern` attributes |

**Reason**: Pure DataAnnotations validation for consistent browser + server rules ✅

---

### 2. Validation Rules

#### ✅ Client-Side (Browser) Validation
Handled by `DataAnnotationsValidator` component:

| Field | Rules | Error Message |
|-------|-------|---------------|
| **Name** | Required | "The Name field is required" |
| **Email** | Required | "The Email field is required" |
| **Email** | Valid format | "The Email field is not a valid e-mail address" |
| **Age** | Required | "The Age field is required" |
| **Age** | Range 18-120 | "The field Age must be between 18 and 120" |

**Note**: Server-side validation **NOT** available to client (no HTML5 constraints)

#### 🔒 Server-Side Validation (POST `/api/register`)
Additional rules enforced on server:

| Field | Rule | Error Message |
|-------|------|---------------|
| **Name** | Not reserved | "'[name]' is a reserved name and cannot be used" |

**Reserved Names**: admin, administrator, system, root, test (case-insensitive)

**Custom Implementation**: `ReservedNameAttribute : ValidationAttribute`

---

### 3. Form Interaction Flow

#### 📝 Before Blur (Initial State)
```
- User types in field
- No validation feedback shown
- Field appears normal
- No styling changes
```

#### ❌ After Blur (Field Invalid)
```
- Validation message appears under field
- Field border turns red (is-invalid class)
- ValidationSummary updates with error
- NO form submission sent
```

#### ✏️ During Correction (Revalidation)
```
- User types to fix invalid field
- Validation runs automatically
- Error message CLEARS immediately
- Field styling returns to normal
- NO additional blur needed
- Still NO form submission
```

#### ✔️ On Form Submit

**Path A: Invalid Data**
```
- User clicks Submit button
- Client validates all fields
- Has errors → NO POST sent
- Form stays on page with errors
- User sees ValidationSummary
```

**Path B: Valid Data + Server Accepts**
```
- User clicks Submit button
- Client validates → all pass
- POST sent to /api/register
- Server validates
- Success: HTTP 200
- Form redirects to /registration?success=true&name=John
- GREEN success alert displays: "Registration successful! Thank you, John"
- Form resets
```

**Path C: Valid Data + Server Rejects**
```
- User clicks Submit button
- Client validates → all pass
- POST sent to /api/register
- Server validation fails (e.g., reserved name)
- Returns HTTP 400 with errors
- Form stays on /registration (NO redirect)
- Red error message displays under Name field
- ValidationSummary shows error
- User can correct and resubmit
```

---

## 🚀 Getting Started

### ⚙️ Prerequisites

| Requirement | Version | Purpose |
|-------------|---------|---------|
| **.NET SDK** | 11.0 RC1 or later | Compile and run |
| **Node.js** | 18+ (optional) | Build tools if needed |
| **Code Editor** | VS Code, Visual Studio, or similar | Edit code |
| **Browser** | Chrome, Edge, Firefox (modern) | View app + DevTools |

**Verify .NET installation:**
```bash
dotnet --version
```

Should output: `11.0.100-rc.1.26425.128` or later

---

### 📥 Installation & Setup

**Step 1: Clone/Navigate to project**
```bash
cd d:\Validation\BlazorStaticSSRFormValidation
```

**Step 2: Restore NuGet packages**
```bash
dotnet restore
```

**Step 3: Verify project structure**
```bash
cd StaticSSRFormValidation
dotnet build
```

---

### 🔧 Running in Development

**Start the application:**
```bash
cd StaticSSRFormValidation
dotnet run
```

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7155
      Now listening on: http://localhost:5155
```

**Open in browser:**
- Home Page: `https://localhost:7155/`
- Registration Form: `https://localhost:7155/registration`

**Stop the app:** Press `Ctrl+C` in terminal

---

### 📦 Publishing for Production

**Build release version:**
```bash
cd StaticSSRFormValidation
dotnet publish -c Release
```

**Published app location:**
```
bin/Release/net11.0/publish/
```

**Files included:**
- `StaticSSRFormValidation.exe` (Windows executable)
- `StaticSSRFormValidation` (Linux/Mac)
- `StaticSSRFormValidation.dll` (Runtime)
- Static assets, configs, dependencies

**Run published app:**

Windows:
```bash
cd bin\Release\net11.0\publish
.\StaticSSRFormValidation.exe
```

Linux/Mac:
```bash
cd bin/Release/net11.0/publish
./StaticSSRFormValidation
```

**Or run via dotnet:**
```bash
dotnet bin/Release/net11.0/publish/StaticSSRFormValidation.dll
```

**Quick one-liner (from project root):**
```bash
dotnet publish -c Release && cd bin\Release\net11.0\publish && .\StaticSSRFormValidation.exe
```

**Access published app:**
```
https://localhost:7155/
https://localhost:7155/registration
```

---

## 🎬 Test Scenarios & Recording Steps

### Test Prerequisites
- Published app running at `https://localhost:7155/registration`
- DevTools open: F12 → Network tab
- Network tab kept visible during all tests

---

### 📹 Scenario 1: Browser Validation - Invalid Email (No POST)

**Video Name:** `01-Browser-Validation-Invalid-Email-No-POST`

**Steps:**
1. Show blank form
2. Type `invalidemail` in Email field
3. Press Tab (blur from field)
4. ✅ Verify: Red error message appears
5. ✅ Verify: Network tab shows 0 requests (NO POST)
6. Say: "Invalid email detected by browser — validation message shows without server call"

**Expected Result:** Message appears, form stays valid, no POST sent ✅

---

### 📹 Scenario 2: Browser Validation - Email Correction

**Video Name:** `02-Browser-Validation-Email-Correction`

**Steps:**
1. (Continuing from Scenario 1) Email field has error
2. Clear and type `user@gmail.com`
3. ✅ Verify: Error message disappears immediately
4. ✅ Verify: Network tab still shows 0 requests
5. Say: "Valid email entered — error clears without additional blur or POST"

**Expected Result:** Error clears automatically, no POST sent ✅

---

### 📹 Scenario 3: Invalid Submit - All Empty Fields

**Video Name:** `03-Invalid-Submit-All-Empty-Fields`

**Steps:**
1. Show blank form (all fields empty)
2. Click "Submit Registration" button
3. ✅ Verify: Network tab shows POST request to `/api/register`
4. Click POST request → Response tab
5. ✅ Verify: HTTP 400 response with three errors:
   ```json
   {
     "errors": {
       "Name": ["The Name field is required"],
       "Email": ["The Email field is required"],
       "Age": ["The Age field is required"]
     }
   }
   ```
6. Pan to form
7. ✅ Verify: All three fields show red error messages
8. ✅ Verify: ValidationSummary card displays all three errors
9. ✅ Verify: Form stays on `/registration` (NO redirect)
10. Say: "Submitted empty form — server validates all fields, errors display, form preserved"

**Expected Result:** Server validates, errors shown, form recoverable ✅

---

### 📹 Scenario 4: Server Validation - Reserved Name

**Video Name:** `04-Server-Validation-Reserved-Name`

**Steps:**
1. Fill form with:
   - Name: `admin` (reserved)
   - Email: `admin@gmail.com` (valid)
   - Age: `25` (valid)
2. ✅ Verify: No browser validation errors (all fields pass client validation)
3. Click Submit
4. ✅ Verify: Network tab shows POST to `/api/register`
5. Click POST → Response tab
6. ✅ Verify: HTTP 400 response with error:
   ```json
   {
     "errors": {
       "Name": ["'admin' is a reserved name and cannot be used"]
     }
   }
   ```
7. Pan to form
8. ✅ Verify: Red error message under Name field
9. ✅ Verify: ValidationSummary shows error
10. ✅ Verify: Form stays on `/registration` (NO redirect)
11. Say: "Reserved name rejected by server — validation error shows, only this field invalid despite valid other fields"

**Expected Result:** Server-only validation works, custom rule enforced ✅

**Reserved Names to Test:**
- `admin`
- `system`
- `root`
- `test`
- `administrator`

---

### 📹 Scenario 5: Valid Submission with Success

**Video Name:** `05-Valid-Submission-Success`

**Steps:**
1. Fill form with:
   - Name: `john` (valid, not reserved)
   - Email: `john@gmail.com` (valid)
   - Age: `25` (valid 18-120)
2. ✅ Verify: No validation errors on form
3. Click Submit
4. ✅ Verify: Network tab shows POST to `/api/register`
5. Click POST → Response tab (Response tab shows 200 OK)
6. Pan to form
7. ✅ Verify: URL changed to `/registration?success=true&name=john`
8. ✅ Verify: GREEN alert displays: "Registration successful! Thank you, john. Your registration has been submitted."
9. ✅ Verify: Form fields are CLEARED/RESET
10. Say: "Valid data accepted — HTTP 200 response, form redirects with success message, form ready for next entry"

**Expected Result:** Success flow works end-to-end ✅

---

### 📹 Scenario 6: Complete Flow - Invalid Email + Correction + Submit

**Video Name:** `06-Complete-Flow-Invalid-Email-Correction-Valid-Submit`

**Steps:**

**Part A: Invalid Email (Browser Validation)**
1. Type `invalidemail` in Email field
2. Press Tab
3. ✅ Verify: Error message appears, Network shows 0 requests

**Part B: Email Correction**
4. Type `jane@gmail.com` in Email field
5. ✅ Verify: Error clears, Network still 0 requests

**Part C: Fill and Submit Valid Data**
6. Fill:
   - Name: `jane` (clear any previous)
   - Email: `jane@gmail.com` (already correct)
   - Age: `30`
7. ✅ Verify: No validation errors on form
8. Click Submit
9. ✅ Verify: Network POST request
10. ✅ Verify: HTTP 200 response
11. ✅ Verify: Redirect with success alert
12. Say: "Complete user flow — browser catches format error, user corrects, valid submit succeeds with server confirmation"

**Expected Result:** Full user journey validated ✅

---

### 📋 Quick Test Checklist

```
☐ Test 1: Browser validation on blur (email invalid)
☐ Test 2: Email correction clears error
☐ Test 3: Empty submit shows all errors
☐ Test 4: Reserved name rejected by server
☐ Test 5: Valid data submits successfully
☐ Test 6: Complete flow end-to-end

Evidence to Collect:
☐ Network tab POST requests
☐ Server response JSON (400/200)
☐ Form error messages displayed
☐ Success redirect with alert
☐ Form reset after success
```

---

## 🔌 API Endpoints

### GET `/`
**Purpose:** Home page with project overview

**Response:** HTML page

---

### GET `/registration`
**Purpose:** Registration form page (main test page)

**Response:** HTML form with form fields and validators

**Query Parameters:**
- `success=true` - Shows success alert
- `name={userName}` - Personalized message

---

### POST `/api/register`
**Purpose:** Form submission and server-side validation

**Response (Success - HTTP 200):** Redirect to `/registration?success=true&name={userName}`

**Response (Error - HTTP 400):** JSON with validation errors

**Example Error Response:**
```json
{
  "errors": {
    "Name": ["'admin' is a reserved name and cannot be used"]
  }
}
```

---

## 🔐 Validation Rules & Attributes

### Complete Validation Matrix

| Field | Attribute | Client? | Server? | Error Message |
|-------|-----------|---------|---------|---------------|
| **Name** | `[Required]` | ✅ Yes | ✅ Yes | "The Name field is required" |
| **Name** | `[ReservedName]` | ❌ No | ✅ Yes | "'[name]' is a reserved name and cannot be used" |
| **Email** | `[Required]` | ✅ Yes | ✅ Yes | "The Email field is required" |
| **Email** | `[EmailAddress]` | ✅ Yes | ✅ Yes | "The Email field is not a valid e-mail address" |
| **Age** | `[Required]` | ✅ Yes | ✅ Yes | "The Age field is required" |
| **Age** | `[Range(18,120)]` | ✅ Yes | ✅ Yes | "The field Age must be between 18 and 120" |

---

### Reserved Names (Server-Only Validation)

**Why Server-Only:**
- Cannot send list of reserved names to browser (security/privacy)
- Custom validation attribute only enforced server-side
- Client cannot validate without knowing the list

**Reserved Names:**
- `admin`
- `administrator`
- `system`
- `root`
- `test`

**Validation is Case-Insensitive:**
- `admin`, `Admin`, `ADMIN` → all rejected
- `john`, `John`, `JOHN` → all accepted (not reserved)

**Implementation Location:** `Models/RegistrationModel.cs`

```csharp
public class ReservedNameAttribute : ValidationAttribute
{
    private static readonly List<string> ReservedNames = 
        new() { "admin", "system", "root", "test", "administrator" };
    
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        var name = value?.ToString() ?? string.Empty;
        if (ReservedNames.Contains(name, StringComparer.OrdinalIgnoreCase))
        {
            return new ValidationResult($"'{name}' is a reserved name and cannot be used");
        }
        return ValidationResult.Success;
    }
}
```

---

### DataAnnotations Attributes Explained

#### `[Required]`
- **What it does:** Field cannot be empty
- **Applied to:** Name, Email, Age
- **When checked:** Both browser and server
- **Message:** "The [Field] field is required"

#### `[EmailAddress]`
- **What it does:** Validates email format (basic pattern)
- **Applied to:** Email
- **When checked:** Both browser and server
- **Pattern:** Must contain `@` and domain
- **Message:** "The Email field is not a valid e-mail address"

#### `[Range(18, 120)]`
- **What it does:** Ensures numeric value within bounds
- **Applied to:** Age
- **When checked:** Both browser and server
- **Valid Range:** 18 to 120 (inclusive)
- **Message:** "The field Age must be between 18 and 120"

#### `[ReservedName]`
- **What it does:** Checks against reserved name list
- **Applied to:** Name
- **When checked:** Server ONLY
- **List:** admin, system, root, test, administrator
- **Message:** "'[name]' is a reserved name and cannot be used"
- **Why Server-Only:** Need to protect reserved names server-side

---

## 📚 Code Highlights

### Registration.razor - Form Setup

**Key Features:**
```razor
@page "/registration"

<EditForm Model="@Model" FormName="registrationForm" method="post" action="/api/register">
    <DataAnnotationsValidator />
    
    <!-- Three input fields with validation feedback -->
    <InputText @bind-Value="Model.Name" />
    <ValidationMessage For="@(() => Model.Name)" />
    
    <!-- ... similar for Email and Age -->
    
    <ValidationSummary />
    <button type="submit">Submit Registration</button>
</EditForm>

@code {
    [SupplyParameterFromQuery]
    public string? Success { get; set; }
    
    private RegistrationModel Model = new();
    private string? SuccessMessage { get; set; }
    
    protected override void OnInitialized()
    {
        if (Success?.Equals("true") == true)
        {
            SuccessMessage = $"Registration successful! Thank you, {Name}.";
        }
    }
}
```

### RegistrationModel.cs - Validation Rules

```csharp
public class RegistrationModel
{
    [Required]
    [ReservedName]
    public string Name { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [Range(18, 120)]
    public int Age { get; set; }
}
```

### RegistrationEndpoint.cs - Server Validation

```csharp
private static IResult HandleRegistration(HttpContext context, IFormCollection form)
{
    // Extract form data
    var model = new RegistrationModel
    {
        Name = form["Model.Name"].ToString(),
        Email = form["Model.Email"].ToString(),
        Age = int.TryParse(form["Model.Age"].ToString(), out var age) ? age : 0
    };

    // Server-side validation
    var validationContext = new ValidationContext(model);
    var results = new List<ValidationResult>();

    if (!Validator.TryValidateObject(model, validationContext, results, true))
    {
        // Build error response
        var errors = new Dictionary<string, string[]>();
        foreach (var result in results)
        {
            var key = result.MemberNames.FirstOrDefault() ?? "general";
            errors[key] = result.ErrorMessage?.Split(',') ?? Array.Empty<string>();
        }
        return Results.BadRequest(new { errors });
    }

    // Success: redirect to form with message
    return Results.Redirect($"/registration?success=true&name={Uri.EscapeDataString(model.Name ?? "")}");
}
```

---

## 🐛 Troubleshooting

### Issue: "Cannot GET /registration"
**Cause:** App not running or wrong URL

**Fix:**
```bash
dotnet run
# Check output for URL, typically https://localhost:7155
```

### Issue: Form doesn't submit (no POST)
**Cause:** Browser validation errors prevent submission

**Fix:** Fill all fields with valid data:
- Name: non-empty, not reserved
- Email: valid format (contains @)
- Age: between 18-120

### Issue: "System.ArgumentNullException: Value cannot be null. (Parameter 'instance')"
**Cause:** Form data not binding correctly

**Fix:** Ensure form uses proper Blazor naming:
```razor
<EditForm Model="@Model" FormName="registrationForm" method="post" action="/api/register">
```

### Issue: Validation doesn't show errors
**Cause:** DataAnnotationsValidator not included

**Fix:** Add validator component:
```razor
<DataAnnotationsValidator />
```

---

## 📖 References

### GitHub Issue #69531
- **Title:** [Validation] Browser feedback and server validation in a static SSR form
- **Validates:** Browser shows validation on blur/input, server validates on POST
- **Purpose:** Ensure Static SSR forms work without interactive components

### Blazor Documentation
- [Static SSR Rendering](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Form Handling](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-validation)
- [DataAnnotations Validation](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations)

### .NET 11 Release Notes
- [.NET 11 Preview](https://github.com/dotnet/core/releases)
- [Blazor in .NET 11](https://devblogs.microsoft.com/dotnet/)

---

## ✅ Validation Checklist

Before submitting your evidence:

### Code Quality
- [ ] No compilation errors
- [ ] All imports present
- [ ] Model validation attributes correct
- [ ] Endpoint correctly implements IFormCollection extraction
- [ ] Success redirect URL properly formatted

### Browser Behavior
- [ ] Blur triggers validation messages
- [ ] Input revalidates without additional blur
- [ ] Invalid submit doesn't POST
- [ ] Valid submit triggers POST

### Server Behavior
- [ ] Reserved name validation works
- [ ] All DataAnnotations validations enforced
- [ ] HTTP 200 on success (with redirect)
- [ ] HTTP 400 on validation error

### User Experience
- [ ] Form is Bootstrap styled
- [ ] Error messages are clear
- [ ] ValidationSummary shows all errors
- [ ] Success alert displays after redirect
- [ ] Form resets after success

### Network Evidence
- [ ] Network tab shows POST requests
- [ ] Response JSON properly formatted
- [ ] Status codes correct (200, 400)
- [ ] Request headers show form-urlencoded

---

## 📞 Support

For issues with this sample:
1. Check the **Troubleshooting** section above
2. Verify .NET version: `dotnet --version`
3. Review error messages in console output
4. Check DevTools Network tab for POST details
5. Ensure all files are present in project structure

**Test recorded on:** September 29, 2026
**Framework:** .NET 11 RC1
**Status:** ✅ Production Ready
These names are rejected by the server validation:
- `admin`
- `administrator`
- `system`
- `root`
- `test`

### Validation Messages

| Rule | Message |
|------|---------|
| Name required | "Name is required" |
| Name length | "Name cannot exceed 50 characters" |
| Reserved name | "'{name}' is a reserved name and cannot be used" |
| Email required | "Email is required" |
| Invalid email | "Email must be a valid email address" |
| Age required | "Age is required" |
| Age out of range | "Age must be between 18 and 120" |

---

## Testing Guidelines

### What to Test

1. **Browser Validation Timing**
   - ✅ Messages appear only after blur
   - ✅ Messages clear on input after error
   - ✅ No POST on invalid submit

2. **Server Validation**
   - ✅ Reserved names rejected by server
   - ✅ Error preserved on form after failed POST
   - ✅ Success confirmation shows name after successful POST

3. **Form Recovery**
   - ✅ Invalid submit doesn't clear form
   - ✅ User can correct and resubmit
   - ✅ Success clears form for next entry

4. **No Native HTML Validation**
   - ✅ Browser's native validation is bypassed
   - ✅ Only DataAnnotations rules enforced
   - ✅ No browser-native error messages

### Evidence to Capture

Create a `validation-evidence/` folder and capture:

1. **Email field scenarios**
   - Before blur (invalid, no message)
   - After blur (error message)
   - During correction (clearing message)

2. **Form submission scenarios**
   - Blocked submit (empty field)
   - Blocked submit (invalid email)
   - Accepted submit (success confirmation)
   - Server-rejected submit (error message)

3. **Network traces**
   - Failed submit (no POST)
   - Successful submit (POST 200)
   - Server error (POST 400)

4. **Published app**
   - Same tests from published application

---

## Documentation

- **[VALIDATION_TEST_GUIDE.md](VALIDATION_TEST_GUIDE.md)** - Comprehensive test procedures and expected behaviors
- **[TEST_REPORT_TEMPLATE.md](TEST_REPORT_TEMPLATE.md)** - Template for documenting test results
- **[Microsoft Docs: Client-side validation for Blazor static SSR forms](https://learn.microsoft.com/aspnet/core/blazor/forms/validation-client-side?view=aspnetcore-11.0)**
- **[Microsoft Docs: Blazor forms validation](https://learn.microsoft.com/aspnet/core/blazor/forms/validation?view=aspnetcore-11.0)**

---

## GitHub Issue Reference

- **Issue**: [#69531 - [Validation] Browser feedback and server validation in a static SSR form](https://github.com/dotnet/aspnetcore/issues/69531)
- **Area**: Blazor, Forms, Validation
- **Type**: Validation Scenario Test

---

## Key Requirements (Must Hold)

These requirements must be met for the test to pass:

1. ✅ Before blur: Invalid email shows **NO** validation message
2. ✅ After blur: Invalid email shows message with invalid styling, **NO** POST
3. ✅ After error: Editing email revalidates on input, clears message **without** blur/POST
4. ✅ Invalid submit: **NO** POST sent, **NO** success confirmation
5. ✅ Valid submit: **POST** sent, success confirmation shows submitted name
6. ✅ Reserved name: **POST** sent, server rejects, error shown, **NO** confirmation
7. ✅ Published app: All behaviors **hold** in published application

---

## Technologies

- **.NET 11.0**: Latest .NET runtime
- **Blazor Static SSR**: Server-side rendering without interactivity
- **Razor Components**: Reusable UI components
- **DataAnnotations**: Built-in validation framework
- **Bootstrap 5**: CSS framework for styling

---

## Browser Support

Tested with:
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Other modern browsers supporting Blazor

---

## Troubleshooting

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

### Port Already in Use
```bash
# Run on different port
dotnet run --urls "https://localhost:7156"
```

### Validation Not Working
1. Ensure `DataAnnotationsValidator` is in the form
2. Check that `InputText` is using `@bind-Value`
3. Verify `ValidationMessage` has correct `For` expression

### Server Endpoint Not Responding
1. Check that `/api/register` endpoint is registered in `Program.cs`
2. Verify `HttpClient` is added to services
3. Check browser console for network errors

---

## Contributing

When adding new test cases:
1. Update `VALIDATION_TEST_GUIDE.md`
2. Add screenshots to `validation-evidence/`
3. Document in `TEST_REPORT_TEMPLATE.md`
4. Update this README if needed

---

## License

This test application is provided as part of the .NET ecosystem validation testing.

---

## Support

For issues or questions about this test application:
1. Check [GitHub Issue #69531](https://github.com/dotnet/aspnetcore/issues/69531)
2. Review the [VALIDATION_TEST_GUIDE.md](VALIDATION_TEST_GUIDE.md)
3. Refer to [Microsoft Documentation](https://learn.microsoft.com/aspnet/core/blazor/forms/)

---

**Last Updated**: January 2025  
**Version**: 1.0  
**Status**: Active
