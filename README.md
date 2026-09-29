# Blazor Static SSR Form Validation Test Application

## Overview

This is a comprehensive test application for GitHub Issue #69531: **[Validation] Browser feedback and server validation in a static SSR form**

The application demonstrates how static SSR forms in Blazor can:
- Show DataAnnotations validation messages in the browser without interactive components
- Validate on field blur and revalidate on input during correction
- Maintain server-side validation for custom rules
- Provide proper form recovery after failed submissions
- Display success confirmations only after successful server POST

---

## Project Structure

```
BlazorStaticSSRFormValidation/
├── StaticSSRFormValidation/
│   ├── Components/
│   │   ├── Pages/
│   │   │   ├── Home.razor                 # Home page with test overview
│   │   │   ├── Registration.razor         # Main form for testing
│   │   │   ├── Error.razor
│   │   │   └── NotFound.razor
│   │   ├── Layout/                        # Layout components
│   │   ├── App.razor
│   │   └── _Imports.razor
│   ├── Models/
│   │   └── RegistrationModel.cs           # Form model with validation attributes
│   ├── Endpoints/
│   │   └── RegistrationEndpoint.cs        # API endpoint for form submission
│   ├── wwwroot/
│   │   ├── app.css                        # Validation styling
│   │   └── lib/bootstrap/                 # Bootstrap CSS framework
│   ├── Program.cs                         # Application startup configuration
│   ├── appsettings.json
│   └── StaticSSRFormValidation.csproj
│
├── VALIDATION_TEST_GUIDE.md               # Comprehensive test procedures
├── TEST_REPORT_TEMPLATE.md                # Template for documenting test results
└── README.md                              # This file
```

---

## Key Features

### 1. Static SSR Form with DataAnnotations Validation
- **EditForm** with form name binding
- **DataAnnotationsValidator** for client-side validation
- **ValidationMessage** components for field-level feedback
- **ValidationSummary** for error overview
- **No native HTML constraints** (no `required`, `type="email"`, or `pattern`)

### 2. Validation Rules

#### Client-Side (Browser) Validation
- **Name**: Required, max 50 characters
- **Email**: Required, valid email format
- **Age**: Required, range 18-120

#### Server-Side Validation
- **Name**: Rejects reserved names (admin, administrator, system, root, test)
- Custom `ReservedNameAttribute` implements server-only rule

### 3. Form Behavior

#### Before Blur
- No validation messages appear
- User can type without feedback

#### After Blur
- Invalid fields show error messages
- Invalid styling (red border) applied
- No form submission occurs

#### During Correction
- Revalidates on input
- Error message clears when valid
- No additional blur or POST needed

#### On Submit
- **If Invalid**: No POST sent, form preserved with errors
- **If Valid**: POST sent to `/api/register`, success confirmation shows
- **If Server Error**: POST sent, server error shown, form preserved

---

## Getting Started

### Prerequisites
- .NET SDK 11 or later
- Visual Studio Code or Visual Studio

### Installation

1. **Clone or open the project**
   ```bash
   cd d:\Validation\BlazorStaticSSRFormValidation
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

### Running in Development

```bash
cd StaticSSRFormValidation
dotnet run
```

The application will start at: `https://localhost:7155` (or shown URL)

**Navigate to**:
- Home Page: `https://localhost:7155/`
- Registration Form: `https://localhost:7155/registration`

### Publishing for Production

```bash
cd StaticSSRFormValidation
dotnet publish -c Release -o ./bin/publish
```

**Run published app**:
```bash
dotnet ./bin/publish/StaticSSRFormValidation.dll
```

---

## Test Scenarios

### Scenario 1: Invalid Email Before Blur
1. Type invalid email (e.g., `invalidemail`)
2. **Expected**: No validation message

### Scenario 2: Invalid Email After Blur
1. Leave the field (blur)
2. **Expected**: Message appears with invalid styling, no POST

### Scenario 3: Correction Without Blur
1. Fix the email while in the field
2. **Expected**: Message clears without blur or POST

### Scenario 4: Blocked Invalid Submit
1. Submit with empty required field or invalid email
2. **Expected**: No POST, form stays with errors

### Scenario 5: Valid Submission
1. Fill all fields with valid data
2. Submit
3. **Expected**: POST sent (200), success confirmation shows

### Scenario 6: Server-Rejected Reserved Name
1. Enter "admin" (or other reserved name)
2. Fill other fields with valid data
3. Submit
4. **Expected**: POST sent, server rejects (400), error shown, no confirmation

---

## Form Endpoints

### GET `/` 
Home page with test overview

### GET `/registration`
Registration form page

### POST `/api/register`
Form submission endpoint

**Request**:
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "age": 25
}
```

**Response (Success - 200)**:
```json
{
  "name": "John Doe",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

**Response (Error - 400)**:
```json
{
  "errors": {
    "name": ["'admin' is a reserved name and cannot be used"]
  }
}
```

---

## Validation Rules Reference

### Reserved Names (Server-Only)
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
