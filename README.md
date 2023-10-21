<p align="center">
<a href=https://github.com/johngthecreator/dh-study target="_blank">
<img src='/frontend/public/octo_logo_clear.png' width="60%" alt="Banner" />
</a>
</p>



<p align="center">
<img src="https://img.shields.io/github/contributors/johngthecreator/dh-study" alt="GitHub contributors" />
<img src="https://img.shields.io/github/discussions/johngthecreator/dh-study" alt="GitHub discussions" />
<img src="https://img.shields.io/github/issues/johngthecreator/dh-study" alt="GitHub issues" />
<img src="https://img.shields.io/github/issues-pr/johngthecreator/dh-study" alt="GitHub pull request" />
</p>

<p></p>
<p></p>

## 🔍 Table of Contents

* [💻 Stack](#stack)

* [📝 Project Summary](#project-summary)

* [⚙️ Setting Up](#setting-up)

* [🚀 Run Locally](#run-locally)

* [🙌 Contributors](#contributors)

* [📄 License](#license)

## 💻 Stack

Include a concise explanation about the Tech Stack employed.

## 📝 Project Summary

- [Backend/Backend](Backend/Backend): Core functionality and main entry point of the backend application.
- [Backend/Backend/Auth](Backend/Backend/Auth): Handles authentication and authorization logic for the backend.
- [Backend/Backend/Controllers](Backend/Backend/Controllers): Contains the API controllers responsible for handling incoming requests.
- [Backend/Backend/Services](Backend/Backend/Services): Houses various services used by the backend application.
- [Backend/Backend/Services/AiServices](Backend/Backend/Services/AiServices): Provides AI-related services and functionality.
- [Backend/Backend/Services/DataService](Backend/Backend/Services/DataService): Handles data-related operations and interactions with the database.
- [frontend](frontend): Main entry point for the frontend application.
- [frontend/public](frontend/public): Contains static assets and resources used by the frontend.
- [frontend/src](frontend/src): Houses the source code for the frontend application.
- [frontend/src/components](frontend/src/components): Contains reusable components used throughout the frontend application.

## ⚙️ Setting Up

#### Your Environment Variable

- Create a `sensitivesettings.json` in the /Backend/Backend/ directory and add the following keys:
  - OpenAiApiKey
  - AzureBlobAccessKey1
  - AzureBlobConnectionString1
  - StorageAccountName
  - BlobServiceEndpoint

- Install the following nuget packages:
  - Azure.Storage.Blobs
  - DocX
  - itext7
  - Microsoft.SemanticKernel
  - Microsoft.Azure.Cosmos
  - Swashbuckle.AspNetCore
  - FirebaseAdmin
  - Microsoft.AspNetCore.Authentication

- Have fun running!

## 🚀 Run Locally
1.Clone the dh-study repository:
```sh
git clone https://github.com/johngthecreator/dh-study
```
2.Install the dependencies with one of the package managers listed below:
```sh 
See above
```
3.Start the development mode:
```sh 
dotnet build
dotnet run 
```

## 🙌 Contributors

<table style="border:1px solid #404040;text-align:center;width:100%">
<tr><td style="width:14.29%;border:1px solid #404040;">
        <a href="https://github.com/matthewbrag" spellcheck="false">
          <img src="https://avatars.githubusercontent.com/u/93839397?v=4?s=100" width="100px;" alt="matthewbrag"/>
          <br />
          <b>matthewbrag</b>
        </a>
        <br />
        <a href="https://github.com/johngthecreator/dh-study/commits?author=matthewbrag" title="Contributions" spellcheck="false">
          22 contributions
        </a>
      </td><td style="width:14.29%;border:1px solid #404040;">
        <a href="https://github.com/axolmain" spellcheck="false">
          <img src="https://avatars.githubusercontent.com/u/85575775?v=4?s=100" width="100px;" alt="axolmain"/>
          <br />
          <b>axolmain</b>
        </a>
        <br />
        <a href="https://github.com/johngthecreator/dh-study/commits?author=axolmain" title="Contributions" spellcheck="false">
          21 contributions
        </a>
      </td><td style="width:14.29%;border:1px solid #404040;">
        <a href="https://github.com/ShelbyVasas" spellcheck="false">
          <img src="https://avatars.githubusercontent.com/u/114014297?v=4?s=100" width="100px;" alt="ShelbyVasas"/>
          <br />
          <b>ShelbyVasas</b>
        </a>
        <br />
        <a href="https://github.com/johngthecreator/dh-study/commits?author=ShelbyVasas" title="Contributions" spellcheck="false">
          8 contributions
        </a>
      </td></table>

## 📄 License

MIT License

Copyright (c) [2023] [John Gorriceta]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

