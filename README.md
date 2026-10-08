# Blazor Static SSR Form Validation

This repository validates the scenario in
[dotnet/aspnetcore#69531](https://github.com/dotnet/aspnetcore/issues/69531):
browser feedback and server validation in a static SSR form on .NET 11 RC1.

## Current evidence status

The application was corrected after review feedback identified that the previous
API-style POST navigated to a raw HTTP 400 response. The recordings were replaced
for the corrected implementation, and the published files were regenerated from
the clean test revision identified below.

Before resubmitting the report:

1. Use test revision `a0a386f0e45203e857bec5f534e3080802d52368`.
2. Build, publish, and run from a clean checkout of that exact revision.
3. Use only recordings captured from that revision.
4. Commit the regenerated published output and reproducibility logs.
5. Ensure the report uses the same full SHA and environment details.

Do not mark the overall or published-output outcome as `Works` until every
mandatory path below passes in both development and published execution.

## Implementation

The registration page is a static SSR component with no interactive render mode.
It uses:

- `EditForm` with `FormName="registrationForm"`.
- A model supplied by `[SupplyParameterFromForm]`.
- `DataAnnotationsValidator`.
- Field-level `ValidationMessage` components and `ValidationSummary`.
- Text inputs without native `required`, `type="email"`, or `pattern`
  constraints.
- Built-in required, email, string-length, and range validation attributes.
- A plain custom `ReservedNameAttribute` with no client rule provider.

The custom attribute is attached to `RegistrationModel.Name`. Browser-supported
rules can block an invalid submission without a POST. Because the custom rule
has no client rule provider, a reserved name passes browser validation, reaches
the server, and is rejected during server-side DataAnnotations validation. The
static SSR response re-renders the same form with its posted values, the Name
field message, and the validation summary.

In the tested RC1 build, browser reconciliation places a server-only custom
error into field-bound validation containers but leaves the standard
`ValidationSummary` container hidden. The summary card therefore contains a
second field-bound Name message. CSS hides that fallback whenever the standard
summary is populated, avoiding duplicate built-in validation messages.

The success message is assigned only by the component's `OnValidSubmit` handler
after a valid server POST. It is not populated from query-string values, so a
direct GET cannot fabricate a confirmation.

## Validation rules

| Field | Rule | Browser-supported | Server |
|---|---|---:|---:|
| Name | Required | Yes | Yes |
| Name | Maximum length 50 | Yes | Yes |
| Name | Not `admin`, `administrator`, `system`, `root`, or `test` | No | Yes |
| Email | Required | Yes | Yes |
| Email | Email address | Yes | Yes |
| Age | Required | Yes | Yes |
| Age | Range 18-120 | Yes | Yes |

## Prerequisites

- Git
- .NET SDK `11.0.100-rc.1.26425.128`
- Microsoft Edge or another supported browser

The required SDK is pinned by `global.json`.

## Clean-clone reproduction

Replace `<full-corrected-sha>` with the complete SHA of the committed correction.

```powershell
git clone https://github.com/Vinoth2562000/BlazorStaticSSRFormValidation.git
Set-Location .\BlazorStaticSSRFormValidation
git checkout <full-corrected-sha>
git status --short
git rev-parse HEAD
dotnet --info
dotnet restore .\StaticSSRFormValidation\StaticSSRFormValidation.csproj
dotnet build .\StaticSSRFormValidation\StaticSSRFormValidation.csproj --configuration Release --no-restore
```

`git status --short` must produce no output, and `git rev-parse HEAD` must match
the SHA stated in the report.

## Development execution

```powershell
dotnet run --project .\StaticSSRFormValidation\StaticSSRFormValidation.csproj --launch-profile http
```

Open `http://localhost:5143/registration`.

## Published execution

Use a clean output directory so no files from an earlier revision remain.

```powershell
dotnet publish .\StaticSSRFormValidation\StaticSSRFormValidation.csproj `
  --configuration Release `
  --output .\artifacts\publish

Set-Location .\artifacts\publish
.\StaticSSRFormValidation.exe --urls http://localhost:5144
```

Open `http://localhost:5144/registration`. Capture the publish command output and
the application startup output as evidence.

## Mandatory test paths

Keep the browser developer tools Network panel open and preserve the log.

### 1. Invalid email feedback and recovery

1. Open `/registration` with a fresh page load.
2. Type `invalid-email` in Email without leaving the field.
3. Verify that no validation message appears while initially typing.
4. Move focus out of Email.
5. Verify the email message and invalid styling appear without a POST.
6. Without leaving Email again, change the value to `user@example.com`.
7. Verify the message and invalid styling clear without a POST.

Expected result: `Pass` only if feedback first appears on change or blur and
then clears on input after the first error.

### 2. Built-in invalid submission is blocked

1. Enter a non-reserved name and valid age.
2. Leave Email empty or enter an invalid email.
3. Submit.
4. Verify no POST appears in the Network panel.
5. Verify no success confirmation appears.

Expected result: `Pass` only if browser-supported validation blocks the POST.
An empty-form submission must not be documented as an expected HTTP 400.

### 3. Successful server POST

1. Enter `Vinoth`, `vinoth@example.com`, and `30`.
2. Submit.
3. Verify a POST occurs.
4. Verify the rendered registration page displays a confirmation containing
   `Vinoth`.
5. Refresh or directly navigate to `/registration`.
6. Verify the confirmation is absent.
7. Navigate to `/registration?success=true&name=Anything`.
8. Verify the confirmation is still absent.

Expected result: `Pass` only if the confirmation follows a successful POST and
cannot be created by direct GET navigation or query-string manipulation.

### 4. Server-only reserved-name rejection

1. Enter `admin`, `admin@example.com`, and `30`.
2. Submit.
3. Verify a POST occurs because all browser-supported rules pass.
4. Verify the browser remains on the registration form.
5. Verify all entered values are preserved.
6. Verify `'Name' is a reserved name and cannot be used` appears beside Name.
7. Verify the same error appears in the validation summary.
8. Verify no success confirmation appears.
9. Verify the browser does not display a raw JSON response.

Expected result: `Pass` only if the server rejection is rendered back into the
form's validation state.

### 5. Published application

Repeat paths 1 through 4 against the published URL. The aggregate published row
is `Failed` if any required browser-feedback or server-POST outcome fails.

## Evidence checklist

Capture new evidence only after checking out the reported full SHA:

- Clean `git status --short` output.
- Full `git rev-parse HEAD` output.
- Full `dotnet --info` output.
- Restore, Release build, and publish output.
- Development application startup output.
- Published application startup output.
- Invalid email before blur, after blur, and while correction clears it.
- Blocked built-in invalid submission with no POST.
- Accepted POST with a server-generated name confirmation.
- Direct GET negative checks with no confirmation.
- Reserved-name POST with preserved values and field/summary errors.
- All mandatory paths repeated against the published app.

Each recording name or report table row should state whether it was captured
from development or published execution.

## Environment used for the correction

These values describe the machine used to implement and locally verify the
correction. Record the actual values again when capturing final evidence.

| Item | Value |
|---|---|
| OS | Windows 11, build 26100 |
| .NET SDK | 11.0.100-rc.1.26425.128 |
| ASP.NET Core runtime | 11.0.0-rc.1.26425.128 |
| Microsoft Edge | 154.0.4258.37 |
| VS Code | 1.140.0, x64 |
| Visual Studio | 18.10.3 |
| Branch | `main` |
| Previous failed baseline | `68f52c217d36dc43ddfdea0fc38f3bed7f07c2d7` |
| Corrected test commit | `a0a386f0e45203e857bec5f534e3080802d52368` |

## Project layout

```text
StaticSSRFormValidation/
  Components/Pages/Registration.razor
  Models/RegistrationModel.cs
  Program.cs
  StaticSSRFormValidation.csproj
Evidence/
global.json
README.md
```

The obsolete `/api/register` endpoint is intentionally removed. Static SSR form
binding and component server validation now own the complete POST lifecycle.
