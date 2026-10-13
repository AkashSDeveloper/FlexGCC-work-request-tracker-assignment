# FlexGCC Work Request Tracker (2-hour MVP)

**Stack:** ASP.NET Core Web API (.NET 8), React + Vite, SQLite + EF Core.

This version prioritizes the assignment's core flow over production polish. It includes list/search/status filter/pagination, create request, update status, add note, basic validation/error responses, SQL schema/indexes, and loading/error states.

## Run it

### 1. Backend
Install the .NET 8 SDK, then:
```bash
cd backend
dotnet restore
dotnet run
```
API base URL: `http://localhost:5080`; Swagger UI: `http://localhost:5080/swagger`.

### 2. Frontend
Install Node.js 20+, then open a second terminal:
```bash
cd frontend
npm install
npm run dev
```
Open `http://localhost:5173`.

### 3. Quick manual checks
- List requests and page through results.
- Search for a title/client name and filter by status.
- Create a request with required fields.
- Change a request's status.
- Add a note.
- Try an invalid status and an unknown request ID using Swagger.

## Deliberate 2-hour trade-offs / known issues
- No authentication/authorization; this is a local assignment demo only.
- Notes are appended to one field, not stored as a separate history table.
- No automated test project included; manual smoke checks are required before submission.
- Minimal styling and no deployment/CI/CD.
- SQLite is used to keep setup simple; `schema.sql` is included separately.

## Before you submit
1. Run both projects and fix any local environment/build errors.
2. Review the code and be ready to explain it.
3. Fill in your name/contact in the assignment document.
4. Paste your real GitHub URL or ZIP filename in section A.
5. Disclose AI help accurately. Do not claim tests were run unless you ran them.
