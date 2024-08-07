Prerequisites:
- Dotnet 8
- an IDE with .net Aspire Support (Eg latest version of VS 2022)
- docker

Quick start:

- In a console 1:
  - Go to `WolverineHoP.WolverineDocumentApi`
  - Run `dotnet run -- codegen write`
- In a console 2:
  - Go to `WolverineHoP.WolverineEventsApi`
  - Run `dotnet run -- codegen write`

- Open solution
- Run the `https` launch profile for the `WolverineHoP.AppHost` (should be the default)
- In console 1, run `dotnet watch run --launch-profile https`
- In console 2, run `dotnet watch run --launch-profile https`

Everything should open in your browser.  Go to the frontend page, and click `Seed data`. Wait until `Done` appears.
